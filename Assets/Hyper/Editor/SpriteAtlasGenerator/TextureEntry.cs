using System;
using UnityEditor;
using UnityEngine;

namespace Hyper.TextureAtlasGenerator
{
    public class TextureEntry
    {
        public Texture2D Texture { get; set; }
        public bool IsSelected { get; set; }
        public string Path => AssetDatabase.GetAssetPath(Texture);
        public bool IsEmpty => Texture == null;
        public void Display(float targetSize, float totalWidth)
        {
            if (IsEmpty)
            {
                return;
            }
            Texture2D texture = Texture;

            float textureWidth = texture.width;
            float textureHeight = texture.height;
            float aspectRatio = (float)textureWidth / textureHeight;

            float targetWidth = targetSize;
            float targetHeight = targetSize;

            Body(totalWidth, targetSize, () =>
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(5);
                DrawTexture(targetWidth, targetHeight);
                EditorUtility.CoverInVerticalBox(() =>
                {
                    EditorUtility.Label(texture.name,EditorUtility.BoldLabel(TextAnchor.MiddleLeft, 13, Color.white));
                    EditorUtility.HorizontalLine();
                    EditorUtility.Label("Texture Size: " + texture.width + "x" + texture.height);
                });
                IsSelected = EditorGUILayout.Toggle(IsSelected, GUILayout.Width(30), GUILayout.Height(targetHeight));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);
            });
        }
        private void Body(float width, float height, Action content = null)
        {
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(width), GUILayout.Height(height));
            content?.Invoke();
            GUILayout.EndVertical();
        }
        private void DrawTexture(float targetWidth, float targetHeight)
        {
            GUILayout.Box(GUIContent.none, GUILayout.Width(targetWidth), GUILayout.Height(targetHeight));
            var boxRect = GUILayoutUtility.GetLastRect();
            GUI.DrawTexture(boxRect, Texture, ScaleMode.ScaleToFit);
            if (boxRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
            {
                EditorGUIUtility.PingObject(Texture);
            }
        }
    }
}
