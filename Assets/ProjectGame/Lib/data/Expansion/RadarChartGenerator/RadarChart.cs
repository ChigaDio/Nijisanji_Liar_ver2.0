
            using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RadarChartGenerator
{
    /// <summary>
    /// レーダーチャート（多角形グラフ）描画コンポーネント
    /// UICanvasのImageコンポーネントと連携して動作します
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class RadarChart : MaskableGraphic
    {
        // =====================
        // 基本設定
        // =====================
        [Header("基本設定")]
        [Tooltip("頂点数 (3〜12)")]
        [Range(3, 12)]
        public int vertexCount = 5;

        [Tooltip("分割数（同心多角形の数）")]
        [Range(1, 10)]
        public int divisions = 5;

        [Tooltip("チャートの半径")]
        public float radius = 180f;

        [Tooltip("各頂点の値 (0.0〜1.0)")]
        public List<float> values = new List<float> { 0.8f, 0.6f, 0.9f, 0.5f, 0.7f };

        // =====================
        // 塗りつぶし設定
        // =====================
        [Header("塗りつぶし設定")]
        public FillMode fillMode = FillMode.SingleColor;

        [Tooltip("単色モード時の塗りつぶし色")]
        public Color fillColor = new Color(0.2f, 0.5f, 1f, 0.4f);

        [Tooltip("グラデーション（外側→内側）")]
        public Gradient fillGradient = new Gradient();

        [Tooltip("各頂点の個別色（VertexColorモード時）")]
        public List<Color> vertexColors = new List<Color>();

        [Tooltip("グラデーション分割精度（高いほど滑らか）")]
        [Range(2, 32)]
        public int gradientSteps = 16;

        // =====================
        // アウトライン設定
        // =====================
        [Header("アウトライン設定")]
        [Tooltip("データポリゴンのアウトライン表示")]
        public bool showOutline = true;

        [Tooltip("アウトラインの色")]
        public Color outlineColor = new Color(0.2f, 0.5f, 1f, 1f);

        [Tooltip("アウトラインの太さ")]
        [Range(0.5f, 8f)]
        public float outlineWidth = 2f;

        // =====================
        // グリッド設定
        // =====================
        [Header("グリッド設定")]
        [Tooltip("グリッド（同心多角形）の表示")]
        public bool showGrid = true;

        [Tooltip("グリッドの色")]
        public Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);

        [Tooltip("グリッド線の太さ")]
        [Range(0.5f, 4f)]
        public float gridLineWidth = 1f;

        // =====================
        // 軸線設定
        // =====================
        [Header("軸線設定")]
        [Tooltip("中心から各頂点への軸線を表示")]
        public bool showAxes = true;

        [Tooltip("軸線の色")]
        public Color axisColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        [Tooltip("軸線の太さ")]
        [Range(0.5f, 4f)]
        public float axisLineWidth = 1f;

        // =====================
        // アニメーション設定
        // =====================
        [Header("アニメーション")]
        [Tooltip("値の変化をアニメーションで補間する")]
        public bool animateChanges = true;

        [Tooltip("アニメーション速度")]
        [Range(0.5f, 10f)]
        public float animationSpeed = 3f;

        // =====================
        // 内部変数
        // =====================
        private List<float> _currentValues = new List<float>();
        private List<float> _targetValues = new List<float>();
        private bool _isAnimating = false;
        private float _startAngle = -Mathf.PI / 2f; // 上から開始

        public enum FillMode
        {
            SingleColor,
            RadialGradient,
            VertexColor
        }

        // =====================
        // 初期化
        // =====================
        protected override void Start()
        {
            base.Start();
            InitializeValues();
            SetVertexDefault();
        }

        private void InitializeValues()
        {
            _currentValues.Clear();
            _targetValues.Clear();
            SyncValueCount();
            for (int i = 0; i < vertexCount; i++)
            {
                float v = (i < values.Count) ? Mathf.Clamp01(values[i]) : 0.5f;
                _currentValues.Add(v);
                _targetValues.Add(v);
            }
        }

        private void SyncValueCount()
        {
            while (values.Count < vertexCount) values.Add(0.5f);
            while (vertexColors.Count < vertexCount) vertexColors.Add(Color.white);
        }

        private void SetVertexDefault()
        {
            if (fillGradient == null || fillGradient.colorKeys.Length == 0)
            {
                var gradient = new Gradient();
                gradient.SetKeys(
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
                fillGradient = gradient;
            }
        }

        // =====================
        // 毎フレーム更新
        // =====================
        private void Update()
        {
            if (!animateChanges || !_isAnimating) return;

            bool stillAnimating = false;
            for (int i = 0; i < _currentValues.Count && i < _targetValues.Count; i++)
            {
                float diff = _targetValues[i] - _currentValues[i];
                if (Mathf.Abs(diff) > 0.001f)
                {
                    _currentValues[i] += diff * Time.deltaTime * animationSpeed;
                    stillAnimating = true;
                }
                else
                {
                    _currentValues[i] = _targetValues[i];
                }
            }
            _isAnimating = stillAnimating;
            SetVerticesDirty();
        }

        // =====================
        // 値の外部設定API
        // =====================
        /// <summary>
        /// 指定インデックスの値をセット（0.0〜1.0）
        /// </summary>
        public void SetValue(int index, float value)
        {
            SyncValueCount();
            if (index < 0 || index >= vertexCount) return;
            value = Mathf.Clamp01(value);
            values[index] = value;

            if (index < _targetValues.Count)
                _targetValues[index] = value;
            else
                while (_targetValues.Count <= index) _targetValues.Add(value);

            if (!animateChanges)
            {
                if (index < _currentValues.Count)
                    _currentValues[index] = value;
                else
                    while (_currentValues.Count <= index) _currentValues.Add(value);
            }
            _isAnimating = true;
            SetVerticesDirty();
        }

        /// <summary>
        /// 全頂点の値を一括セット
        /// </summary>
        public void SetAllValues(List<float> newValues)
        {
            SyncValueCount();
            for (int i = 0; i < vertexCount; i++)
            {
                float v = (i < newValues.Count) ? Mathf.Clamp01(newValues[i]) : 0f;
                values[i] = v;
                if (i < _targetValues.Count) _targetValues[i] = v;
                else _targetValues.Add(v);
                if (!animateChanges)
                {
                    if (i < _currentValues.Count) _currentValues[i] = v;
                    else _currentValues.Add(v);
                }
            }
            _isAnimating = true;
            SetVerticesDirty();
        }

        /// <summary>
        /// 頂点数を動的に変更
        /// </summary>
        public void SetVertexCount(int count)
        {
            count = Mathf.Clamp(count, 3, 12);
            vertexCount = count;
            InitializeValues();
            SetVerticesDirty();
        }

        // =====================
        // 描画メイン
        // =====================
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            SyncValueCount();
            if (_currentValues.Count < vertexCount)
                InitializeValues();

            Vector2 center = Vector2.zero;

            // 描画順序：グリッド → 軸線 → データポリゴン → アウトライン
            if (showGrid) DrawGrid(vh, center);
            if (showAxes) DrawAxes(vh, center);
            DrawDataPolygon(vh, center);
            if (showOutline) DrawOutline(vh, center);
        }

        // =====================
        // グリッド（同心多角形）描画
        // =====================
        private void DrawGrid(VertexHelper vh, Vector2 center)
        {
            for (int d = 1; d <= divisions; d++)
            {
                float r = radius * d / divisions;
                DrawPolygonOutline(vh, center, r, vertexCount, gridColor, gridLineWidth);
            }
        }

        // =====================
        // 軸線描画
        // =====================
        private void DrawAxes(VertexHelper vh, Vector2 center)
        {
            for (int i = 0; i < vertexCount; i++)
            {
                float angle = _startAngle + (2f * Mathf.PI * i / vertexCount);
                Vector2 tip = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                DrawLine(vh, center, tip, axisLineWidth, axisColor);
            }
        }

        // =====================
        // データポリゴン描画
        // =====================
        private void DrawDataPolygon(VertexHelper vh, Vector2 center)
        {
            switch (fillMode)
            {
                case FillMode.SingleColor:
                    DrawPolygonFillSingleColor(vh, center);
                    break;
                case FillMode.RadialGradient:
                    DrawPolygonFillGradient(vh, center);
                    break;
                case FillMode.VertexColor:
                    DrawPolygonFillVertexColor(vh, center);
                    break;
            }
        }

        private void DrawPolygonFillSingleColor(VertexHelper vh, Vector2 center)
        {
            int baseIndex = vh.currentVertCount;
            UIVertex centerVert = UIVertex.simpleVert;
            centerVert.position = center;
            centerVert.color = fillColor;
            vh.AddVert(centerVert);

            for (int i = 0; i < vertexCount; i++)
            {
                float angle = _startAngle + (2f * Mathf.PI * i / vertexCount);
                float val = GetCurrentValue(i);
                Vector2 pos = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius * val;
                UIVertex v = UIVertex.simpleVert;
                v.position = pos;
                v.color = fillColor;
                vh.AddVert(v);
            }
            for (int i = 0; i < vertexCount; i++)
            {
                vh.AddTriangle(baseIndex, baseIndex + 1 + i, baseIndex + 1 + (i + 1) % vertexCount);
            }
        }

        private void DrawPolygonFillGradient(VertexHelper vh, Vector2 center)
        {
            // 放射状グラデーション: 同心リングを重ねて近似
            for (int step = 0; step < gradientSteps; step++)
            {
                float t0 = (float)step / gradientSteps;
                float t1 = (float)(step + 1) / gradientSteps;
                Color c0 = fillGradient.Evaluate(1f - t0); // 内側
                Color c1 = fillGradient.Evaluate(1f - t1); // 外側

                int baseIdx = vh.currentVertCount;
                for (int i = 0; i < vertexCount; i++)
                {
                    float angle = _startAngle + (2f * Mathf.PI * i / vertexCount);
                    float val = GetCurrentValue(i);
                    Vector2 inner = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius * val * t0;
                    Vector2 outer = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius * val * t1;
                    UIVertex vi = UIVertex.simpleVert;
                    vi.position = inner; vi.color = c0; vh.AddVert(vi);
                    UIVertex vo = UIVertex.simpleVert;
                    vo.position = outer; vo.color = c1; vh.AddVert(vo);
                }
                // 中心
                if (step == 0)
                {
                    // 最内リングは中心点から
                    UIVertex vc = UIVertex.simpleVert;
                    vc.position = center;
                    vc.color = fillGradient.Evaluate(1f);
                    vh.AddVert(vc);
                    int centerIdx = vh.currentVertCount - 1;
                    for (int i = 0; i < vertexCount; i++)
                    {
                        int cur = baseIdx + i * 2;
                        int nxt = baseIdx + ((i + 1) % vertexCount) * 2;
                        vh.AddTriangle(centerIdx, cur, nxt);
                    }
                }
                else
                {
                    for (int i = 0; i < vertexCount; i++)
                    {
                        int cur0 = baseIdx + i * 2;
                        int cur1 = baseIdx + i * 2 + 1;
                        int nxt0 = baseIdx + ((i + 1) % vertexCount) * 2;
                        int nxt1 = baseIdx + ((i + 1) % vertexCount) * 2 + 1;
                        vh.AddTriangle(cur0, cur1, nxt0);
                        vh.AddTriangle(cur1, nxt1, nxt0);
                    }
                }
            }
            // 最外リング
            {
                int baseIdx = vh.currentVertCount;
                Color outerColor = fillGradient.Evaluate(0f);
                for (int i = 0; i < vertexCount; i++)
                {
                    float angle = _startAngle + (2f * Mathf.PI * i / vertexCount);
                    float val = GetCurrentValue(i);
                    float t0 = (float)(gradientSteps - 1) / gradientSteps;
                    Vector2 inner = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius * val * t0;
                    Vector2 outer = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius * val;
                    Color c0 = fillGradient.Evaluate(1f - t0);
                    UIVertex vi = UIVertex.simpleVert;
                    vi.position = inner; vi.color = c0; vh.AddVert(vi);
                    UIVertex vo = UIVertex.simpleVert;
                    vo.position = outer; vo.color = outerColor; vh.AddVert(vo);
                }
                for (int i = 0; i < vertexCount; i++)
                {
                    int cur0 = baseIdx + i * 2;
                    int cur1 = baseIdx + i * 2 + 1;
                    int nxt0 = baseIdx + ((i + 1) % vertexCount) * 2;
                    int nxt1 = baseIdx + ((i + 1) % vertexCount) * 2 + 1;
                    vh.AddTriangle(cur0, cur1, nxt0);
                    vh.AddTriangle(cur1, nxt1, nxt0);
                }
            }
        }

        private void DrawPolygonFillVertexColor(VertexHelper vh, Vector2 center)
        {
            // 各頂点に指定色、中心はブレンド色
            Color centerColor = Color.black;
            centerColor.a = 0f;
            for (int i = 0; i < vertexCount; i++)
            {
                Color vc = (i < vertexColors.Count) ? vertexColors[i] : fillColor;
                centerColor += vc;
            }
            centerColor /= vertexCount;

            int baseIndex = vh.currentVertCount;
            UIVertex cv = UIVertex.simpleVert;
            cv.position = center;
            cv.color = centerColor;
            vh.AddVert(cv);

            for (int i = 0; i < vertexCount; i++)
            {
                float angle = _startAngle + (2f * Mathf.PI * i / vertexCount);
                float val = GetCurrentValue(i);
                Vector2 pos = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius * val;
                Color vc = (i < vertexColors.Count) ? vertexColors[i] : fillColor;
                UIVertex v = UIVertex.simpleVert;
                v.position = pos;
                v.color = vc;
                vh.AddVert(v);
            }
            for (int i = 0; i < vertexCount; i++)
            {
                vh.AddTriangle(baseIndex, baseIndex + 1 + i, baseIndex + 1 + (i + 1) % vertexCount);
            }
        }

        // =====================
        // アウトライン描画
        // =====================
        private void DrawOutline(VertexHelper vh, Vector2 center)
        {
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i < vertexCount; i++)
            {
                float angle = _startAngle + (2f * Mathf.PI * i / vertexCount);
                float val = GetCurrentValue(i);
                points.Add(center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius * val);
            }
            for (int i = 0; i < points.Count; i++)
            {
                DrawLine(vh, points[i], points[(i + 1) % points.Count], outlineWidth, outlineColor);
            }
        }

        // =====================
        // 汎用：多角形アウトライン描画
        // =====================
        private void DrawPolygonOutline(VertexHelper vh, Vector2 center, float r, int sides, Color col, float lineWidth)
        {
            List<Vector2> pts = new List<Vector2>();
            for (int i = 0; i < sides; i++)
            {
                float angle = _startAngle + (2f * Mathf.PI * i / sides);
                pts.Add(center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r);
            }
            for (int i = 0; i < pts.Count; i++)
            {
                DrawLine(vh, pts[i], pts[(i + 1) % pts.Count], lineWidth, col);
            }
        }

        // =====================
        // 汎用：太さのある線描画
        // =====================
        private void DrawLine(VertexHelper vh, Vector2 a, Vector2 b, float width, Color col)
        {
            Vector2 dir = (b - a).normalized;
            Vector2 perp = new Vector2(-dir.y, dir.x) * (width * 0.5f);

            int idx = vh.currentVertCount;
            UIVertex v0 = UIVertex.simpleVert; v0.position = a - perp; v0.color = col; vh.AddVert(v0);
            UIVertex v1 = UIVertex.simpleVert; v1.position = a + perp; v1.color = col; vh.AddVert(v1);
            UIVertex v2 = UIVertex.simpleVert; v2.position = b + perp; v2.color = col; vh.AddVert(v2);
            UIVertex v3 = UIVertex.simpleVert; v3.position = b - perp; v3.color = col; vh.AddVert(v3);

            vh.AddTriangle(idx, idx + 1, idx + 2);
            vh.AddTriangle(idx, idx + 2, idx + 3);
        }

        // =====================
        // ヘルパー
        // =====================
        private float GetCurrentValue(int index)
        {
            if (index < _currentValues.Count) return _currentValues[index];
            if (index < values.Count) return Mathf.Clamp01(values[index]);
            return 0f;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            SyncValueCount();
            InitializeValues();
            SetVerticesDirty();
        }
#endif
    }
}
            