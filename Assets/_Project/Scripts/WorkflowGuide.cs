using UnityEngine;

namespace PlasmaDragon
{
    /// <summary>
    /// Simple workflow guide for modular scene development
    /// Shows best practices for Asset Store imports and prefab workflow
    /// </summary>
    public class WorkflowGuide : MonoBehaviour
    {
        [Header("Scene Workflow")]
        [SerializeField] private bool showWorkflowInConsole = true;
        
        void Start()
        {
            if (showWorkflowInConsole)
            {
                ShowWorkflowGuide();
            }
        }
        
        [ContextMenu("📋 Show Complete Workflow")]
        public void ShowWorkflowGuide()
        {
            Debug.Log("🏗️ === MODULAR DEVELOPMENT WORKFLOW ===");
            Debug.Log("");
            Debug.Log("📁 SCENE STRUCTURE:");
            Debug.Log("   🎮 MasterGame_Orchestrator → References everything");
            Debug.Log("   🐉 DragonScene_WorkSpace → Dragon + flight systems");
            Debug.Log("   🏞️ IslandBuildScene_001 → Environment building");
            Debug.Log("");
            Debug.Log("📦 ASSET STORE IMPORT STRATEGY:");
            Debug.Log("   ✅ Import ONLY into IslandBuildScene_001");
            Debug.Log("   ✅ Organize into ImportedAssets/[Category]/");
            Debug.Log("   ❌ NEVER import to Assets root or other scenes");
            Debug.Log("");
            Debug.Log("🔄 PREFAB WORKFLOW:");
            Debug.Log("   1. Build in dedicated scenes");
            Debug.Log("   2. Create prefabs from completed systems");
            Debug.Log("   3. Reference prefabs in Master scene");
            Debug.Log("   4. Changes auto-propagate through prefabs!");
            Debug.Log("");
            Debug.Log("💡 This keeps file sizes small and scenes modular!");
        }
        
        [ContextMenu("📦 Asset Store Best Practices")]
        public void ShowAssetStorePractices()
        {
            Debug.Log("📦 === ASSET STORE IMPORT BEST PRACTICES ===");
            Debug.Log("");
            Debug.Log("🎯 IMPORT LOCATION: IslandBuildScene_001 ONLY");
            Debug.Log("");
            Debug.Log("📂 FOLDER ORGANIZATION:");
            Debug.Log("   ImportedAssets/Dragon_Models/");
            Debug.Log("   ImportedAssets/Castle_Assets/");
            Debug.Log("   ImportedAssets/Environment_Packs/");
            Debug.Log("   ImportedAssets/Combat_Systems/");
            Debug.Log("   ImportedAssets/Effects_And_Particles/");
            Debug.Log("");
            Debug.Log("✅ IMPORT WORKFLOW:");
            Debug.Log("   1. Download Asset Store package");
            Debug.Log("   2. Open IslandBuildScene_001");
            Debug.Log("   3. Import package");
            Debug.Log("   4. Move to proper ImportedAssets folder");
            Debug.Log("   5. Build environment using assets");
            Debug.Log("   6. Create prefabs when section complete");
        }
        
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, Screen.height - 100, 300, 90));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("🏗️ WORKFLOW GUIDE ACTIVE");
            GUILayout.Label("Scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            
            if (GUILayout.Button("📋 Show Workflow"))
            {
                ShowWorkflowGuide();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}