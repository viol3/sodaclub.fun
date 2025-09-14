using UnityEditor;
using UnityEngine;

namespace Hyper.TextureAtlasGenerator
{
    public class TextureAtlas
    {
        public Texture2D AtlasImage { get; private set; }

        public TextureAtlas(int width, int height)
        {
            AtlasImage = new Texture2D(width, height);
            ClearAtlas();
        }

        private void ClearAtlas()
        {
            Color[] transparentPixels = new Color[AtlasImage.width * AtlasImage.height];
            for (int i = 0; i < transparentPixels.Length; i++)
            {
                transparentPixels[i] = Color.clear;
            }
            AtlasImage.SetPixels(transparentPixels);
            AtlasImage.Apply();
        }

        public void AddTexture(Texture2D texture, int x, int y)
        {
            string texturePath = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(texturePath);
            if (textureImporter != null)
            {
                textureImporter.isReadable = true;
                AssetDatabase.ImportAsset(texturePath);
            }
            Color[] pixels = texture.GetPixels();
            AtlasImage.SetPixels(x, y, texture.width, texture.height, pixels);
            AtlasImage.Apply(); // Apply changes to the texture
        }

        public void Save(string filename)
        {
            byte[] bytes = AtlasImage.EncodeToPNG();
            System.IO.File.WriteAllBytes(filename, bytes);
        }
    }
}
