using UnityEngine;

[ExecuteAlways] // Updates in Editor too
public class TextureSizeUpdater : MonoBehaviour
{
    public string propertyName = "_MainTex_TexelSize"; // Must match Shader Graph property name
    public string texelSizeYPropertyName = "_TexelSizeY"; // New property name for Y texel size

    void Update()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.sharedMaterial != null)
        {
            UpdateTextureSize(renderer.sharedMaterial);
            UpdateTexelSizeY(renderer.sharedMaterial);
        }
    }

    void UpdateTextureSize(Material material)
    {
        Texture texture = material.mainTexture;
        if (texture != null)
        {
            Vector2 size = new Vector2(texture.width, texture.height);
            material.SetVector(propertyName, size);
        }
    }

    void UpdateTexelSizeY(Material material)
    {
        Texture texture = material.mainTexture;
        if (texture != null)
        {
            float height = texture.height;
            Debug.Log("texture size height = " + height);
            material.SetFloat(texelSizeYPropertyName, height);
        }
    }
}
