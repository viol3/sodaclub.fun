using UnityEditor;
using UnityEngine;
using UnityMeshSimplifier;

//-----------------------------------------------------------------------
// <copyright file="MeshSimplifierEditor.cs" company="Bermuda Games">
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

namespace Hyper.EditorMeshSimplifier
{
    public class MeshSimplifierEditor : EditorWindow
    {
        private GameObject _gameObject;
        private Mesh _editedMesh;
        private Mesh _meshCache;
        private Editor _meshDisplayEditor;
        private int _simplificationQuality = 0; // Default value

        bool _preserveBorderEdges = false;
        bool _preserveUVSeamEdges = false;
        bool _preserveUVFoldoverEdges = false;
        bool _preserveSurfaceCurvature = false;
        bool _enableSmartLink = true;
        double _vertexLinkDistance = double.Epsilon;
        int _maxIterationCount = 100;
        float _agressiveness = 7f;
        bool _manualUVComponentCount = false;
        int _UVComponentCount = 2;

        [MenuItem("Window/Hyper/Mesh Simplifier")]
        public static void ShowWindow()
        {
            GetWindow<MeshSimplifierEditor>("Mesh Simplifier");
        }

        public void OnGUI()
        {
            DisplayMeshField();
            DisplayMesh();
            if (_meshCache != null)
            {
                DisplayQualitySlider();

                EditorGUILayout.Space();

                if (EditorUtility.Button("Export as Mesh", GUILayout.Height(38)))
                {
                    string path = EditorUtility.AskForFilePanel("Export Simplified Mesh as FBX", "Assets", _gameObject.name + "_Simplified", "asset");
                    if (path.Length > 0)
                    {
                        MeshUtility.ExportMesh(_editedMesh.DuplicateMesh(), path);
                    }
                }
                EditorGUILayout.HelpBox("After exporting the mesh, drop original file to the scene and change the Mesh or Skinned Mesh renderer with exported .asset file.", MessageType.Info);
            }
        }

        private void DisplayMeshField()
        {
            EditorGUI.BeginChangeCheck();

            EventType currentEventType = Event.current.type;

            GUIStyle dropBoxStyle = new GUIStyle(GUI.skin.box);
            dropBoxStyle.alignment = TextAnchor.MiddleCenter;
            dropBoxStyle.normal.textColor = Color.gray;
            dropBoxStyle.wordWrap = true;

            if (_gameObject != null)
            {
                MeshRenderer meshRenderer = _gameObject.GetComponent<MeshRenderer>();
                SkinnedMeshRenderer skinnedMeshRenderer = _gameObject.GetComponent<SkinnedMeshRenderer>();

                if (meshRenderer != null || skinnedMeshRenderer != null)
                {
                    if (EditorUtility.InLineButtonHorizontal("Remove", () =>
                    {
                        _gameObject = (GameObject)EditorGUILayout.ObjectField(_gameObject, typeof(GameObject), true, GUILayout.ExpandWidth(true), GUILayout.Height(38));
                    }, false, GUILayout.Height(38), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(100), GUILayout.MinWidth(100), GUILayout.Width(100), GUILayout.MaxHeight(38), GUILayout.MinHeight(38), GUILayout.Height(38)))
                    {
                        _gameObject = null;
                        ResetVariables();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Selected GameObject must have a MeshRenderer or SkinnedMeshRenderer component.", MessageType.Warning);
                    _gameObject = null;
                }
            }
            else
            {
                GUILayout.Box("Drag & Drop Mesh or SkinnedMeshRenderer Asset Here", dropBoxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(200));

                switch (currentEventType)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                            if (currentEventType == EventType.DragPerform)
                            {
                                DragAndDrop.AcceptDrag();
                                if (DragAndDrop.objectReferences.Length > 0)
                                {
                                    GameObject draggedObject = DragAndDrop.objectReferences[0] as GameObject;
                                    MeshRenderer draggedMeshRenderer = draggedObject.GetComponent<MeshRenderer>();
                                    SkinnedMeshRenderer draggedSkinnedMeshRenderer = draggedObject.GetComponent<SkinnedMeshRenderer>();
                                    if (draggedMeshRenderer != null || draggedSkinnedMeshRenderer != null)
                                    {
                                        _gameObject = draggedObject;
                                    }
                                }
                            }

                            Event.current.Use();
                        }
                        break;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (_meshDisplayEditor != null) DestroyImmediate(_meshDisplayEditor);
                ResetVariables();
                GetMeshFromGameObject();
            }
        }

