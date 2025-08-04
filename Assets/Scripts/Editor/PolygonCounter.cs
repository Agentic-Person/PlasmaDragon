using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using System.Linq;

/// <summary>
/// Scene View Overlay that displays real-time polygon/triangle count
/// Perfect for monitoring file size optimization during development
/// </summary>
[Overlay(typeof(SceneView), "Polygon Counter", true)]
public class PolygonCounterOverlay : Overlay
{
    private static bool showTriangles = true;
    private static bool showVertices = false;
    private static int lastTriangleCount = 0;
    private static int lastVertexCount = 0;
    private static float lastUpdateTime = 0f;
    private const float UPDATE_INTERVAL = 0.5f; // Update every 0.5 seconds for performance

    public override void OnGUI()
    {
        // Update counts periodically for performance
        if (Time.realtimeSinceStartup - lastUpdateTime > UPDATE_INTERVAL)
        {
            UpdateCounts();
            lastUpdateTime = Time.realtimeSinceStartup;
        }

        GUILayout.BeginVertical("box", GUILayout.MinWidth(200));
        
        // Title
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        
        GUILayout.Label("🔺 Polygon Counter", titleStyle);
        
        GUILayout.Space(5);
        
        // Triangle count
        if (showTriangles)
        {
            GUIStyle triangleStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = GetColorForTriangleCount(lastTriangleCount) }
            };
            GUILayout.Label($"Triangles: {lastTriangleCount:N0}", triangleStyle);
        }
        
        // Vertex count
        if (showVertices)
        {
            GUILayout.Label($"Vertices: {lastVertexCount:N0}");
        }
        
        GUILayout.Space(5);
        
        // Performance indicator
        string perfIndicator = GetPerformanceIndicator(lastTriangleCount);
        GUIStyle perfStyle = new GUIStyle(GUI.skin.label)
        {
            fontStyle = FontStyle.Italic,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = GetColorForPerformance(lastTriangleCount) }
        };
        GUILayout.Label(perfIndicator, perfStyle);
        
        GUILayout.Space(5);
        
        // Toggle buttons
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button(showTriangles ? "Hide Triangles" : "Show Triangles", GUILayout.Height(20)))
        {
            showTriangles = !showTriangles;
        }
        
        if (GUILayout.Button(showVertices ? "Hide Vertices" : "Show Vertices", GUILayout.Height(20)))
        {
            showVertices = !showVertices;
        }
        
        GUILayout.EndHorizontal();
        
        // Force update button
        if (GUILayout.Button("🔄 Refresh Count", GUILayout.Height(25)))
        {
            UpdateCounts();
        }
        
        GUILayout.EndVertical();
    }

    private void UpdateCounts()
    {
        lastTriangleCount = 0;
        lastVertexCount = 0;
        
        // Get all mesh renderers in the scene
        MeshRenderer[] meshRenderers = Object.FindObjectsOfType<MeshRenderer>();
        
        foreach (MeshRenderer renderer in meshRenderers)
        {
            if (renderer.gameObject.activeInHierarchy)
            {
                MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    Mesh mesh = meshFilter.sharedMesh;
                    lastTriangleCount += mesh.triangles.Length / 3;
                    lastVertexCount += mesh.vertexCount;
                }
            }
        }
        
        // Also count skinned mesh renderers
        SkinnedMeshRenderer[] skinnedRenderers = Object.FindObjectsOfType<SkinnedMeshRenderer>();
        
        foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
        {
            if (renderer.gameObject.activeInHierarchy && renderer.sharedMesh != null)
            {
                Mesh mesh = renderer.sharedMesh;
                lastTriangleCount += mesh.triangles.Length / 3;
                lastVertexCount += mesh.vertexCount;
            }
        }
    }

    private Color GetColorForTriangleCount(int triangleCount)
    {
        if (triangleCount < 50000) return Color.green;        // Good performance
        if (triangleCount < 100000) return Color.yellow;      // Moderate performance
        if (triangleCount < 200000) return Color.red;         // High triangle count
        return Color.magenta;                                  // Very high triangle count
    }
    
    private Color GetColorForPerformance(int triangleCount)
    {
        if (triangleCount < 50000) return Color.green;
        if (triangleCount < 100000) return Color.yellow;
        return Color.red;
    }

    private string GetPerformanceIndicator(int triangleCount)
    {
        if (triangleCount < 25000) return "🟢 Excellent";
        if (triangleCount < 50000) return "🟡 Good";
        if (triangleCount < 100000) return "🟠 Moderate";
        if (triangleCount < 200000) return "🔴 High";
        return "🟣 Very High";
    }
}

/// <summary>
/// Menu items for Polygon Counter functionality
/// </summary>
public class PolygonCounterMenu
{
    [MenuItem("Tools/Dragon Toolkit/Polygon Counter/Toggle Scene Overlay", false, 100)]
    public static void TogglePolygonCounterOverlay()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            var overlay = sceneView.overlayCanvas.overlays.FirstOrDefault(o => o is PolygonCounterOverlay);
            if (overlay != null)
            {
                overlay.displayed = !overlay.displayed;
                sceneView.Repaint();
            }
        }
    }
    
    [MenuItem("Tools/Dragon Toolkit/Polygon Counter/Count Current Scene", false, 101)]
    public static void CountCurrentScene()
    {
        int triangleCount = 0;
        int vertexCount = 0;
        int meshCount = 0;
        
        // Count mesh renderers
        MeshRenderer[] meshRenderers = Object.FindObjectsOfType<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            if (renderer.gameObject.activeInHierarchy)
            {
                MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    Mesh mesh = meshFilter.sharedMesh;
                    triangleCount += mesh.triangles.Length / 3;
                    vertexCount += mesh.vertexCount;
                    meshCount++;
                }
            }
        }
        
        // Count skinned mesh renderers
        SkinnedMeshRenderer[] skinnedRenderers = Object.FindObjectsOfType<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
        {
            if (renderer.gameObject.activeInHierarchy && renderer.sharedMesh != null)
            {
                Mesh mesh = renderer.sharedMesh;
                triangleCount += mesh.triangles.Length / 3;
                vertexCount += mesh.vertexCount;
                meshCount++;
            }
        }
        
        string message = $"Scene Polygon Count:\n\n" +
                        $"🔺 Triangles: {triangleCount:N0}\n" +
                        $"📍 Vertices: {vertexCount:N0}\n" +
                        $"🎯 Mesh Objects: {meshCount:N0}\n\n" +
                        GetPerformanceMessage(triangleCount);
        
        EditorUtility.DisplayDialog("Polygon Counter", message, "OK");
        
        Debug.Log($"[Polygon Counter] Triangles: {triangleCount:N0}, Vertices: {vertexCount:N0}, Meshes: {meshCount:N0}");
    }
    
    private static string GetPerformanceMessage(int triangleCount)
    {
        if (triangleCount < 25000) return "🟢 Performance: Excellent for all platforms";
        if (triangleCount < 50000) return "🟡 Performance: Good for most platforms";
        if (triangleCount < 100000) return "🟠 Performance: Moderate - consider optimization";
        if (triangleCount < 200000) return "🔴 Performance: High - optimization recommended";
        return "🟣 Performance: Very High - optimization required";
    }
}
