using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom default settings for texture imports
/// </summary>
public class FesTexturePreProcessor : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        if (!assetPath.Contains("FESInternalLogo"))
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.convertToNormalmap = false;
            textureImporter.anisoLevel = 0;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.spritePixelsPerUnit = 1;
            textureImporter.maxTextureSize = 8192;
            textureImporter.isReadable = true;
        }
    }
}
