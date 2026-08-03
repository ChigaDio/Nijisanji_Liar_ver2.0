
            using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RadarChartGenerator
{
    /// <summary>
    /// ゲーム実行中にレーダーチャートを動的に操作するコントローラー。
    /// RadarChart コンポーネントと同じ GameObject にアタッチしてください。
    /// </summary>
    [RequireComponent(typeof(RadarChart))]
    public class RadarChartController : MonoBehaviour
    {
        private RadarChart _chart;

        // =====================
        // デモ用設定
        // =====================
        [Header("デモ設定")]
        [Tooltip("起動時にデモアニメーションを再生する")]
        public bool playDemoOnStart = true;

        [Tooltip("デモのループ再生")]
        public bool loopDemo = true;

        [Tooltip("デモの各ステップ間隔（秒）")]
        public float demoInterval = 2f;

        [Header("プリセットデータ")]
        public List<ChartPreset> presets = new List<ChartPreset>
        {
            new ChartPreset { name = "バランス型",  values = new List<float>{ 0.7f,0.7f,0.7f,0.7f,0.7f } },
            new ChartPreset { name = "アタッカー",  values = new List<float>{ 1.0f,0.3f,0.8f,0.6f,0.4f } },
            new ChartPreset { name = "タンク",      values = new List<float>{ 0.4f,1.0f,0.3f,0.2f,0.9f } },
            new ChartPreset { name = "マジシャン",  values = new List<float>{ 0.3f,0.4f,0.5f,1.0f,0.6f } },
            new ChartPreset { name = "スピードスター",values= new List<float>{ 0.6f,0.4f,1.0f,0.5f,0.5f } },
        };

        // =====================
        // 初期化
        // =====================
        private void Start()
        {
            _chart = GetComponent<RadarChart>();
            if (playDemoOnStart)
                StartCoroutine(DemoCoroutine());
        }

        // =====================
        // デモコルーチン
        // =====================
        private IEnumerator DemoCoroutine()
        {
            int presetIndex = 0;
            do
            {
                foreach (var preset in presets)
                {
                    ApplyPreset(preset);
                    yield return new WaitForSeconds(demoInterval);
                    presetIndex++;
                }
            } while (loopDemo);
        }

        // =====================
        // 公開API
        // =====================
        /// <summary>プリセットを名前で適用</summary>
        public void ApplyPresetByName(string presetName)
        {
            var preset = presets.Find(p => p.name == presetName);
            if (preset != null) ApplyPreset(preset);
            else Debug.LogWarning($"[RadarChartController] プリセット '{presetName}' が見つかりません");
        }

        /// <summary>プリセットをインデックスで適用</summary>
        public void ApplyPresetByIndex(int index)
        {
            if (index >= 0 && index < presets.Count) ApplyPreset(presets[index]);
        }

        /// <summary>プリセットを適用</summary>
        public void ApplyPreset(ChartPreset preset)
        {
            if (_chart == null) return;
            _chart.SetAllValues(preset.values);
        }

        /// <summary>個別頂点の値を設定（0.0〜1.0）</summary>
        public void SetValue(int index, float value)
        {
            _chart?.SetValue(index, value);
        }

        /// <summary>全頂点の値をランダムに設定</summary>
        public void RandomizeValues()
        {
            if (_chart == null) return;
            var vals = new List<float>();
            for (int i = 0; i < _chart.vertexCount; i++)
                vals.Add(Random.Range(0.2f, 1f));
            _chart.SetAllValues(vals);
        }

        /// <summary>頂点数を変更（3〜12）</summary>
        public void SetVertexCount(int count)
        {
            _chart?.SetVertexCount(count);
        }

        /// <summary>全値を指定値に設定（0.0〜1.0）</summary>
        public void SetAllToValue(float value)
        {
            if (_chart == null) return;
            var vals = new List<float>();
            for (int i = 0; i < _chart.vertexCount; i++) vals.Add(value);
            _chart.SetAllValues(vals);
        }

        /// <summary>塗りつぶしモードを切り替え</summary>
        public void CycleFillMode()
        {
            if (_chart == null) return;
            int next = ((int)_chart.fillMode + 1) % 3;
            _chart.fillMode = (RadarChart.FillMode)next;
            _chart.SetVerticesDirty();
        }
    }

    // =====================
    // プリセットデータクラス
    // =====================
    [System.Serializable]
    public class ChartPreset
    {
        public string name;
        public List<float> values = new List<float>();
    }
}
            