using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using System.Linq;

public class DragonToolkit : EditorWindow
{
    private Vector2 scrollPosition;
    private const string PROJECT_NAME = "TreasureDragon";
    private const string MAIN_OBJECT_NAME = "Dragon";
    
    // Vegetation slider values
    private static float grassPercentage = 20f;
    private static float plantPercentage = 20f;
    private static float treePercentage = 20f;
    private static float palmTreePercentage = 20f;
    private static float vinePercentage = 20f;
    private static float smallRockPercentage = 20f;
    
    private static readonly Dictionary<string, string> ASSET_PATHS = new Dictionary<string, string>
    {
        ["MainTexture"] = "Assets/Textures/DragonTexture.png",
        ["MaterialsFolder"] = "Assets/Materials/",
        ["URPAssetPath"] = "Assets/Settings/URP-PipelineAsset.asset",
        ["RendererDataPath"] = "Assets/Settings/UniversalRenderer.asset"
    };

    [MenuItem("Tools/Dragon Toolkit")]
    public static void ShowWindow()
    {
        GetWindow<DragonToolkit>("Dragon Toolkit");
    }

    void OnGUI()
    {
        GUILayout.Label("🐉 TreasureDragon Toolkit v2.0", EditorStyles.boldLabel);
        GUILayout.Space(10);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Emergency Fixes Section
        GUILayout.Label("🚨 Emergency Fixes", EditorStyles.boldLabel);
        if (GUILayout.Button("🚨 EMERGENCY - Fix Everything"))
        {
            EmergencyFixEverything();
        }
        if (GUILayout.Button("🔍 Diagnose All Issues"))
        {
            DiagnoseAllIssues();
        }

        GUILayout.Space(10);

        // Vegetation Optimizer Section
        GUILayout.Label("🌿 Vegetation Optimizer", EditorStyles.boldLabel);
        GUILayout.Label("Interactive sliders to reduce vegetation for prefab optimization", EditorStyles.helpBox);
        
        ShowVegetationSlider("🌱 Grass", "grass", ref grassPercentage);
        ShowVegetationSlider("🌿 Plants", "plant", ref plantPercentage);  
        ShowVegetationSlider("🌳 Trees", "TAI_Tree_", ref treePercentage);
        ShowVegetationSlider("🌴 Palm Trees", "palm", ref palmTreePercentage);
        ShowVegetationSlider("🍃 Vines", "vine", ref vinePercentage);
        ShowVegetationSlider("🪨 Small Rock", "TAI_Small_Rock", ref smallRockPercentage);

        GUILayout.Space(10);

        // Individual Fixes Section
        GUILayout.Label("🔧 Individual Fixes", EditorStyles.boldLabel);
        if (GUILayout.Button("Fix URP Setup"))
        {
            FixURPSetup();
        }
        if (GUILayout.Button("Fix Lighting and Camera"))
        {
            FixLightingAndCamera();
        }
        if (GUILayout.Button("Fix Scene Visibility"))
        {
            FixSceneVisibility();
        }
        if (GUILayout.Button("Fix All Materials"))
        {
            FixAllMaterials();
        }

        GUILayout.Space(10);

        // Diagnostics Section
        GUILayout.Label("🔍 Diagnostics", EditorStyles.boldLabel);
        if (GUILayout.Button("Diagnose Render Pipeline"))
        {
            DiagnoseRenderPipeline();
        }
        if (GUILayout.Button("Diagnose Camera"))
        {
            DiagnoseCamera();
        }
        if (GUILayout.Button("Diagnose Lighting"))
        {
            DiagnoseLighting();
        }
        if (GUILayout.Button("Diagnose Main Object"))
        {
            DiagnoseMainObject();
        }
        if (GUILayout.Button("Diagnose Materials"))
        {
            DiagnoseMaterials();
        }
        if (GUILayout.Button("Diagnose Grass Objects"))
        {
            DiagnoseGrassObjects();
        }

        EditorGUILayout.EndScrollView();
    }

    [MenuItem("Tools/🚨 EMERGENCY - Fix Everything")]
    static void EmergencyFixEverything()
    {
        Debug.Log($"🚨 EMERGENCY FIX INITIATED FOR {PROJECT_NAME}...");
        FixURPSetup();
        FixLightingAndCamera();
        FixSceneVisibility();
        FixAllMaterials();
        CustomProjectFixes();
        Debug.Log("✅ Emergency fixes complete!");
    }

