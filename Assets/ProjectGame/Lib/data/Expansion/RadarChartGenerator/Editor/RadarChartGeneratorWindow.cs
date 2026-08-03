
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace RadarChartGenerator
{
    /// <summary>
    /// レーダーチャートジェネレーター — Unityエディタ拡張GUIウィンドウ
    /// メニュー: Tools > Radar Chart Generator
    /// </summary>
    public class RadarChartGeneratorWindow : EditorWindow
    {
        // =====================
        // GUIステート
        // =====================
        private int _vertexCount = 5;
        private int _divisions = 5;
        private float _radius = 180f;

        private RadarChart.FillMode _fillMode = RadarChart.FillMode.SingleColor;
        private Color _fillColor = new Color(0.2f, 0.5f, 1f, 0.4f);
        private Gradient _fillGradient = new Gradient();
        private List<Color> _vertexColors = new List<Color>();
        private int _gradientSteps = 16;

        private bool _showOutline = true;
        private Color _outlineColor = new Color(0.2f, 0.5f, 1f, 1f);
        private float _outlineWidth = 2f;

        private bool _showGrid = true;
        private Color _gridColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
        private float _gridLineWidth = 1f;

        private bool _showAxes = true;
        private Color _axisColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        private float _axisLineWidth = 1f;

        private bool _animateChanges = true;
        private float _animationSpeed = 3f;

        private List<float> _values = new List<float> { 0.8f, 0.6f, 0.9f, 0.5f, 0.7f };
        private List<string> _labels = new List<string> { "攻撃", "防御", "速度", "魔法", "体力" };

        private Vector2 _scrollPos;
        private bool _foldBasic = true;
        private bool _foldFill = true;
        private bool _foldOutline = true;
        private bool _foldGrid = true;
        private bool _foldValues = true;
        private bool _foldAnim = true;

        private SerializedObject _gradientSO;
        private GradientWrapper _gradientWrapper;

        // =====================
        // メニュー登録
        // =====================
        [MenuItem("Tools/Radar Chart Generator")]
        public static void Open()
        {
            var win = GetWindow<RadarChartGeneratorWindow>("Radar Chart Generator");
            win.minSize = new Vector2(400, 600);
            win.Show();
        }

        // =====================
        // 初期化
        // =====================
        private void OnEnable()
        {
            InitGradient();
            SyncListSizes();

            _gradientWrapper = ScriptableObject.CreateInstance<GradientWrapper>();
            _gradientWrapper.gradient = _fillGradient;
            _gradientSO = new SerializedObject(_gradientWrapper);
        }

        private void OnDisable()
        {
            if (_gradientWrapper != null)
                DestroyImmediate(_gradientWrapper);
        }

        private void InitGradient()
        {
            _fillGradient = new Gradient();
            _fillGradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(new Color(0.2f, 0.5f, 1f), 0f),
                    new GradientColorKey(new Color(0.5f, 0.8f, 1f), 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0.8f, 0f),
                    new GradientAlphaKey(0.2f, 1f)
                }
            );
        }

        private void SyncListSizes()
        {
            while (_values.Count < _vertexCount) _values.Add(0.5f);
            while (_values.Count > _vertexCount) _values.RemoveAt(_values.Count - 1);

            while (_labels.Count < _vertexCount) _labels.Add("項目 " + (_labels.Count + 1));
            while (_labels.Count > _vertexCount) _labels.RemoveAt(_labels.Count - 1);

            while (_vertexColors.Count < _vertexCount) _vertexColors.Add(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.7f, 1f, 0.7f, 0.9f));
            while (_vertexColors.Count > _vertexCount) _vertexColors.RemoveAt(_vertexColors.Count - 1);
        }

        // =====================
        // GUI描画
        // =====================
        private void OnGUI()
        {
            DrawHeader();
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            DrawBasicSection();
            DrawFillSection();
            DrawOutlineSection();
            DrawGridSection();
            DrawValuesSection();
            DrawAnimSection();
            DrawActionButtons();

            EditorGUILayout.EndScrollView();
        }

        // =====================
        // ヘッダー
        // =====================
        private void DrawHeader()
        {
            var headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.4f, 0.8f, 1f) }
            };
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("◆ Radar Chart Generator", headerStyle, GUILayout.Height(28));
            DrawSeparator();
        }

        // =====================
        // 基本設定セクション
        // =====================
        private void DrawBasicSection()
        {
            _foldBasic = DrawFoldout("基本設定", _foldBasic);
            if (!_foldBasic) return;
            EditorGUI.indentLevel++;

            int newVertex = EditorGUILayout.IntSlider("頂点数", _vertexCount, 3, 12);
            if (newVertex != _vertexCount)
            {
                _vertexCount = newVertex;
                SyncListSizes();
            }

            _divisions = EditorGUILayout.IntSlider("分割数（同心リング）", _divisions, 1, 10);
            _radius = EditorGUILayout.Slider("半径", _radius, 50f, 400f);

            EditorGUI.indentLevel--;
            DrawSeparator();
        }

        // =====================
        // 塗りつぶしセクション
        // =====================
        private void DrawFillSection()
        {
            _foldFill = DrawFoldout("塗りつぶし設定", _foldFill);
            if (!_foldFill) return;
            EditorGUI.indentLevel++;

            _fillMode = (RadarChart.FillMode)EditorGUILayout.EnumPopup("塗りつぶしモード", _fillMode);

            switch (_fillMode)
            {
                case RadarChart.FillMode.SingleColor:
                    _fillColor = EditorGUILayout.ColorField("塗りつぶし色", _fillColor);
                    break;

                case RadarChart.FillMode.RadialGradient:
                    EditorGUILayout.LabelField("グラデーション（内側 → 外側）", EditorStyles.miniLabel);

                    _gradientSO.Update();
                    var gradProp = _gradientSO.FindProperty("gradient");
                    EditorGUILayout.PropertyField(gradProp, new GUIContent("グラデーション"));
                    if (_gradientSO.ApplyModifiedProperties())
                    {
                        _fillGradient = _gradientWrapper.gradient;
                    }

                    _gradientSteps = EditorGUILayout.IntSlider("グラデーション精度", _gradientSteps, 2, 32);
                    break;

                case RadarChart.FillMode.VertexColor:
                    EditorGUILayout.LabelField("各頂点の色", EditorStyles.miniLabel);
                    for (int i = 0; i < _vertexCount; i++)
                    {
                        string lbl = (i < _labels.Count) ? _labels[i] : "頂点 " + (i + 1);
                        _vertexColors[i] = EditorGUILayout.ColorField($"  頂点 {i + 1}（{lbl}）", _vertexColors[i]);
                    }
                    break;
            }

            EditorGUI.indentLevel--;
            DrawSeparator();
        }

        // =====================
        // アウトラインセクション
        // =====================
        private void DrawOutlineSection()
        {
            _foldOutline = DrawFoldout("アウトライン設定", _foldOutline);
            if (!_foldOutline) return;
            EditorGUI.indentLevel++;

            _showOutline = EditorGUILayout.Toggle("アウトライン表示", _showOutline);
            if (_showOutline)
            {
                _outlineColor = EditorGUILayout.ColorField("アウトラインの色", _outlineColor);
                _outlineWidth = EditorGUILayout.Slider("アウトラインの太さ", _outlineWidth, 0.5f, 8f);
            }

            EditorGUI.indentLevel--;
            DrawSeparator();
        }

        // =====================
        // グリッドセクション
        // =====================
        private void DrawGridSection()
        {
            _foldGrid = DrawFoldout("グリッド／軸線設定", _foldGrid);
            if (!_foldGrid) return;
            EditorGUI.indentLevel++;

            _showGrid = EditorGUILayout.Toggle("グリッド表示", _showGrid);
            if (_showGrid)
            {
                _gridColor = EditorGUILayout.ColorField("グリッド色", _gridColor);
                _gridLineWidth = EditorGUILayout.Slider("グリッド線の太さ", _gridLineWidth, 0.5f, 4f);
            }

            EditorGUILayout.Space(4);

            _showAxes = EditorGUILayout.Toggle("軸線表示", _showAxes);
            if (_showAxes)
            {
                _axisColor = EditorGUILayout.ColorField("軸線色", _axisColor);
                _axisLineWidth = EditorGUILayout.Slider("軸線の太さ", _axisLineWidth, 0.5f, 4f);
            }

            EditorGUI.indentLevel--;
            DrawSeparator();
        }

        // =====================
        // 頂点値セクション
        // =====================
        private void DrawValuesSection()
        {
            _foldValues = DrawFoldout("頂点データ設定", _foldValues);
            if (!_foldValues) return;
            EditorGUI.indentLevel++;

            // ランダム生成ボタン
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("ランダム生成", GUILayout.Width(100)))
                {
                    for (int i = 0; i < _values.Count; i++) _values[i] = Random.Range(0.2f, 1f);
                }
                if (GUILayout.Button("全て1.0に", GUILayout.Width(90)))
                {
                    for (int i = 0; i < _values.Count; i++) _values[i] = 1f;
                }
                if (GUILayout.Button("全て0.5に", GUILayout.Width(90)))
                {
                    for (int i = 0; i < _values.Count; i++) _values[i] = 0.5f;
                }
            }

            EditorGUILayout.Space(4);
            for (int i = 0; i < _vertexCount; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    // ラベル編集
                    _labels[i] = EditorGUILayout.TextField(_labels[i], GUILayout.Width(80));
                    _values[i] = EditorGUILayout.Slider(_values[i], 0f, 1f);
                    EditorGUILayout.LabelField($"{_values[i]:F2}", GUILayout.Width(36));
                }
            }

            EditorGUI.indentLevel--;
            DrawSeparator();
        }

        // =====================
        // アニメーションセクション
        // =====================
        private void DrawAnimSection()
        {
            _foldAnim = DrawFoldout("アニメーション設定", _foldAnim);
            if (!_foldAnim) return;
            EditorGUI.indentLevel++;

            _animateChanges = EditorGUILayout.Toggle("値変化アニメーション", _animateChanges);
            if (_animateChanges)
                _animationSpeed = EditorGUILayout.Slider("アニメーション速度", _animationSpeed, 0.5f, 10f);

            EditorGUI.indentLevel--;
            DrawSeparator();
        }

        // =====================
        // アクションボタン
        // =====================
        private void DrawActionButtons()
        {
            EditorGUILayout.Space(8);

            var btnStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                fixedHeight = 38
            };

            // メイン生成ボタン
            GUI.backgroundColor = new Color(0.3f, 0.8f, 0.4f);
            if (GUILayout.Button("▶  チャートを生成（Canvas に配置）", btnStyle))
                CreateRadarChart();
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(4);

            // 選択中のチャートに設定反映
            GUI.backgroundColor = new Color(0.3f, 0.6f, 1f);
            if (GUILayout.Button("↺  選択中のチャートに設定を反映", btnStyle))
                ApplyToSelected();
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(4);

            // ラベル付きで生成
            GUI.backgroundColor = new Color(0.9f, 0.7f, 0.2f);
            if (GUILayout.Button("★  ラベル付きチャートを生成", btnStyle))
                CreateRadarChartWithLabels();
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(8);
        }

        // =====================
        // チャート生成
        // =====================
        private void CreateRadarChart()
        {
            // Canvas確保
            var canvas = EnsureCanvas();

            // 親パネル
            var chartObj = new GameObject("RadarChart");
            chartObj.transform.SetParent(canvas.transform, false);

            var rt = chartObj.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(_radius * 2.2f, _radius * 2.2f);
            rt.anchoredPosition = Vector2.zero;

            // RadarChartコンポーネント追加
            var chart = chartObj.AddComponent<RadarChart>();
            ApplySettings(chart);

            Undo.RegisterCreatedObjectUndo(chartObj, "Create Radar Chart");
            Selection.activeGameObject = chartObj;
            Debug.Log($"[RadarChart] 生成完了: {chartObj.name}");
        }

        private void CreateRadarChartWithLabels()
        {
            var canvas = EnsureCanvas();

            // ルートオブジェクト
            var rootObj = new GameObject("RadarChart_WithLabels");
            rootObj.transform.SetParent(canvas.transform, false);
            var rootRt = rootObj.AddComponent<RectTransform>();
            rootRt.sizeDelta = new Vector2(_radius * 2.8f, _radius * 2.8f);
            rootRt.anchoredPosition = Vector2.zero;

            // チャート本体
            var chartObj = new GameObject("Chart");
            chartObj.transform.SetParent(rootObj.transform, false);
            var chartRt = chartObj.AddComponent<RectTransform>();
            chartRt.sizeDelta = new Vector2(_radius * 2.2f, _radius * 2.2f);
            chartRt.anchoredPosition = Vector2.zero;
            var chart = chartObj.AddComponent<RadarChart>();
            ApplySettings(chart);

            // ラベルオブジェクト
            for (int i = 0; i < _vertexCount; i++)
            {
                float angle = -Mathf.PI / 2f + (2f * Mathf.PI * i / _vertexCount);
                float labelRadius = _radius * 1.25f;
                Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * labelRadius;

                var labelObj = new GameObject($"Label_{i}_{_labels[i]}");
                labelObj.transform.SetParent(rootObj.transform, false);
                var labelRt = labelObj.AddComponent<RectTransform>();
                labelRt.sizeDelta = new Vector2(100f, 30f);
                labelRt.anchoredPosition = pos;

                var tmp = labelObj.AddComponent<Text>();
                tmp.text = _labels[i];
                tmp.fontSize = 14;
                tmp.alignment = TextAnchor.MiddleCenter;
                tmp.color = Color.white;
                var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                tmp.font = font;
            }

            Undo.RegisterCreatedObjectUndo(rootObj, "Create Radar Chart With Labels");
            Selection.activeGameObject = rootObj;
            Debug.Log($"[RadarChart] ラベル付き生成完了: {rootObj.name}");
        }

        private void ApplyToSelected()
        {
            var chart = Selection.activeGameObject?.GetComponent<RadarChart>();
            if (chart == null)
            {
                EditorUtility.DisplayDialog("エラー", "RadarChart コンポーネントを持つオブジェクトを選択してください。", "OK");
                return;
            }
            Undo.RecordObject(chart, "Apply Radar Chart Settings");
            ApplySettings(chart);
            EditorUtility.SetDirty(chart);
            Debug.Log($"[RadarChart] 設定を反映しました: {chart.gameObject.name}");
        }

        private void ApplySettings(RadarChart chart)
        {
            chart.vertexCount = _vertexCount;
            chart.divisions = _divisions;
            chart.radius = _radius;

            chart.fillMode = _fillMode;
            chart.fillColor = _fillColor;
            chart.fillGradient = CloneGradient(_fillGradient);
            chart.vertexColors = new List<Color>(_vertexColors);
            chart.gradientSteps = _gradientSteps;

            chart.showOutline = _showOutline;
            chart.outlineColor = _outlineColor;
            chart.outlineWidth = _outlineWidth;

            chart.showGrid = _showGrid;
            chart.gridColor = _gridColor;
            chart.gridLineWidth = _gridLineWidth;

            chart.showAxes = _showAxes;
            chart.axisColor = _axisColor;
            chart.axisLineWidth = _axisLineWidth;

            chart.animateChanges = _animateChanges;
            chart.animationSpeed = _animationSpeed;

            chart.values = new List<float>(_values);

            EditorUtility.SetDirty(chart);
        }

        // =====================
        // ユーティリティ
        // =====================
        private Canvas EnsureCanvas()
        {
            var canvas = GameObject.FindAnyObjectByType<Canvas>();
            if (canvas != null) return canvas;

            var canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");
            Debug.Log("[RadarChart] Canvasを自動生成しました");
            return canvas;
        }

        private Gradient CloneGradient(Gradient src)
        {
            var g = new Gradient();
            g.SetKeys(src.colorKeys, src.alphaKeys);
            g.mode = src.mode;
            return g;
        }

        private bool DrawFoldout(string label, bool state)
        {
            var style = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };
            bool result = EditorGUILayout.Foldout(state, " " + label, true, style);
            return result;
        }

        private void DrawSeparator()
        {
            EditorGUILayout.Space(4);
            var rect = EditorGUILayout.GetControlRect(false, 1f);
            EditorGUI.DrawRect(rect, new Color(0.4f, 0.4f, 0.4f, 0.5f));
            EditorGUILayout.Space(4);
        }
    }

    // =====================
    // Gradient をSerializedObjectで扱うためのWrapper
    // =====================
    internal class GradientWrapper : ScriptableObject
    {
        public Gradient gradient = new Gradient();
    }
}
            
            