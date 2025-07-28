using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.IO;

/// <summary>
/// Unity Troubleshooting Toolkit - Reusable fixes for common Unity issues
/// Created after resolving major URP material and rendering problems
/// 
/// LESSON LEARNED: When ALL materials turn pink/magenta in Unity:
/// 1. Check if URP Pipeline Asset exists and is assigned in Graphics Settings
/// 2. Check if URP Pipeline Asset has a Default Renderer assigned
/// 3. Only THEN worry about individual materials and textures
/// </summary>
public class UnityTroubleshootingToolkit : EditorWindow
{
    // CUSTOMIZE THESE FOR EACH PROJECT
    private const string PROJECT_NAME = "PlasmaDragon";
    private const string MAIN_OBJECT_NAME = "Unka Toon"; // Customize per project
    private const string PARENT_OBJECT_NAME = "Plasma Dragon"; // Customize per project
    
    [MenuItem("Tools/Unity Troubleshooting Toolkit")]
    public static void ShowWindow()
    {
        GetWindow<UnityTroubleshootingToolkit>("Unity Toolkit");
    }
    
    void OnGUI()
    {
        GUILayout.Label($"{PROJECT_NAME} Troubleshooting Toolkit", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("🚨 CRITICAL URP FIXES (Use These First!)", EditorStyles.boldLabel);
        
        if (GUILayout.Button("🔧 FIX URP PIPELINE - SOLVE PINK MATERIALS"))
        {
            CreateAndAssignURPPipelineAsset();
        }
        
        if (GUILayout.Button("🎮 FIX URP RENDERER - SOLVE BLACK SCREEN"))
        {
            FixURPRendererData();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("📊 Diagnostics", EditorStyles.boldLabel);
        
        if (GUILayout.Button("🔍 Check URP Setup"))
        {
            CheckURPSetup();
        }
        
        if (GUILayout.Button("🎯 Check Project Status"))
        {
            CheckProjectStatus();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("🎨 Material & Rendering Fixes", EditorStyles.boldLabel);
        
        if (GUILayout.Button("⬆️ Upgrade All Materials to URP"))
        {
            UpgradeAllMaterialsToURP();
        }
        
        if (GUILayout.Button("🐉 Fix Main Object Materials"))
        {
            FixMainObjectMaterials();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("📸 Scene & Camera Fixes", EditorStyles.boldLabel);
        
        if (GUILayout.Button("💡 Fix Lighting and Camera"))
        {
            FixLightingAndCamera();
        }
        
        if (GUILayout.Button("👁️ Fix Scene Visibility"))
        {
            FixSceneVisibility();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("🎯 Project-Specific Fixes", EditorStyles.boldLabel);
        
        if (GUILayout.Button($"🔧 Custom {PROJECT_NAME} Fixes"))
        {
            CustomProjectFixes();
        }
        
        GUILayout.Space(20);
        GUILayout.Label("💡 Remember: URP Pipeline Asset is the foundation!", EditorStyles.helpBox);
    }
    
    /// <summary>
    /// THE BIG FIX: Creates and assigns URP Pipeline Asset
    /// This fixes the root cause of pink/magenta materials
    /// </summary>
    static void CreateAndAssignURPPipelineAsset()
    {
        Debug.Log("🔧 Creating URP Pipeline Asset...");
        
        // Create the pipeline asset
        string assetPath = "Assets/_Project/URP-PipelineAsset.asset";
        
        var pipelineAsset = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
        AssetDatabase.CreateAsset(pipelineAsset, assetPath);
        
        // Assign it in Graphics Settings
        GraphicsSettings.defaultRenderPipeline = pipelineAsset;
        
        // Force refresh
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("✅ URP Pipeline Asset created and assigned!");
        Debug.Log("🎯 This should fix pink/magenta materials across your project!");
        
        // Also create the renderer data
        FixURPRendererData();
    }
    
    /// <summary>
    /// Creates and assigns Universal Renderer Data to URP Pipeline Asset
    /// This fixes black screen in Play mode
    /// </summary>
    static void FixURPRendererData()
    {
        Debug.Log("🎮 Fixing URP Renderer Data...");
        
        var pipelineAsset = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
        if (pipelineAsset == null)
        {
            Debug.LogError("❌ No URP Pipeline Asset found! Run 'Fix URP Pipeline' first!");
            return;
        }
        
        // Create Universal Renderer Data
        string rendererPath = "Assets/_Project/UniversalRenderer.asset";
        var rendererData = ScriptableObject.CreateInstance<UniversalRendererData>();
        AssetDatabase.CreateAsset(rendererData, rendererPath);
        
        // Use reflection to assign the renderer to the pipeline asset
        var serializedObject = new SerializedObject(pipelineAsset);
        var rendererListProperty = serializedObject.FindProperty("m_RendererDataList");
        
        if (rendererListProperty != null)
        {
            rendererListProperty.arraySize = 1;
            rendererListProperty.GetArrayElementAtIndex(0).objectReferenceValue = rendererData;
            serializedObject.ApplyModifiedProperties();
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("✅ Universal Renderer Data created and assigned!");
        Debug.Log("🎯 This should fix black screen in Play mode!");
    }
    
    /// <summary>
    /// Converts all Standard materials to URP Lit shader
    /// </summary>
    static void UpgradeAllMaterialsToURP()
    {
        Debug.Log("⬆️ Upgrading all materials to URP...");
        
        var urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLitShader == null)
        {
            Debug.LogError("❌ URP Lit shader not found! Install URP package first!");
            return;
        }
        
        string[] materialGUIDs = AssetDatabase.FindAssets("t:Material");
        int upgraded = 0;
        
        foreach (string guid in materialGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            
            if (material != null && material.shader.name == "Standard")
            {
                material.shader = urpLitShader;
                EditorUtility.SetDirty(material);
                upgraded++;
                Debug.Log($"✅ Upgraded: {material.name}");
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log($"🎯 Upgraded {upgraded} materials to URP Lit shader!");
    }
    
    /// <summary>
    /// Fixes materials on the main object (customize MAIN_OBJECT_NAME)
    /// </summary>
    static void FixMainObjectMaterials()
    {
        GameObject mainObject = GameObject.Find(MAIN_OBJECT_NAME);
        if (mainObject == null)
        {
            Debug.LogError($"❌ {MAIN_OBJECT_NAME} not found in scene!");
            return;
        }
        
        Debug.Log($"🐉 Fixing materials on {MAIN_OBJECT_NAME}...");
        
        var renderers = mainObject.GetComponentsInChildren<Renderer>();
        var urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
        
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                if (material != null && material.shader.name != "Universal Render Pipeline/Lit")
                {
                    material.shader = urpLitShader;
                    EditorUtility.SetDirty(material);
                    Debug.Log($"✅ Fixed material: {material.name}");
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log($"🎯 Fixed all materials on {MAIN_OBJECT_NAME}!");
    }
    
    /// <summary>
    /// Fixes common lighting and camera issues
    /// </summary>
    static void FixLightingAndCamera()
    {
        Debug.Log("💡 Fixing lighting and camera...");
        
        // Ensure there's a directional light
        Light directionalLight = FindObjectOfType<Light>();
        if (directionalLight == null || directionalLight.type != LightType.Directional)
        {
            GameObject lightObj = new GameObject("Directional Light");
            directionalLight = lightObj.AddComponent<Light>();
            directionalLight.type = LightType.Directional;
            directionalLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            Debug.Log("✅ Created Directional Light");
        }
        
        // Fix camera settings
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.Skybox;
            mainCamera.backgroundColor = Color.blue;
            mainCamera.nearClipPlane = 0.3f;
            mainCamera.farClipPlane = 1000f;
            Debug.Log("✅ Fixed main camera settings");
        }
        
        // Force scene view refresh
        SceneView.RepaintAll();
        Debug.Log("🎯 Lighting and camera fixes applied!");
    }
    
    /// <summary>
    /// Fixes scene visibility issues
    /// </summary>
    static void FixSceneVisibility()
    {
        Debug.Log("👁️ Fixing scene visibility...");
        
        // Reset scene view to show everything
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.showGrid = true;
            SceneView.lastActiveSceneView.sceneViewState.showSkybox = true;
            SceneView.lastActiveSceneView.Repaint();
            Debug.Log("✅ Scene view settings reset");
        }
        
        // Find and activate main objects
        GameObject mainObject = GameObject.Find(MAIN_OBJECT_NAME);
        if (mainObject != null)
        {
            mainObject.SetActive(true);
            Selection.activeGameObject = mainObject;
            SceneView.FrameLastActiveSceneView();
            Debug.Log($"✅ Found and activated {MAIN_OBJECT_NAME}");
        }
        
        Debug.Log("🎯 Scene visibility fixes applied!");
    }
    
    /// <summary>
    /// Comprehensive URP setup check
    /// </summary>
    static void CheckURPSetup()
    {
        Debug.Log("🔍 Checking URP Setup...");
        
        var pipeline = GraphicsSettings.defaultRenderPipeline;
        if (pipeline == null)
        {
            Debug.LogError("❌ NO RENDER PIPELINE ASSET ASSIGNED!");
            Debug.LogError("💡 This is why all materials are pink/magenta!");
            return;
        }
        
        Debug.Log($"✅ Render Pipeline: {pipeline.name}");
        
        var urpAsset = pipeline as UniversalRenderPipelineAsset;
        if (urpAsset != null)
        {
            Debug.Log("✅ URP Pipeline Asset found");
            
            // Check renderer data
            var serializedObject = new SerializedObject(urpAsset);
            var rendererListProperty = serializedObject.FindProperty("m_RendererDataList");
            
            if (rendererListProperty != null && rendererListProperty.arraySize > 0)
            {
                var renderer = rendererListProperty.GetArrayElementAtIndex(0).objectReferenceValue;
                if (renderer != null)
                {
                    Debug.Log("✅ Default Renderer assigned");
                }
                else
                {
                    Debug.LogError("❌ DEFAULT RENDERER IS MISSING!");
                    Debug.LogError("💡 This causes black screen in Play mode!");
                }
            }
        }
        
        Debug.Log("🎯 URP Setup check complete!");
    }
    
    /// <summary>
    /// Checks overall project status
    /// </summary>
    static void CheckProjectStatus()
    {
        Debug.Log("🎯 Checking Project Status...");
        
        // Check main objects
        GameObject mainObject = GameObject.Find(MAIN_OBJECT_NAME);
        GameObject parentObject = GameObject.Find(PARENT_OBJECT_NAME);
        
        Debug.Log($"Main Object ({MAIN_OBJECT_NAME}): {(mainObject != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Parent Object ({PARENT_OBJECT_NAME}): {(parentObject != null ? "✅ Found" : "❌ Missing")}");
        
        // Check camera
        Camera mainCamera = Camera.main;
        Debug.Log($"Main Camera: {(mainCamera != null ? "✅ Found" : "❌ Missing")}");
        
        // Check lights
        Light[] lights = FindObjectsOfType<Light>();
        Debug.Log($"Lights in scene: {lights.Length}");
        
        Debug.Log("🎯 Project status check complete!");
    }
    
    /// <summary>
    /// CUSTOMIZE THIS FOR EACH PROJECT
    /// Add project-specific fixes here
    /// </summary>
    static void CustomProjectFixes()
    {
        Debug.Log($"🔧 Running custom {PROJECT_NAME} fixes...");
        
        // Example: Fix dragon materials specifically
        FixMainObjectMaterials();
        
        // Example: Check for flight controller (generic component check)
        GameObject parentObject = GameObject.Find(PARENT_OBJECT_NAME);
        if (parentObject != null)
        {
            var components = parentObject.GetComponents<MonoBehaviour>();
            bool hasFlightController = false;
            
            foreach (var comp in components)
            {
                if (comp != null && comp.GetType().Name.Contains("Flight"))
                {
                    hasFlightController = true;
                    Debug.Log($"✅ Flight controller found: {comp.GetType().Name}");
                    break;
                }
            }
            
            if (!hasFlightController)
            {
                Debug.LogWarning($"⚠️ No flight controller found on {PARENT_OBJECT_NAME}");
            }
        }
        
        // Add more project-specific fixes here...
        
        Debug.Log($"🎯 Custom {PROJECT_NAME} fixes complete!");
    }
} 