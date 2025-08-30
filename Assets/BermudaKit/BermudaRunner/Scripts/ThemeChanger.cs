using System.Collections.Generic;
using Ali.Helper;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class MaterialColorChanger
{
    public Material material;
    
    public Color normalMainColor;
    public Color normalShadowColor;
    
    public Color monoMainColor;
    public Color monoShadowColor;

    public Color purpleMainColor;
    public Color purpleShadowColor;

    public Color darkBlueMainColor;
    public Color darkBlueShadowColor;
    
    public Color redMainColor;
    public Color redShadowColor;
    
    public Color orangeMainColor;
    public Color orangeShadowColor;
    
    public Color screenShotMainColor;
    public Color screenShotShadowColor;
    
}

public enum Theme
{
    Normal,
    Monochrome,
    Orange,
    Purple,
    DarkBlue,
    Red,
    ScreenShot
}

public class ThemeChanger : LocalSingleton<ThemeChanger>
{
    [SerializeField] private List<MaterialColorChanger> materialColorChangers;
    [SerializeField] private Material[] _skyBoxes;
    [SerializeField] private Color[] _fogColors;
    private static readonly int MainColor = Shader.PropertyToID("_Color");
    private static readonly int ShadowColor = Shader.PropertyToID("_SColor");

    public void ChangeTheme(Theme theme)
    {
        foreach (var materialColorChanger in materialColorChangers)
        {
            switch (theme)
            {
                case Theme.Normal:
                    materialColorChanger.material.SetColor(MainColor, materialColorChanger.normalMainColor);
                    materialColorChanger.material.SetColor(ShadowColor, materialColorChanger.normalShadowColor);
                    break;
                case Theme.Monochrome:
                    materialColorChanger.material.SetColor(MainColor, materialColorChanger.monoMainColor);
                    materialColorChanger.material.SetColor(ShadowColor, materialColorChanger.monoShadowColor);
                    break;
                case Theme.Purple:
                    materialColorChanger.material.SetColor(MainColor, materialColorChanger.purpleMainColor);
                    materialColorChanger.material.SetColor(ShadowColor, materialColorChanger.purpleShadowColor);
                    break;
                case Theme.Orange:
                    materialColorChanger.material.SetColor(MainColor, materialColorChanger.orangeMainColor);
                    materialColorChanger.material.SetColor(ShadowColor, materialColorChanger.orangeShadowColor);
                    break;
                case Theme.DarkBlue:
                    materialColorChanger.material.SetColor(MainColor, materialColorChanger.darkBlueMainColor);
                    materialColorChanger.material.SetColor(ShadowColor, materialColorChanger.darkBlueShadowColor);
                    break;
                case Theme.Red:
                    materialColorChanger.material.SetColor(MainColor, materialColorChanger.redMainColor);
                    materialColorChanger.material.SetColor(ShadowColor, materialColorChanger.redShadowColor);
                    break;
                case Theme.ScreenShot:
                    materialColorChanger.material.SetColor(MainColor, materialColorChanger.screenShotMainColor);
                    materialColorChanger.material.SetColor(ShadowColor, materialColorChanger.screenShotShadowColor);
                    break;
                default:
                    materialColorChanger.material.SetColor(MainColor, materialColorChanger.normalMainColor);
                    materialColorChanger.material.SetColor(ShadowColor, materialColorChanger.normalShadowColor);
                    break;
            }
        }

        RenderSettings.skybox = theme switch
        {
            Theme.Normal => _skyBoxes[0],
            Theme.Monochrome => _skyBoxes[1],
            Theme.Orange => _skyBoxes[2],
            Theme.Purple => _skyBoxes[3],
            Theme.DarkBlue => _skyBoxes[4],
            Theme.Red => _skyBoxes[5],
            Theme.ScreenShot => _skyBoxes[6],
            _ => _skyBoxes[0]
        };
        RenderSettings.fogColor = theme switch
        {
            Theme.Normal => _fogColors[0],
            Theme.Monochrome => _fogColors[1],
            Theme.Orange => _fogColors[2],
            Theme.Purple => _fogColors[3],
            Theme.DarkBlue => _fogColors[4],
            Theme.Red => _fogColors[5],
            Theme.ScreenShot => _fogColors[6],
            _ => _fogColors[0]
        };
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ThemeChanger))]
public class MaterialColorManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var myScript = (ThemeChanger)target;
        var buttonWidth = EditorGUIUtility.currentViewWidth / 2;
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Normal Theme", GUILayout.Width(buttonWidth - 10), GUILayout.Height(40)))
        {
            myScript.ChangeTheme(Theme.Normal);
        }
        if (GUILayout.Button("Monochrome Theme", GUILayout.Width(buttonWidth - 10), GUILayout.Height(40)))
        {
            myScript.ChangeTheme(Theme.Monochrome);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Orange Theme", GUILayout.Width(buttonWidth - 10), GUILayout.Height(40)))
        {
            myScript.ChangeTheme(Theme.Orange);
        }
        if (GUILayout.Button("Purple Theme", GUILayout.Width(buttonWidth - 10), GUILayout.Height(40)))
        {
            myScript.ChangeTheme(Theme.Purple);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("DarkBlue Theme", GUILayout.Width(buttonWidth - 10), GUILayout.Height(40)))
        {
            myScript.ChangeTheme(Theme.DarkBlue);
        }
        if (GUILayout.Button("Red Theme", GUILayout.Width(buttonWidth - 10), GUILayout.Height(40)))
        {
            myScript.ChangeTheme(Theme.Red);
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Screenshot Theme", GUILayout.Width(buttonWidth - 10), GUILayout.Height(40)))
        {
            myScript.ChangeTheme(Theme.ScreenShot);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}
#endif
