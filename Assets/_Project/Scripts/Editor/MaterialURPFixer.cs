using UnityEngine;
using UnityEditor;
using System.IO;

namespace PlasmaDragon.Editor
{
    public class MaterialURPFixer : EditorWindow
    {
        [MenuItem("Tools/Fix Pink Textures - URP Converter")]
        public static void FixPinkTextures()
        {
            // Find all TAI materials
            string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/_Project/Models/Toon Adventure Island" });
            
            int fixedCount = 0;
            
            foreach (string guid in materialGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                
                if (material != null)
                {
                    // Check if it's using the wrong shader
                    if (material.shader.name.Contains("Standard") || material.shader.name.Contains("Legacy"))
                    {
                        // Convert to URP Lit shader
                        material.shader = Shader.Find("Universal Render Pipeline/Lit");
                        EditorUtility.SetDirty(material);
                        fixedCount++;
                        
                        Debug.Log($"Fixed material: {path}");
                    }
                }
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"✅ Fixed {fixedCount} materials to URP shaders!");
            EditorUtility.DisplayDialog("URP Fix Complete", $"Fixed {fixedCount} materials to URP shaders!", "OK");
        }
        
        [MenuItem("Tools/Check URP Pipeline Settings")]
        public static void CheckURPSettings()
        {
            // Check if URP pipeline asset is assigned
            var pipelineAsset = GraphicsSettings.renderPipelineAsset;
            
            if (pipelineAsset == null)
            {
                Debug.LogError("❌ No URP Pipeline Asset assigned! Go to Edit > Project Settings > Graphics");
                EditorUtility.DisplayDialog("URP Pipeline Missing", "No URP Pipeline Asset assigned! Go to Edit > Project Settings > Graphics", "OK");
            }
            else
            {
                Debug.Log($"✅ URP Pipeline Asset found: {pipelineAsset.name}");
                EditorUtility.DisplayDialog("URP Pipeline OK", $"URP Pipeline Asset: {pipelineAsset.name}", "OK");
            }
        }
        
        [MenuItem("Tools/Convert All Materials to URP")]
        public static void ConvertAllMaterialsToURP()
        {
            // Find all materials in the project
            string[] materialGuids = AssetDatabase.FindAssets("t:Material");
            
            int convertedCount = 0;
            
            foreach (string guid in materialGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                
                if (material != null)
                {
                    // Convert Standard shaders to URP
                    if (material.shader.name.Contains("Standard"))
                    {
                        material.shader = Shader.Find("Universal Render Pipeline/Lit");
                        EditorUtility.SetDirty(material);
                        convertedCount++;
                    }
                    // Convert Unlit shaders to URP
                    else if (material.shader.name.Contains("Unlit"))
                    {
                        material.shader = Shader.Find("Universal Render Pipeline/Unlit");
                        EditorUtility.SetDirty(material);
                        convertedCount++;
                    }
                }
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"✅ Converted {convertedCount} materials to URP!");
            EditorUtility.DisplayDialog("URP Conversion Complete", $"Converted {convertedCount} materials to URP shaders!", "OK");
        }
    }
} 