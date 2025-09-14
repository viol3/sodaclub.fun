using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hyper.Checkup
{
    public static class RulesManager
    {
        #region TextureRules
        public const int k_maxTextureSize = 1024;
        public const int k_minViolateCount = 3;
        public static Rule TextureRule1(List<TextureEntry> textureEntries)
        {
            string info = $"You have {k_minViolateCount} or more UI textures, use sprite atlasses instead";
            Rule rule = new Rule(info, null, null);
            Rule.RuleDelegate ruleDelegate = () =>
            {
                int singleTexturesCount = 0;
                for (int i = 0; i < textureEntries.Count; i++)
                {
                    TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(textureEntries[i].Path);

                    if (textureImporter.textureType != TextureImporterType.Sprite) continue;

                    if (textureImporter.spriteImportMode == SpriteImportMode.Single || textureImporter.spriteImportMode == SpriteImportMode.None)
                    {
                        singleTexturesCount++;
                    }
                }
                rule.violatedCount = singleTexturesCount;
                return singleTexturesCount >= k_minViolateCount;
            };
            Rule.PercentageDelegate percentageDelegate = () =>
            {
                if (rule.isViolated && rule.violatedCount > k_minViolateCount)
                    return 1f - ((float)(rule.violatedCount - (float)RulesManager.k_minViolateCount) / (float)textureEntries.Count);
                return 1f;
            };
            rule.percentageDelegate = percentageDelegate;
            rule.ruleDelegate = ruleDelegate;
            return rule;
        }
        public static Rule TextureRule2(List<TextureEntry> textureEntries)
        {
            string info = "Some Textures are not sized with power of 2";
            Rule rule = new Rule(info, null, null);
            Rule.RuleDelegate ruleDelegate = () =>
            {
                rule.violatedCount = textureEntries.FindAll(x => !TextureUtility.IsPowerOfTwo(x.Texture)).Count;
                return textureEntries.Exists(x => !TextureUtility.IsPowerOfTwo(x.Texture));
            };
            Rule.PercentageDelegate percentageDelegate = () =>
            {
                if (rule.isViolated)
                    return 1f - (float)rule.violatedCount / (float)textureEntries.Count;
                return 1f;
            };
            rule.percentageDelegate = percentageDelegate;
            rule.ruleDelegate = ruleDelegate;
            return rule;
        }
        public static Rule TextureRule3(List<TextureEntry> textureEntries)
        {
            string info = "You have Model/Sprite textures sized more than 1024";
            Rule rule = new Rule(info, null, null);
            Rule.RuleDelegate ruleDelegate = () =>
            {
                rule.violatedCount = textureEntries.FindAll(x => x.Texture.width > k_maxTextureSize || x.Texture.height > k_maxTextureSize).Count;
                return textureEntries.Exists(x => x.Texture.width > k_maxTextureSize || x.Texture.height > k_maxTextureSize);
            };
            Rule.PercentageDelegate percentageDelegate = () =>
            {
                if (rule.CheckViolated())
                    return 1f - (float)rule.violatedCount / (float)textureEntries.Count;
                return 1f;
            };
            rule.percentageDelegate = percentageDelegate;
            rule.ruleDelegate = ruleDelegate;
            return rule;
        }
        #endregion
        #region  MeshRules
        public static Rule MeshRule1(List<MeshEntry> meshEntries)
        {
            int trisCount = 10000;
            string info = $"You have meshes that has more than {trisCount} tris";
            Rule rule = new Rule(info, null, null);
            Rule.RuleDelegate ruleDelegate = () =>
            {
                rule.violatedCount = meshEntries.FindAll(x => x.Mesh.triangles.Length > trisCount).Count;
                return meshEntries.Exists(x => x.Mesh.triangles.Length > trisCount);
            };
            Rule.PercentageDelegate percentageDelegate = () =>
            {
                if (rule.CheckViolated())
                    return 1f - (float)rule.violatedCount / (float)meshEntries.Count;
                return 1f;
            };
            rule.percentageDelegate = percentageDelegate;
            rule.ruleDelegate = ruleDelegate;
            return rule;
        }
        #endregion
    }
    [System.Serializable]
    public class Rule
    {
        public string info;
        public bool isViolated;
        public delegate float PercentageDelegate();
        public PercentageDelegate percentageDelegate;
        public delegate bool RuleDelegate();
        public RuleDelegate ruleDelegate;
        public int violatedCount;
        public Rule(string info, RuleDelegate ruleDelegate, PercentageDelegate percentageDelegate, int violatedCount = 0)
        {
            this.info = info;
            this.ruleDelegate = ruleDelegate;
            this.violatedCount = violatedCount;
            this.percentageDelegate = percentageDelegate;
        }
        public bool CheckViolated()
        {
            isViolated = ruleDelegate();
            return isViolated;
        }
        public void DisplayOnGUI()
        {
            if (isViolated)
            {
                EditorUtility.Label("● " + info, EditorUtility.BoldLabel(TextAnchor.MiddleLeft, 13, Color.white));
                EditorGUILayout.Space(5);
            }
        }
    }
}