        private void GetMeshFromGameObject()
        {
            if (_gameObject != null)
            {
                Mesh mesh = _gameObject.GetMeshFromGameObject();
                if (mesh != null)
                {
                    _meshCache = mesh.DuplicateMesh();
                    _editedMesh = _meshCache.DuplicateMesh();
                }
            }
        }

        private void DisplayMesh()
        {
            if (_gameObject != null && _meshCache != null)
            {

                GUIStyle bgColor = new GUIStyle();
                bgColor.normal.background = EditorUtility.GetTexture(Color.clear);

                if (_meshDisplayEditor == null || _meshDisplayEditor.target != _editedMesh)
                {
                    if (_meshDisplayEditor != null)
                    {
                        DestroyImmediate(_meshDisplayEditor);
                    }
                    _meshDisplayEditor = Editor.CreateEditor(_editedMesh);
                }
                if (_meshDisplayEditor != null)
                {

                    _meshDisplayEditor.OnPreviewSettings();
                    _meshDisplayEditor.DrawPreview(GUILayoutUtility.GetRect(400, 400));
                }
            }
            else if (_gameObject != null && _meshCache == null)
            {
                if (EditorUtility.InLineButtonHorizontal("Remove", () =>
                {
                    EditorGUILayout.HelpBox("No mesh found in the selected GameObject", MessageType.Warning);
                }, false, GUILayout.Height(38)))
                {
                    _gameObject = null;
                    ResetVariables();
                }
                GetMeshFromGameObject();
            }
        }

        private void DisplayQualitySlider()
        {
            EditorGUI.BeginChangeCheck();

            _simplificationQuality = (int)EditorGUILayout.Slider("Simplification Quality", _simplificationQuality, 0, 100);
            _agressiveness = EditorGUILayout.Slider("Aggresiveness", _agressiveness, 1f, 14f);
            _enableSmartLink = EditorGUILayout.ToggleLeft("Enable Smart Link", _enableSmartLink);
            _preserveBorderEdges = EditorGUILayout.ToggleLeft("Preserve Border Edges", _preserveBorderEdges);
            _preserveUVSeamEdges = EditorGUILayout.ToggleLeft("Preserve UV Seam Edges", _preserveUVSeamEdges);
            _preserveUVFoldoverEdges = EditorGUILayout.ToggleLeft("Preserve UV Foldover Edges", _preserveUVFoldoverEdges);
            _preserveSurfaceCurvature = EditorGUILayout.ToggleLeft("Preserve Surface Curvature", _preserveSurfaceCurvature);
            _manualUVComponentCount = EditorGUILayout.ToggleLeft("Manual UV Component Count", _manualUVComponentCount);
            _UVComponentCount = EditorGUILayout.IntSlider("UV Component Count", _UVComponentCount, 1, 4);

            if (EditorGUI.EndChangeCheck())
            {
                if (_meshCache != null)
                {
                    var meshSimplifier = new MeshSimplifier();
                    meshSimplifier.SimplificationOptions = new SimplificationOptions
                    {
                        PreserveBorderEdges = _preserveBorderEdges,
                        PreserveUVSeamEdges = _preserveUVSeamEdges,
                        PreserveUVFoldoverEdges = _preserveUVFoldoverEdges,
                        PreserveSurfaceCurvature = _preserveSurfaceCurvature,
                        EnableSmartLink = _enableSmartLink,
                        VertexLinkDistance = _vertexLinkDistance,
                        MaxIterationCount = _maxIterationCount,
                        Agressiveness = _agressiveness,
                        ManualUVComponentCount = _manualUVComponentCount,
                        UVComponentCount = _UVComponentCount
                    };
                    _editedMesh.CopyParametersFrom(MeshUtility.SimplifyMesh(1f - ((float)_simplificationQuality / 100f), _meshCache, meshSimplifier));
                    _meshDisplayEditor.ReloadPreviewInstances();
                }
            }
        }
        private void ResetVariables()
        {
            _meshCache = null;
            _editedMesh = null;
            _meshDisplayEditor = null;
            _simplificationQuality = 0;
            _preserveBorderEdges = false;
            _preserveUVSeamEdges = false;
            _preserveUVFoldoverEdges = false;
            _preserveSurfaceCurvature = false;
            _enableSmartLink = true;
            _vertexLinkDistance = double.Epsilon;
            _maxIterationCount = 100;
            _agressiveness = 7f;
            _manualUVComponentCount = false;
            _UVComponentCount = 2;
        }
    }
}
