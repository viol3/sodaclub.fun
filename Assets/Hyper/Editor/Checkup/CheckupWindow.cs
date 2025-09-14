using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//-----------------------------------------------------------------------
// <copyright file="CheckupWindow.cs" company="Bermuda Games">
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

namespace Hyper.Checkup
{
    public class CheckupWindow : EditorWindow
    {
        private List<Rule> _textureRules = new List<Rule>();
        private List<Rule> _meshRules = new List<Rule>();
        private List<TextureEntry> _textures = new List<TextureEntry>();
        private List<MeshEntry> _meshes = new List<MeshEntry>();
        private float _texturesPercentage = 1.0f;
        private float _meshesPercentage = 1.0f;
        private bool _isResetting;

        [MenuItem("Window/Hyper/Checkup Tool")]
        public static void ShowWindow()
        {
            var window = GetWindow<CheckupWindow>();
            window.titleContent = new GUIContent("Checkup");
            window.GetTexturesInScene();
            window.GetMeshesInScene();
            window.SetRules();
            window.Show();
        }
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                ResetWindow();
            }
        }
        private void ResetWindow()
        {
            _isResetting = true;
            GetTexturesInScene();
            GetMeshesInScene();
            SetRules();
            Repaint();
            _isResetting = false;
        }
        private void GetTexturesInScene()
        {
            _textures.Clear();
            TextureUtility.GetTexturesInScene(ref _textures);
        }
        private void GetMeshesInScene()
        {
            _meshes.Clear();
            MeshUtility.GetMeshesInScene(ref _meshes);
        }
        private void SetRules()
        {
            _textureRules.Clear();
            _meshRules.Clear();
            _textureRules.Add(RulesManager.TextureRule1(_textures));
            _textureRules.Add(RulesManager.TextureRule2(_textures));
            _textureRules.Add(RulesManager.TextureRule3(_textures));
            _meshRules.Add(RulesManager.MeshRule1(_meshes));
        }
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Check Up Tool by Hyper.gs", EditorUtility.BoldLabel(TextAnchor.MiddleCenter, 12, Color.white));
            EditorUtility.HorizontalLine(2, Color.gray);
            EditorGUILayout.LabelField("Try to do all bars %100 or close to %100. More closer to 100, more optimized game.", Hyper.EditorUtility.BoldLabel(TextAnchor.MiddleCenter, 11, Color.white));
            EditorUtility.HorizontalLine(2, Color.gray);
            CheckButton();
            while (_isResetting)
            {
                EditorGUILayout.HelpBox("Loading...", MessageType.Info);
                return;
            }
            EditorUtility.CoverInVerticalBox(() =>
            {
                TexturesPanel();
            });
            EditorUtility.CoverInVerticalBox(() =>
            {
                MeshesPanel();
            });
        }
        private void CheckButton()
        {
            if (GUILayout.Button("Check Now", GUILayout.Height(30)))
            {
                ResetWindow();
            }
        }
        private void TexturesPanel()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Textures", EditorUtility.BoldLabel(TextAnchor.MiddleLeft, 13, Color.white));
            EditorGUILayout.LabelField("Total textures: " + _textures.Count, EditorUtility.BoldLabel(TextAnchor.MiddleRight, 13, Color.white));
            EditorGUILayout.EndHorizontal();
            EditorUtility.HorizontalLine(2, Color.gray);
            DisplayPercentageBar(_texturesPercentage);
            DisplayTextureRules();
            TextureCalculatePercentageForRules();
        }
        private void MeshesPanel()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Meshes", EditorUtility.BoldLabel(TextAnchor.MiddleLeft, 13, Color.white));
            EditorGUILayout.LabelField("Total Meshes: " + _meshes.Count, EditorUtility.BoldLabel(TextAnchor.MiddleRight, 13, Color.white));
            EditorGUILayout.EndHorizontal();
            EditorUtility.HorizontalLine(2, Color.gray);
            DisplayPercentageBar(_meshesPercentage);
            DisplayMeshRules();
            MeshCalculatePercentageForRules();
        }

        private void DisplayTextureRules()
        {
            foreach (var rule in _textureRules)
            {
                try
                {
                    if (rule.CheckViolated())
                        rule.DisplayOnGUI();
                }
                catch
                {
                    ResetWindow();
                    break;
                }
            }
        }
        private void DisplayMeshRules()
        {
            foreach (var rule in _meshRules)
            {
                try
                {
                    if (rule.CheckViolated())
                        rule.DisplayOnGUI();
                }
                catch
                {
                    ResetWindow();
                    break;
                }
            }
        }

        private void TextureCalculatePercentageForRules()
        {
            _texturesPercentage = 0.0f;
            foreach (var rule in _textureRules)
            {
                _texturesPercentage += rule.percentageDelegate() / (float)_textureRules.Count;
            }
        }
        private void MeshCalculatePercentageForRules()
        {
            _meshesPercentage = 0.0f;
            foreach (var rule in _meshRules)
            {
                _meshesPercentage += rule.percentageDelegate() / (float)_meshRules.Count;
            }
        }
        private void DisplayPercentageBar(float percentage)
        {
            Color fillColor = Color.green;
            Color backgroundColor = Color.gray;
            if (percentage > 0.5f && percentage < 0.9f)
            {
                fillColor = Color.yellow;
            }
            else if (percentage <= 0.5f)
            {
                fillColor = Color.red;
                if (percentage == 0)
                {
                    backgroundColor = Color.red;
                }
            }

            EditorUtility.CoverInHorizontalBox(() =>
            {
                float height = 17;
                float gap = 20;
                int fontSize = 13;
                Rect healthBarRect = EditorGUILayout.GetControlRect(false, height);
                float backgroundWidth = EditorGUIUtility.currentViewWidth - gap;
                float filledWidth = percentage * backgroundWidth;

                EditorUtility.Bar(healthBarRect, backgroundWidth, filledWidth, height, fillColor, backgroundColor);
                string percentageText = "%" + (percentage * 100f).ToString("0.0");
                EditorGUILayout.LabelField(percentageText, EditorUtility.BoldLabel(TextAnchor.MiddleRight, fontSize, Color.black));
            });
        }
    }
}
