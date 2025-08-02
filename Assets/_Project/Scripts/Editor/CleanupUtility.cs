using UnityEngine;
using UnityEditor;
using System.IO;

public class CleanupUtility : EditorWindow
{
    [MenuItem("Tools/Force Clean Library Issues")]
    public static void ForceCleanLibraryIssues()
    {
        Debug.Log("🧹 Starting forced cleanup of Library folder issues...");
        
        // Force refresh all assets
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        
        // Clear Unity's asset cache
        AssetDatabase.ForceReserializeAssets();
        
        // Reimport everything to clear any cached references
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        
        Debug.Log("✅ Forced cleanup completed. Library folder warnings should be resolved.");
        
        // Also run our material fixer while we're at it
        Debug.Log("🎨 Also fixing black materials...");
        MaterialFixer.FixBlackMaterials();
    }
    
    [MenuItem("Tools/Emergency Project Cleanup")]
    public static void EmergencyCleanup()
    {
        Debug.Log("🚨 EMERGENCY CLEANUP - Fixing all common Unity issues...");
        
        // Clean Library issues
        ForceCleanLibraryIssues();
        
        // Additional cleanup
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        
        Debug.Log("🎉 Emergency cleanup complete!");
    }
} 