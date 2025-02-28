using UnityEditor;
using UnityEngine;

public class SpriteProcessor : AssetPostprocessor
{
    private void OnPostprocessTexture(Texture2D texture)
    {
        // ����ļ���չ���Ƿ�Ϊ PNG
        if (assetPath.EndsWith(".png"))
        {
            // ��ȡ��������
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            // ������������Ϊ Sprite
            textureImporter.textureType = TextureImporterType.Sprite;
            // ���þ���ģʽΪ Single
            textureImporter.spriteImportMode = SpriteImportMode.Single;
            // ����͸����
            textureImporter.alphaIsTransparency = true;
            // ���� Mipmap
            textureImporter.mipmapEnabled = false;
        }
    }
}