using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Hyper.Checkup
{
    public static class TextureUtility
    {
        private static string[] _targetFormats = { ".png", ".jpg", ".jpeg" };
        public static void GetTexturesInScene(ref List<TextureEntry> textureEntries)
        {
            textureEntries.Clear();

            // Find SpriteRenderer components
            SpriteRenderer[] spriteRenderers = GameObject.FindObjectsOfType<SpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer.sprite != null)
                {
                    Texture2D texture = spriteRenderer.sprite.texture;
                    if (TextureInTargetFormat(texture, _targetFormats) && !TextureNameContainsKeyword(texture.name))
                    {
                        AddTextureToList(ref textureEntries, texture);
                    }
                }
            }

            // Find Image components
            Image[] images = GameObject.FindObjectsOfType<Image>();
            foreach (var image in images)
            {
                if (image.sprite != null)
                {
                    Texture2D texture = image.sprite.texture;
                    if (TextureInTargetFormat(texture, _targetFormats) && !TextureNameContainsKeyword(texture.name))
                    {
                        AddTextureToList(ref textureEntries, texture);
                    }
                }
            }

            // Find materials on all renderers
            Renderer[] allRenderers = GameObject.FindObjectsOfType<Renderer>();
            foreach (var renderer in allRenderers)
            {
                Material[] materials = renderer.sharedMaterials;
                foreach (var material in materials)
                {
                    if (material != null)
                    {
                        Texture2D mainTexture = material.mainTexture as Texture2D;
                        if (mainTexture != null && TextureInTargetFormat(mainTexture, _targetFormats) && !TextureNameContainsKeyword(mainTexture.name))
                        {
                            AddTextureToList(ref textureEntries, mainTexture);
                        }
                    }
                }
            }
        }

        private static void AddTextureToList(ref List<TextureEntry> textureEntries, Texture2D texture)
        {
            if(textureEntries.Contains(textureEntries.Find(x => x.Texture == texture)))
            {
                return;
            }
            textureEntries.Add(new TextureEntry { Texture = texture, IsSelected = false });
        }
        #region Utility
        public static bool TextureInTargetFormat(Texture2D texture, params string[] format)
        {
            if (texture == null)
            {
                return false;
            }
            string assetPath = AssetDatabase.GetAssetPath(texture);
            string extension = Path.GetExtension(assetPath);
            return format.Contains(extension);
        }
        public static bool TextureNameContainsKeyword(string textureName, string keyword = "atlas")
        {
            string lowerTextureName = textureName.ToLower();
            return lowerTextureName.Contains(keyword) &&
                   (lowerTextureName.IndexOf(keyword) == 0 || !char.IsLetter(lowerTextureName[lowerTextureName.IndexOf(keyword) - 1])) &&
                   (lowerTextureName.IndexOf(keyword) == lowerTextureName.Length - 5 || !char.IsLetter(lowerTextureName[lowerTextureName.IndexOf(keyword) + 5]));
        }
        public static bool IsPowerOfTwo(Texture2D texture)
        {
            return IsPowerOfTwo(texture.width) && IsPowerOfTwo(texture.height);
        }
        public static bool IsPowerOfTwo(int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }
        #endregion
    }
}
