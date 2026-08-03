using UnityEngine;

namespace GameCore.MaterialData
{
    /// <summary>
    /// Material CS生成機能によって生成される各クラスの共通基底クラス
    /// </summary>
    [System.Serializable]
    public abstract class BaseMaterialData 
    {
        [SerializeField] protected Renderer targetRenderer;
        // MaterialPropertyBlock用（メモリを汚さず、マテリアルを複製しない最高効率の方式）
        protected MaterialPropertyBlock propertyBlock;

        // マテリアル自体のパラメータを直接変える必要がある場合にキャッシュする変数
        protected Material cachedMaterial;

        public virtual void Awake()
        {
            if (targetRenderer == null) return;
            propertyBlock = new MaterialPropertyBlock();

            // 【注意】もしマテリアル自体のシェーダーキーワード切り替えなどが必要な場合のみ、
            // インスタンスをキャッシュして使い回します（毎フレームの .material 呼び出しは絶対NG）
            // cachedMaterial = targetRenderer.material;
        }
    }
}