using UnityEngine;
using UnityEditor;
using System;

//-----------------------------------------------------------------------
// <copyright file="EditorUtility.cs" company="Bermuda Games">
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

namespace Hyper
{
    public static class EditorUtility
    {
        public static Color LineColor = new Color(0f, 0f, 0f, 0.1f);
        public static GUIStyle BoldLabel(TextAnchor alignment = TextAnchor.MiddleCenter, int fontSize = 12, Color color = default)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = alignment,
                fontSize = fontSize,
            };
            if (color == default)
            {
                color = Color.white;
            }
            style.normal.textColor = color;
            return style;
        }
        public static GUIStyle TextStyle(TextAnchor alignment = TextAnchor.UpperLeft
        , FontStyle fontStyle = FontStyle.Normal, int fontSize = 12)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                fontStyle = fontStyle,
                alignment = alignment,
                fontSize = fontSize,
                richText = true,
            };
            return style;
        }
        public static Texture2D GetTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.alphaIsTransparency = true;
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
        public static void DisabledGroup(Action group, bool condition = true)
        {
            EditorGUI.BeginDisabledGroup(condition);
            group?.Invoke();
            EditorGUI.EndDisabledGroup();
        }
        public static void Bar(Rect rect, float backgroundWidth, float filledWidth, float height, Color color, Color backgroundColor = default)
        {
            if (backgroundColor == default)
            {
                backgroundColor = Color.white;
            }

            // Draw the health bar background with the specified width.
            Rect backgroundRect = new Rect(rect.x, rect.y, backgroundWidth, height);
            EditorGUI.DrawRect(backgroundRect, backgroundColor);

            // Ensure the filled width does not exceed the background width.
            filledWidth = Mathf.Clamp(filledWidth, 0f, backgroundWidth);

            // Draw the filled part of the health bar with the calculated color.
            Rect filledRect = new Rect(rect.x, rect.y, filledWidth, height);
            EditorGUI.DrawRect(filledRect, color);
        }
        public static bool Button(string text = "", params GUILayoutOption[] styles)
        {
            return GUILayout.Button(text, styles);
        }
        public static void CoverInVerticalWindow(Action content)
        {
            EditorGUILayout.BeginVertical("window");
            content?.Invoke();
            EditorGUILayout.EndVertical();
        }
        public static void CoverInHorizontalWindow(Action content)
        {
            EditorGUILayout.BeginVertical("window");
            content?.Invoke();
            EditorGUILayout.EndVertical();
        }
        public static void CoverInVerticalBox(Action content)
        {
            EditorGUILayout.BeginVertical("box");
            content?.Invoke();
            EditorGUILayout.EndVertical();
        }
        public static void CoverInHorizontalBox(Action content)
        {
            EditorGUILayout.BeginHorizontal("box");
            content?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        public static bool AskUserDialog(string header, string text, string accepted = "Yes", string declined = "No") => UnityEditor.EditorUtility.DisplayDialog(header, text, accepted, declined);
        public static string AskForFilePanel(string title, string directory, string defaultName, string extension)
        {
            return UnityEditor.EditorUtility.SaveFilePanel(title, directory, defaultName, extension);
        }
        public static void HeaderLabel(string text, bool useLine = true)
        {
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 12 };
            EditorGUILayout.LabelField(text, style);
            if (useLine)
                HorizontalLine();
        }
        public static void Label(string label, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null) style = new GUIStyle(GUI.skin.label);
            GUILayout.Label(label, style, options);
        }
        public static void HorizontalLine(float height = 1f, Color color = default)
        {
            if (color == default) color = LineColor;
            GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(height));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), GetTexture(color));
        }
        public static void VerticalLine(float width = 1f, Color color = default)
        {
            if (color == default) color = LineColor;
            GUILayout.Box(GUIContent.none, GUILayout.Width(width), GUILayout.ExpandHeight(true));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), GetTexture(color));
        }
        public static bool InLineButtonHorizontal(string label, Action inLine, bool isLeft = false, params GUILayoutOption[] layoutOptions)
        {
            EditorGUILayout.BeginHorizontal();
            if (!isLeft)
                inLine?.Invoke();

            bool isPressed = Button(label, layoutOptions);

            if (isLeft)
                inLine?.Invoke();

            EditorGUILayout.EndHorizontal();
            return isPressed;
        }
        public static bool InLineButtonHorizontalWithCondition(string label, Action inLine, bool condition, bool isLeft = false, params GUILayoutOption[] layoutOptions)
        {
            EditorGUILayout.BeginHorizontal();
            if (!isLeft)
                inLine?.Invoke();

            bool isPressed = false;
            DisabledGroup(() => isPressed = Button(label, layoutOptions), condition);


            if (isLeft)
                inLine?.Invoke();

            EditorGUILayout.EndHorizontal();
            return isPressed;
        }
        public static bool InLineButtonVerticall(string label, Action inLine, bool isLeft = false, params GUILayoutOption[] layoutOptions)
        {
            EditorGUILayout.BeginVertical();
            if (!isLeft)
                inLine?.Invoke();

            bool isPressed = Button(label, layoutOptions);

            if (isLeft)
                inLine?.Invoke();

            EditorGUILayout.EndVertical();
            return isPressed;
        }
    }
}
