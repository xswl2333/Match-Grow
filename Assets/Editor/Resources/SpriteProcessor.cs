using UnityEditor;
using UnityEngine;

public class SpriteProcessor : AssetPostprocessor
{
    private void OnPostprocessTexture(Texture2D texture)
    {
        // 检查文件扩展名是否为 PNG
        if (assetPath.EndsWith(".png"))
        {
            // 获取纹理导入器
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            // 设置纹理类型为 Sprite
            textureImporter.textureType = TextureImporterType.Sprite;
            // 设置精灵模式为 Single
            textureImporter.spriteImportMode = SpriteImportMode.Single;
            // 启用透明度
            textureImporter.alphaIsTransparency = true;
            // 禁用 Mipmap
            textureImporter.mipmapEnabled = false;
        }
    }
}