    [MenuItem("Tools/🔍 Diagnose All Issues")]
    static void DiagnoseAllIssues()
    {
        Debug.Log($"🔍 DIAGNOSING ALL ISSUES FOR {PROJECT_NAME}...");
        DiagnoseRenderPipeline();
        DiagnoseCamera();
        DiagnoseLighting();
        DiagnoseMainObject();
        DiagnoseMaterials();
        DiagnoseGrassObjects();
        Debug.Log("🔍 Diagnostic complete - check console for results!");
    }

    [MenuItem("Tools/🌱 DESTROY 30% Random Grass (Prefab Optimization)")]
    static void RemoveRandomGrass30()
    {
        if (EditorUtility.DisplayDialog("Confirm Grass Deletion", 
            "This will PERMANENTLY DELETE 30% of grass objects to reduce prefab file size.\n\nThis CANNOT be undone!\n\nSave a backup first?", 
            "DELETE Grass", "Cancel"))
        {
            RemoveRandomGrass(0.3f);
        }
    }

    static void RemoveRandomGrass(float percentage)
    {
        Debug.Log($"🌱 PERMANENTLY REMOVING {percentage * 100}% OF GRASS OBJECTS FOR PREFAB OPTIMIZATION...");
        
        // Find all GameObjects with "grass" in their name (case insensitive)
        List<GameObject> grassObjects = new List<GameObject>();
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("grass"))
            {
                grassObjects.Add(obj);
            }
        }
        
        if (grassObjects.Count == 0)
        {
            Debug.LogWarning("⚠️ No grass objects found in scene!");
            return;
        }
        
        Debug.Log($"Found {grassObjects.Count} grass objects in scene");
        
        // Calculate how many to remove
        int objectsToRemove = Mathf.RoundToInt(grassObjects.Count * percentage);
        Debug.Log($"⚠️ PERMANENTLY DESTROYING {objectsToRemove} grass objects ({percentage * 100}%) - THIS CANNOT BE UNDONE!");
        
        // Shuffle the list and remove the specified percentage
        System.Random random = new System.Random();
        var shuffledGrass = grassObjects.OrderBy(x => random.Next()).ToList();
        
        int removedCount = 0;
        long totalVertices = 0;
        
        for (int i = 0; i < objectsToRemove && i < shuffledGrass.Count; i++)
        {
            GameObject grassObj = shuffledGrass[i];
            if (grassObj != null)
            {
                // Count vertices for polygon reduction reporting
                MeshFilter[] meshFilters = grassObj.GetComponentsInChildren<MeshFilter>();
                foreach (MeshFilter mf in meshFilters)
                {
                    if (mf.sharedMesh != null)
                        totalVertices += mf.sharedMesh.vertexCount;
                }
                
                Debug.Log($"🗑️ DESTROYING grass object: {grassObj.name}");
                DestroyImmediate(grassObj);
                removedCount++;
            }
        }
        
        Debug.Log($"✅ Successfully DESTROYED {removedCount} grass objects!");
        Debug.Log($"📊 Reduced vertex count by approximately: {totalVertices}");
        Debug.Log($"📦 Prefab file size will be significantly reduced!");
        Debug.Log($"Remaining grass objects: {grassObjects.Count - removedCount}");
        
        // Mark scene as dirty for saving
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
    

    
    static void DiagnoseGrassObjects()
    {
        Debug.Log("🔍 GRASS OBJECTS DIAGNOSIS:");
        
        // Find all grass objects in the scene
        List<GameObject> grassObjects = new List<GameObject>();
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        long totalVertices = 0;
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("grass"))
            {
                grassObjects.Add(obj);
                
                // Count vertices for polygon analysis
                MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
                foreach (MeshFilter mf in meshFilters)
                {
                    if (mf.sharedMesh != null)
                        totalVertices += mf.sharedMesh.vertexCount;
                }
            }
        }
        
        Debug.Log($"   Total grass objects: {grassObjects.Count}");
        Debug.Log($"   Total grass vertices: {totalVertices}");
        Debug.Log($"   Estimated polygon count: {totalVertices / 3}");
        
        if (grassObjects.Count > 0)
        {
            Debug.Log("   Grass objects found:");
            foreach (GameObject grass in grassObjects.Take(5))
            {
                MeshFilter[] meshFilters = grass.GetComponentsInChildren<MeshFilter>();
                int objVertices = 0;
                foreach (MeshFilter mf in meshFilters)
                {
                    if (mf.sharedMesh != null)
                        objVertices += mf.sharedMesh.vertexCount;
                }
                Debug.Log($"      - {grass.name} at {grass.transform.position} ({objVertices} vertices)");
            }
            if (grassObjects.Count > 5)
                Debug.Log($"      ... and {grassObjects.Count - 5} more");
        }
        
        if (grassObjects.Count > 100)
        {
            Debug.Log("💡 SUGGESTION: Consider removing 30-50% of grass for prefab optimization");
        }
    }

    static void FixURPSetup()
    {
        Debug.Log("🔧 Fixing URP setup...");
        
        // Set URP as the active render pipeline
        var urpAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(ASSET_PATHS["URPAssetPath"]);
        if (urpAsset == null)
        {
            // Find any URP asset in the project
            string[] guids = AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                urpAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);
            }
        }
        
        if (urpAsset != null)
        {
            GraphicsSettings.defaultRenderPipeline = urpAsset;
            QualitySettings.renderPipeline = urpAsset;
            Debug.Log("✅ URP setup fixed!");
        }
        else
        {
            Debug.LogWarning("⚠️ URP asset not found - check if URP is installed");
        }
    }

    static void FixLightingAndCamera()
    {
        Debug.Log("🔧 Fixing lighting and camera...");
        
        // Fix main camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject cameraGO = new GameObject("Main Camera");
            mainCamera = cameraGO.AddComponent<Camera>();
            cameraGO.tag = "MainCamera";
        }
        
        mainCamera.transform.position = new Vector3(0, 1, -10);
        mainCamera.transform.rotation = Quaternion.identity;
        mainCamera.fieldOfView = 60f;
        mainCamera.backgroundColor = Color.black;
        
        // Add directional light if none exists
        Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        bool hasDirectionalLight = false;
        foreach (Light light in lights)
        {
            if (light.type == LightType.Directional)
            {
                hasDirectionalLight = true;
                break;
            }
        }
        
        if (!hasDirectionalLight)
        {
            GameObject lightGO = new GameObject("Directional Light");
            Light light = lightGO.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.0f;
            lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }
        
        Debug.Log("✅ Lighting and camera fixed!");
    }

    static void FixSceneVisibility()
    {
        Debug.Log("🔧 Fixing scene visibility...");
        
        // Ensure scene view camera is positioned correctly
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            sceneView.pivot = Vector3.zero;
            sceneView.rotation = Quaternion.LookRotation(new Vector3(1, -1, 1));
            sceneView.size = 10f;
            sceneView.Repaint();
        }
        
        // Find and position main object if it exists
        GameObject mainObject = GameObject.Find(MAIN_OBJECT_NAME);
        if (mainObject != null && mainObject.transform.position.y < -100)
        {
            mainObject.transform.position = Vector3.zero;
            Debug.Log($"✅ {MAIN_OBJECT_NAME} repositioned to origin");
        }
        
        Debug.Log("✅ Scene visibility fixed!");
    }

    static void FixAllMaterials()
    {
        Debug.Log("🔧 Fixing all materials...");
        
        Material[] materials = Resources.FindObjectsOfTypeAll<Material>();
        int fixedCount = 0;
        
        foreach (Material material in materials)
        {
            if (material.shader.name.Contains("Standard") || 
                material.shader.name.Contains("Legacy") ||
                material.color == Color.magenta)
            {
                material.shader = Shader.Find("Universal Render Pipeline/Lit");
                fixedCount++;
            }
        }
        
        Debug.Log($"✅ Fixed {fixedCount} materials for URP compatibility!");
    }

    static void DiagnoseRenderPipeline()
    {
        Debug.Log("🔍 RENDER PIPELINE DIAGNOSIS:");
        
        var currentPipeline = GraphicsSettings.defaultRenderPipeline;
        if (currentPipeline == null)
        {
            Debug.Log("❌ No render pipeline assigned - using Built-in");
        }
        else if (currentPipeline is UniversalRenderPipelineAsset)
        {
            Debug.Log("✅ URP is properly assigned");
        }
        else
        {
            Debug.Log($"⚠️ Unknown pipeline: {currentPipeline.GetType().Name}");
        }
    }

    static void DiagnoseCamera()
    {
        Debug.Log("🔍 CAMERA DIAGNOSIS:");
        
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.Log("❌ No main camera found");
        }
        else
        {
            Debug.Log($"✅ Main camera found: {mainCamera.name}");
            Debug.Log($"   Position: {mainCamera.transform.position}");
            Debug.Log($"   FOV: {mainCamera.fieldOfView}");
        }
    }

    static void DiagnoseLighting()
    {
        Debug.Log("🔍 LIGHTING DIAGNOSIS:");
        
        Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        Debug.Log($"Found {lights.Length} lights in scene");
        
        bool hasDirectionalLight = false;
        foreach (Light light in lights)
        {
            Debug.Log($"   {light.name}: {light.type}, Intensity: {light.intensity}");
            if (light.type == LightType.Directional) hasDirectionalLight = true;
        }
        
        if (!hasDirectionalLight)
        {
            Debug.Log("⚠️ No directional light found - scene may be dark");
        }
    }

    static void DiagnoseMainObject()
    {
        Debug.Log($"🔍 {MAIN_OBJECT_NAME} DIAGNOSIS:");
        
        GameObject mainObject = GameObject.Find(MAIN_OBJECT_NAME);
        if (mainObject == null)
        {
            Debug.Log($"❌ {MAIN_OBJECT_NAME} not found in scene");
        }
        else
        {
            Debug.Log($"✅ {MAIN_OBJECT_NAME} found");
            Debug.Log($"   Position: {mainObject.transform.position}");
            Debug.Log($"   Active: {mainObject.activeInHierarchy}");
        }
    }

    static void DiagnoseMaterials()
    {
        Debug.Log("🔍 MATERIALS DIAGNOSIS:");
        
        Material[] materials = Resources.FindObjectsOfTypeAll<Material>();
        int urpCount = 0;
        int standardCount = 0;
        int magentaCount = 0;
        
        foreach (Material material in materials)
        {
            if (material.shader.name.Contains("Universal"))
                urpCount++;
            else if (material.shader.name.Contains("Standard") || material.shader.name.Contains("Legacy"))
                standardCount++;
            
            if (material.color == Color.magenta)
                magentaCount++;
        }
        
        Debug.Log($"   URP Materials: {urpCount}");
        Debug.Log($"   Non-URP Materials: {standardCount}");
        Debug.Log($"   Magenta Materials: {magentaCount}");
        
        if (standardCount > 0)
        {
            Debug.Log("⚠️ Non-URP materials found - may appear pink/magenta");
        }
    }

    static void CustomProjectFixes()
    {
        Debug.Log($"🎯 Running {PROJECT_NAME}-specific fixes...");
        
        // TreasureDragon specific fixes can be added here
        // Example: Position specific objects, configure dragon controller, etc.
        
        Debug.Log("✅ Custom project fixes complete!");
    }
    
    void ShowVegetationSlider(string label, string searchTerm, ref float percentage)
    {
        // Count current objects
        var objects = FindObjectsByType<GameObject>(FindObjectsSortMode.None)
            .Where(o => o.name.ToLower().Contains(searchTerm)).ToArray();
        
        GUILayout.BeginHorizontal();
        
        // Label and count
        GUILayout.Label($"{label} ({objects.Length} found)", GUILayout.Width(120));
        
        // Slider
        percentage = GUILayout.HorizontalSlider(percentage, 0f, 50f, GUILayout.Width(100));
        
        // Percentage label
        GUILayout.Label($"{percentage:F0}%", GUILayout.Width(35));
        
        // Remove button
        if (GUILayout.Button("Remove", GUILayout.Width(60)))
        {
            RemoveVegetationType(searchTerm, percentage / 100f);
        }
        
        GUILayout.EndHorizontal();
    }
    
    static void RemoveVegetationType(string searchTerm, float percentage)
    {
        var objects = FindObjectsByType<GameObject>(FindObjectsSortMode.None)
            .Where(o => o.name.ToLower().Contains(searchTerm)).ToArray();
        
        if (objects.Length == 0)
        {
            Debug.Log($"No {searchTerm} objects found in scene!");
            return;
        }
        
        int toRemove = Mathf.RoundToInt(objects.Length * percentage);
        
        if (toRemove == 0)
        {
            Debug.Log($"Percentage too low - no {searchTerm} objects to remove");
            return;
        }
        
        Debug.Log($"🌿 Removing {toRemove} {searchTerm} objects ({percentage * 100:F0}% of {objects.Length})");
        
        // Randomize selection
        var randomized = objects.OrderBy(x => System.Guid.NewGuid()).ToArray();
        
        for (int i = 0; i < toRemove; i++)
        {
            if (randomized[i] != null)
            {
                Debug.Log($"Destroying {searchTerm}: {randomized[i].name}");
                DestroyImmediate(randomized[i]);
            }
        }
        
        Debug.Log($"✅ Removed {toRemove} {searchTerm} objects!");
        
        // Mark scene dirty
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
} // Force recompile at Sun Aug  3 05:31:25 PM CDT 2025
