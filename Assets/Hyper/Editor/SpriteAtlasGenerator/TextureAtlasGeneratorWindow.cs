using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;


//-----------------------------------------------------------------------
// <copyright file="TextureAtlasGeneratorWindow.cs" company="Bermuda Games">
//     Author: Bermuda Games Oyun Yazilim ve Pazarlama A.S.
//     Copyright (c) Bermuda Games. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

/** 
 * Copyright (C) Bermuda Games - All Rights Reserved
 *
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Bermuda Games, Ali AVCI info@bermuda.gs, 01/07/2023
 */

namespace Hyper.TextureAtlasGenerator
{
    public class TextureAtlasGeneratorWindow : EditorWindow
    {
        private List<TextureEntry> _textures = new List<TextureEntry>();
        private Vector2 _scrollPosition;
        private int _atlasSize = 1024;
        private float _targetSize = 50;
        private float _widthGap = 20;
        private string[] _targetFormats = { ".png", /*".jpg", ".jpeg"*/ };

        [MenuItem("Window/Hyper/Texture Packer")]
        public static void ShowWindow()
        {
            var window = GetWindow<TextureAtlasGeneratorWindow>("Texture Packer");
            window.RefreshTextures();
        }

        private void OnGUI()
        {
            Header();
            EditorUtility.HorizontalLine();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DisplayTextureList();
            EditorGUILayout.EndScrollView();
        }
        private void Header()
        {
            if (EditorUtility.InLineButtonHorizontal("Refresh", () => EditorUtility.HeaderLabel("Sprite Packer by Hyper.gs", false), false, GUILayout.Width(60), GUILayout.Height(20)))
            {
                RefreshTextures();
            }

            EditorUtility.HorizontalLine(2, Color.gray);
            EditorGUILayout.LabelField("Pack single seperated sprites to one sprite atlas to reduce game build size and memory usage.", Hyper.EditorUtility.BoldLabel(TextAnchor.MiddleCenter, 11, Color.white));
            EditorUtility.HorizontalLine(2, Color.gray);

            if (EditorUtility.InLineButtonHorizontalWithCondition("Generate", () =>
            {
                _atlasSize = EditorGUILayout.IntField("Atlas Size", _atlasSize, GUILayout.Width(200));
            }, GetSelectedTextures().Count == 0, false))
            {
                GenerateAtlas();
            }
        }

        private void GenerateAtlas()
        {
            if (EditorUtility.AskForFilePanel("Save Atlas", "Assets/", "Atlas", "png") is string path && !string.IsNullOrEmpty(path))
            {
                TextureAtlasCreator.CreateTextureAtlas(GetSelectedTextures(), _atlasSize, _atlasSize, path);
            }
        }
        private void DisplayTextureList()
        {
            GUILayout.BeginVertical();
            _textures.ForEach(entry => EditorUtility.DisabledGroup(() => entry.Display(_targetSize, position.width - _widthGap), entry.Texture.width > _atlasSize || entry.Texture.height > _atlasSize));
            _textures.ForEach(entry =>
            {
                if (entry.Texture.width > _atlasSize || entry.Texture.height > _atlasSize)
                    entry.IsSelected = false;
            });
            GUILayout.EndVertical();
        }
        private void RefreshTextures()
        {
            GetTexturesInScene();
            _textures.Sort((a, b) => (b.Texture.width * b.Texture.height).CompareTo(a.Texture.width * a.Texture.height));
            _textures.Reverse();
        }
        private void GetTexturesInScene()
        {
            _textures.Clear();
            Dictionary<string, int> texturePathOccurrences = new Dictionary<string, int>();

            SpriteRenderer[] spriteRenderers = FindObjectsOfType<SpriteRenderer>();

            foreach (var spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer.sprite != null)
                {
                    Texture2D texture = spriteRenderer.sprite.texture;
                    if (TextureInTargetFormat(texture, _targetFormats) && !TextureNameContainsKeyword(texture.name))
                    {
                        AddTextureToList(texture, texturePathOccurrences);
                    }
                }
            }

            Image[] images = FindObjectsOfType<Image>();

            foreach (var image in images)
            {
                if (image.sprite != null)
                {
                    Texture2D texture = image.sprite.texture;
                    if (TextureInTargetFormat(texture, _targetFormats) && !TextureNameContainsKeyword(texture.name))
                    {
                        AddTextureToList(texture, texturePathOccurrences);
                    }
                }
            }
        }
        private void GetTexturesInProject()
        {
            _textures.Clear();

            string[] assetPaths = AssetDatabase.FindAssets("t:Texture2D")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => TextureInsideAssetsFolder(path))
                .ToArray();

            foreach (var assetPath in assetPaths)
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                if (texture != null && TextureInTargetFormat(texture, _targetFormats) && !TextureNameContainsKeyword(texture.name))
                {
                    _textures.Add(new TextureEntry { Texture = texture, IsSelected = false });
                }
            }
        }

        private void AddTextureToList(Texture2D texture, Dictionary<string, int> texturePathOccurrences)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            if (!texturePathOccurrences.ContainsKey(assetPath))
            {
                texturePathOccurrences[assetPath] = 1;
                _textures.Add(new TextureEntry { Texture = texture, IsSelected = false });
            }
            else
            {
                texturePathOccurrences[assetPath]++;
            }
        }
        #region Utility
        private bool TextureInTargetFormat(Texture2D texture, params string[] format)
        {
            if (texture == null)
            {
                return false;
            }
            string assetPath = AssetDatabase.GetAssetPath(texture);
            string extension = Path.GetExtension(assetPath);
            return format.Contains(extension);
        }
        private bool TextureNameContainsKeyword(string textureName, string keyword = "atlas")
        {
            string lowerTextureName = textureName.ToLower();
            return lowerTextureName.Contains(keyword) &&
                   (lowerTextureName.IndexOf(keyword) == 0 || !char.IsLetter(lowerTextureName[lowerTextureName.IndexOf(keyword) - 1])) &&
                   (lowerTextureName.IndexOf(keyword) == lowerTextureName.Length - 5 || !char.IsLetter(lowerTextureName[lowerTextureName.IndexOf(keyword) + 5]));
        }
        private bool TextureInsideAssetsFolder(string assetPath) => assetPath.StartsWith("Assets/") && !assetPath.StartsWith("Assets/Plugins/");
        private List<Texture2D> GetSelectedTextures() => _textures.Where(entry => entry.IsSelected).Select(entry => entry.Texture).ToList();

        #endregion
    }
}
