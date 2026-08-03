using UnityEngine;
using GameCore.MaterialData;

/// <summary>
/// 円形のマスク
/// </summary>
[System.Serializable]
public class CircleAlphaMask : BaseMaterialData
{

    private static readonly int ColorPropertyId = Shader.PropertyToID("_Color");
    private static readonly int CenterPropertyId = Shader.PropertyToID("_Center");
    private static readonly int RadiusPropertyId = Shader.PropertyToID("_Radius");
    private static readonly int StrengthPropertyId = Shader.PropertyToID("_Strength");



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
    /// _Center (Vector) をMaterialPropertyBlock経由で効率的に変更する
    /// メモリリークせず、バッチング（描画最適化）も維持されます
    /// </summary>
    public void SetCenterEfficiently(Vector4 newValue)
    {
        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetVector(CenterPropertyId, newValue);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }
    /// <summary>
    /// _Radius (Float) をMaterialPropertyBlock経由で効率的に変更する
    /// メモリリークせず、バッチング（描画最適化）も維持されます
    /// </summary>
    public void SetRadiusEfficiently(float newValue)
    {
        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(RadiusPropertyId, newValue);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }
    /// <summary>
    /// _Strength (Float) をMaterialPropertyBlock経由で効率的に変更する
    /// メモリリークせず、バッチング（描画最適化）も維持されます
    /// </summary>
    public void SetStrengthEfficiently(float newValue)
    {
        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(StrengthPropertyId, newValue);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }

}
