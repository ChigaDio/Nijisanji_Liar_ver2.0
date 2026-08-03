using UnityEngine;
using GameCore.MaterialData;

/// <summary>
/// キャラクター
/// </summary>
[System.Serializable]
public class Character : BaseMaterialData
{

    private static readonly int Texture2DPropertyId = Shader.PropertyToID("_Texture2D");
    private static readonly int ColorPropertyId = Shader.PropertyToID("_Color");
    private static readonly int SwitchPropertyId = Shader.PropertyToID("_Switch");



    /// <summary>
    /// _Texture2D (Texture) をMaterialPropertyBlock経由で効率的に変更する
    /// メモリリークせず、バッチング（描画最適化）も維持されます
    /// </summary>
    public void SetTexture2DEfficiently(Texture newValue)
    {
        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetTexture(Texture2DPropertyId, newValue);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }
    /// <summary>
    /// _Color (Color) をMaterialPropertyBlock経由で効率的に変更する
    /// メモリリークせず、バッチング（描画最適化）も維持されます
    /// </summary>
    public void SetColorEfficiently(Color newValue)
    {
        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(ColorPropertyId, newValue);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }
    /// <summary>
    /// _Switch (Float) をMaterialPropertyBlock経由で効率的に変更する
    /// メモリリークせず、バッチング（描画最適化）も維持されます
    /// </summary>
    public void SetSwitchEfficiently(float newValue)
    {
        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(SwitchPropertyId, newValue);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }

}
