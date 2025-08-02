using UnityEngine;
using UnityEditor;
using System.IO;

public class MaterialFixer : EditorWindow
{
    [MenuItem("Tools/Fix Black Materials - Convert TAI to URP")]
    public static void FixBlackMaterials()
    {
        Debug.Log("🔧 Starting automatic material conversion...");
        
        // Find all TAI materials
        string[] materialGUIDs = AssetDatabase.FindAssets("t:Material TAI", new[] { "Assets/_Project" });
        int convertedCount = 0;
        
        foreach (string guid in materialGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            
            if (material != null && material.shader != null)
            {
                string shaderName = material.shader.name;
                Debug.Log($"Processing: {material.name} (Shader: {shaderName})");
                
                // Convert Built-in shaders to URP equivalents
                string newShaderName = ConvertToURPShader(shaderName);
                
                if (newShaderName != shaderName)
                {
                    Shader newShader = Shader.Find(newShaderName);
                    if (newShader != null)
                    {
                        // Store original properties before conversion
                        Texture mainTexture = material.mainTexture;
                        Color color = material.HasProperty("_Color") ? material.color : Color.white;
                        
                        // Convert to URP shader
                        material.shader = newShader;
                        
                        // Restore properties
                        if (mainTexture != null)
                            material.mainTexture = mainTexture;
                        
                        if (material.HasProperty("_BaseColor"))
                            material.SetColor("_BaseColor", color);
                        else if (material.HasProperty("_Color"))
                            material.SetColor("_Color", color);
                        
                        EditorUtility.SetDirty(material);
                        convertedCount++;
                        Debug.Log($"✅ Converted {material.name} to {newShaderName}");
                    }
                    else
                    {
                        Debug.LogWarning($"⚠️ URP shader not found: {newShaderName}");
                    }
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"🎉 Material conversion complete! Converted {convertedCount} materials to URP.");
        Debug.Log("Your environment should now render with proper colors!");
    }
    
    private static string ConvertToURPShader(string builtinShaderName)
    {
        // Map common Built-in shaders to URP equivalents
        switch (builtinShaderName.ToLower())
        {
            case var name when name.Contains("standard"):
                return "Universal Render Pipeline/Lit";
            case var name when name.Contains("diffuse"):
                return "Universal Render Pipeline/Simple Lit";
            case var name when name.Contains("bumped diffuse"):
                return "Universal Render Pipeline/Lit";
            case var name when name.Contains("transparent"):
                return "Universal Render Pipeline/Lit";
            case var name when name.Contains("cutout"):
                return "Universal Render Pipeline/Lit";
            case var name when name.Contains("unlit"):
                return "Universal Render Pipeline/Unlit";
            default:
                return "Universal Render Pipeline/Lit"; // Default fallback
        }
    }
} 