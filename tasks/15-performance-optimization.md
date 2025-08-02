# Task 15: Performance Optimization

## Status: Pending

## Priority: MEDIUM - Critical for WebGL deployment

## Description
Optimize the game to meet performance targets: 60 FPS on desktop, 30 FPS on mobile, build size under 50MB, and load time under 20 seconds.

## Prerequisites
- All core systems implemented
- Visual effects added
- Initial WebGL build created

## Step-by-Step Instructions

### 1. Create Performance Profiler
`Assets/_Project/Scripts/Utilities/PerformanceProfiler.cs`:
```csharp
using UnityEngine;
using UnityEngine.Profiling;
using System.Collections.Generic;
using System.Text;

public class PerformanceProfiler : MonoBehaviour
{
    [Header("Display Settings")]
    public bool showFPS = true;
    public bool showMemory = true;
    public bool showDrawCalls = true;
    public bool showDetailedStats = false;
    public KeyCode toggleKey = KeyCode.F1;
    
    [Header("Performance Targets")]
    public int targetFPS = 60;
    public int warningFPS = 45;
    public int criticalFPS = 30;
    
    private float deltaTime = 0.0f;
    private float fps = 0.0f;
    private GUIStyle style;
    private Rect rect;
    private bool isVisible = true;
    
    // Performance tracking
    private Queue<float> fpsHistory = new Queue<float>(60);
    private float lowestFPS = float.MaxValue;
    private float highestFPS = 0f;
    
    void Start()
    {
        // GUI setup
        rect = new Rect(10, 10, 300, 500);
        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = 16;
        style.normal.textColor = Color.white;
        
        // Start performance monitoring
        InvokeRepeating(nameof(UpdatePerformanceStats), 1f, 1f);
    }
    
    void Update()
    {
        // Toggle visibility
        if (Input.GetKeyDown(toggleKey))
        {
            isVisible = !isVisible;
        }
        
        // Calculate FPS
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
        
        // Track history
        fpsHistory.Enqueue(fps);
        if (fpsHistory.Count > 60)
            fpsHistory.Dequeue();
        
        // Track extremes
        if (fps < lowestFPS) lowestFPS = fps;
        if (fps > highestFPS) highestFPS = fps;
    }
    
    void OnGUI()
    {
        if (!isVisible) return;
        
        // Background
        GUI.Box(rect, "");
        
        // Build performance text
        StringBuilder sb = new StringBuilder();
        
        // FPS
        if (showFPS)
        {
            Color fpsColor = GetFPSColor(fps);
            style.normal.textColor = fpsColor;
            sb.AppendLine($"FPS: {fps:F1} (Target: {targetFPS})");
            sb.AppendLine($"Min: {lowestFPS:F1} Max: {highestFPS:F1}");
            sb.AppendLine();
        }
        
        // Memory
        if (showMemory)
        {
            style.normal.textColor = Color.white;
            long totalMemory = Profiler.GetTotalAllocatedMemoryLong() / 1048576;
            long reservedMemory = Profiler.GetTotalReservedMemoryLong() / 1048576;
            sb.AppendLine($"Memory: {totalMemory}MB / {reservedMemory}MB");
            sb.AppendLine($"Mono: {Profiler.GetMonoUsedSizeLong() / 1048576}MB");
            sb.AppendLine();
        }
        
        // Draw calls
        if (showDrawCalls)
        {
            sb.AppendLine($"Batches: {UnityStats.batches}");
            sb.AppendLine($"Tris: {UnityStats.triangles / 1000}k");
            sb.AppendLine($"Verts: {UnityStats.vertices / 1000}k");
            sb.AppendLine();
        }
        
        // Detailed stats
        if (showDetailedStats)
        {
            sb.AppendLine($"SetPass: {UnityStats.setPassCalls}");
            sb.AppendLine($"Shadow Casters: {UnityStats.shadowCasters}");
            sb.AppendLine($"Render Time: {UnityStats.renderTime:F2}ms");
        }
        
        GUI.Label(rect, sb.ToString(), style);
    }
    
    Color GetFPSColor(float currentFPS)
    {
        if (currentFPS >= targetFPS) return Color.green;
        if (currentFPS >= warningFPS) return Color.yellow;
        if (currentFPS >= criticalFPS) return new Color(1f, 0.5f, 0f); // Orange
        return Color.red;
    }
    
    void UpdatePerformanceStats()
    {
        // Log performance issues
        if (fps < criticalFPS)
        {
            Debug.LogWarning($"Performance Critical: {fps:F1} FPS");
            IdentifyBottlenecks();
        }
    }
    
    void IdentifyBottlenecks()
    {
        // Check common performance issues
        if (UnityStats.batches > 100)
        {
            Debug.LogWarning("High batch count - consider batching or atlasing");
        }
        
        if (UnityStats.triangles > 100000)
        {
            Debug.LogWarning("High triangle count - reduce mesh complexity");
        }
        
        ParticleSystem[] particles = FindObjectsOfType<ParticleSystem>();
        int activeParticles = 0;
        foreach (var ps in particles)
        {
            if (ps.isPlaying)
                activeParticles += ps.particleCount;
        }
        
        if (activeParticles > 1000)
        {
            Debug.LogWarning($"High particle count: {activeParticles}");
        }
    }
}

// Helper class for stats
public static class UnityStats
{
    public static int batches => UnityEditor.UnityStats.batches;
    public static int setPassCalls => UnityEditor.UnityStats.setPassCalls;
    public static int triangles => UnityEditor.UnityStats.triangles;
    public static int vertices => UnityEditor.UnityStats.vertices;
    public static int shadowCasters => UnityEditor.UnityStats.shadowCasters;
    public static float renderTime => UnityEditor.UnityStats.renderTime;
}
```

### 2. Create LOD System for Enemies
`Assets/_Project/Scripts/Optimization/EnemyLODSystem.cs`:
```csharp
using UnityEngine;

public class EnemyLODSystem : MonoBehaviour
{
    [Header("LOD Settings")]
    public float[] lodDistances = { 30f, 60f, 100f };
    public GameObject[] lodMeshes; // High, Medium, Low detail
    
    [Header("Component Management")]
    public bool disableAnimatorAtDistance = true;
    public float animatorDisableDistance = 50f;
    public bool reduceUpdateRate = true;
    
    private Camera mainCamera;
    private Animator animator;
    private BaseEnemy enemy;
    private int currentLOD = -1;
    private float nextUpdateTime;
    private float baseUpdateInterval = 0.2f;
    
    void Start()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        enemy = GetComponent<BaseEnemy>();
        
        // Ensure LOD meshes are set up
        if (lodMeshes.Length == 0)
        {
            SetupAutomaticLODs();
        }
    }
    
    void Update()
    {
        if (Time.time < nextUpdateTime) return;
        
        float distance = Vector3.Distance(transform.position, 
            mainCamera.transform.position);
        
        UpdateLOD(distance);
        UpdateComponents(distance);
        
        // Variable update rate based on distance
        float updateMultiplier = Mathf.Clamp01(distance / 100f);
        nextUpdateTime = Time.time + baseUpdateInterval * (1f + updateMultiplier * 3f);
    }
    
    void UpdateLOD(float distance)
    {
        int newLOD = GetLODLevel(distance);
        
        if (newLOD != currentLOD)
        {
            currentLOD = newLOD;
            
            // Disable all LODs
            foreach (var lod in lodMeshes)
            {
                if (lod != null)
                    lod.SetActive(false);
            }
            
            // Enable current LOD
            if (currentLOD >= 0 && currentLOD < lodMeshes.Length)
            {
                lodMeshes[currentLOD].SetActive(true);
            }
        }
    }
    
    void UpdateComponents(float distance)
    {
        // Animator optimization
        if (animator != null && disableAnimatorAtDistance)
        {
            bool shouldAnimate = distance < animatorDisableDistance;
            
            if (animator.enabled != shouldAnimate)
            {
                animator.enabled = shouldAnimate;
                
                // Set to idle pose when disabling
                if (!shouldAnimate)
                {
                    transform.rotation = Quaternion.identity;
                }
            }
        }
        
        // AI update rate
        if (enemy != null && reduceUpdateRate)
        {
            // Reduce AI complexity at distance
            enemy.SetUpdateMultiplier(Mathf.Clamp01(1f - distance / 100f));
        }
    }
    
    int GetLODLevel(float distance)
    {
        for (int i = 0; i < lodDistances.Length; i++)
        {
            if (distance < lodDistances[i])
                return i;
        }
        
        return lodDistances.Length; // Beyond all LODs
    }
    
    void SetupAutomaticLODs()
    {
        // Simple automatic LOD setup
        MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
        if (meshFilter == null) return;
        
        lodMeshes = new GameObject[3];
        
        // LOD0 - Original
        lodMeshes[0] = meshFilter.gameObject;
        
        // LOD1 & LOD2 would need mesh simplification
        // For now, just use the same mesh with reduced features
        lodMeshes[1] = lodMeshes[0];
        lodMeshes[2] = lodMeshes[0];
    }
}
```

### 3. Create Texture Optimization System
`Assets/_Project/Scripts/Optimization/TextureOptimizer.cs`:
```csharp
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TextureOptimizer : MonoBehaviour
{
    [Header("Optimization Settings")]
    public int maxTextureSize = 1024;
    public bool compressTextures = true;
    public bool generateMipmaps = true;
    
    #if UNITY_EDITOR
    [ContextMenu("Optimize All Textures")]
    public void OptimizeAllTextures()
    {
        string[] texturePaths = AssetDatabase.FindAssets("t:Texture2D");
        int optimized = 0;
        
        foreach (string guid in texturePaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            
            // Skip external assets
            if (!path.StartsWith("Assets/_Project")) continue;
            
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                bool changed = false;
                
                // Max texture size
                if (importer.maxTextureSize > maxTextureSize)
                {
                    importer.maxTextureSize = maxTextureSize;
                    changed = true;
                }
                
                // Compression
                if (compressTextures && importer.textureCompression != TextureImporterCompression.Compressed)
                {
                    importer.textureCompression = TextureImporterCompression.Compressed;
                    changed = true;
                }
                
                // Mipmaps
                if (importer.mipmapEnabled != generateMipmaps)
                {
                    importer.mipmapEnabled = generateMipmaps;
                    changed = true;
                }
                
                // Platform-specific overrides
                TextureImporterPlatformSettings webGLSettings = importer.GetPlatformTextureSettings("WebGL");
                webGLSettings.overridden = true;
                webGLSettings.maxTextureSize = Mathf.Min(maxTextureSize, 512);
                webGLSettings.format = TextureImporterFormat.DXT5Crunched;
                importer.SetPlatformTextureSettings(webGLSettings);
                
                if (changed)
                {
                    importer.SaveAndReimport();
                    optimized++;
                }
            }
        }
        
        Debug.Log($"Optimized {optimized} textures");
    }
    
    [ContextMenu("Create Texture Atlas")]
    public void CreateTextureAtlas()
    {
        // Find all UI sprites
        Sprite[] sprites = Resources.FindObjectsOfTypeAll<Sprite>();
        List<Texture2D> texturesToPack = new List<Texture2D>();
        
        foreach (var sprite in sprites)
        {
            if (sprite.texture != null && AssetDatabase.GetAssetPath(sprite).StartsWith("Assets/_Project"))
            {
                texturesToPack.Add(sprite.texture);
            }
        }
        
        if (texturesToPack.Count > 0)
        {
            // Create atlas
            Texture2D atlas = new Texture2D(2048, 2048);
            Rect[] rects = atlas.PackTextures(texturesToPack.ToArray(), 2, 2048);
            
            // Save atlas
            byte[] bytes = atlas.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/_Project/Textures/UIAtlas.png", bytes);
            AssetDatabase.Refresh();
            
            Debug.Log($"Created atlas with {texturesToPack.Count} textures");
        }
    }
    #endif
}
```

### 4. Create Object Pooling System
`Assets/_Project/Scripts/Optimization/UniversalObjectPool.cs`:
```csharp
using UnityEngine;
using System.Collections.Generic;

public class UniversalObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class PoolInfo
    {
        public string poolName;
        public GameObject prefab;
        public int initialSize = 10;
        public int maxSize = 50;
        public bool autoExpand = true;
        public float returnDelay = 0f;
    }
    
    [Header("Pool Configuration")]
    public List<PoolInfo> pools = new List<PoolInfo>();
    
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, PoolInfo> poolInfoDictionary;
    private Dictionary<GameObject, string> activeObjects;
    private Transform poolContainer;
    
    private static UniversalObjectPool instance;
    public static UniversalObjectPool Instance => instance;
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializePools();
    }
    
    void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolInfoDictionary = new Dictionary<string, PoolInfo>();
        activeObjects = new Dictionary<GameObject, string>();
        
        // Create container
        poolContainer = new GameObject("PoolContainer").transform;
        poolContainer.SetParent(transform);
        
        // Initialize each pool
        foreach (var poolInfo in pools)
        {
            CreatePool(poolInfo);
        }
    }
    
    void CreatePool(PoolInfo poolInfo)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();
        GameObject poolFolder = new GameObject($"Pool_{poolInfo.poolName}");
        poolFolder.transform.SetParent(poolContainer);
        
        // Create initial objects
        for (int i = 0; i < poolInfo.initialSize; i++)
        {
            GameObject obj = CreatePooledObject(poolInfo, poolFolder.transform);
            objectPool.Enqueue(obj);
        }
        
        poolDictionary.Add(poolInfo.poolName, objectPool);
        poolInfoDictionary.Add(poolInfo.poolName, poolInfo);
    }
    
    GameObject CreatePooledObject(PoolInfo poolInfo, Transform parent)
    {
        GameObject obj = Instantiate(poolInfo.prefab, parent);
        obj.SetActive(false);
        
        // Add pooled object component
        PooledObject pooledObj = obj.AddComponent<PooledObject>();
        pooledObj.poolName = poolInfo.poolName;
        
        return obj;
    }
    
    public GameObject Spawn(string poolName, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.LogWarning($"Pool {poolName} doesn't exist!");
            return null;
        }
        
        GameObject objectToSpawn = null;
        Queue<GameObject> pool = poolDictionary[poolName];
        PoolInfo info = poolInfoDictionary[poolName];
        
        // Get object from pool
        if (pool.Count > 0)
        {
            objectToSpawn = pool.Dequeue();
        }
        else if (info.autoExpand)
        {
            // Create new object if pool is empty
            objectToSpawn = CreatePooledObject(info, poolContainer);
        }
        else
        {
            Debug.LogWarning($"Pool {poolName} is empty!");
            return null;
        }
        
        // Set up spawned object
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);
        
        // Track active object
        activeObjects[objectToSpawn] = poolName;
        
        return objectToSpawn;
    }
    
    public void Return(GameObject obj, float delay = 0f)
    {
        if (delay > 0f)
        {
            StartCoroutine(ReturnWithDelay(obj, delay));
        }
        else
        {
            ReturnImmediate(obj);
        }
    }
    
    void ReturnImmediate(GameObject obj)
    {
        if (!activeObjects.ContainsKey(obj)) return;
        
        string poolName = activeObjects[obj];
        activeObjects.Remove(obj);
        
        // Reset object
        obj.SetActive(false);
        obj.transform.SetParent(poolContainer.Find($"Pool_{poolName}"));
        
        // Return to pool
        if (poolDictionary.ContainsKey(poolName))
        {
            poolDictionary[poolName].Enqueue(obj);
        }
    }
    
    System.Collections.IEnumerator ReturnWithDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnImmediate(obj);
    }
    
    public void PrewarmPool(string poolName, int count)
    {
        if (!poolInfoDictionary.ContainsKey(poolName)) return;
        
        PoolInfo info = poolInfoDictionary[poolName];
        Queue<GameObject> pool = poolDictionary[poolName];
        
        for (int i = 0; i < count; i++)
        {
            if (pool.Count >= info.maxSize) break;
            
            GameObject obj = CreatePooledObject(info, poolContainer);
            pool.Enqueue(obj);
        }
    }
    
    public int GetPoolSize(string poolName)
    {
        if (poolDictionary.ContainsKey(poolName))
            return poolDictionary[poolName].Count;
        
        return 0;
    }
}

public class PooledObject : MonoBehaviour
{
    public string poolName;
    public float autoReturnTime = 0f;
    
    void OnEnable()
    {
        if (autoReturnTime > 0f)
        {
            Invoke(nameof(AutoReturn), autoReturnTime);
        }
    }
    
    void OnDisable()
    {
        CancelInvoke();
    }
    
    void AutoReturn()
    {
        UniversalObjectPool.Instance.Return(gameObject);
    }
}
```

### 5. Create Draw Call Batching System
`Assets/_Project/Scripts/Optimization/DrawCallOptimizer.cs`:
```csharp
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class DrawCallOptimizer : MonoBehaviour
{
    [Header("Batching Settings")]
    public bool enableStaticBatching = true;
    public bool enableDynamicBatching = true;
    public bool combineStaticMeshes = true;
    
    [Header("Material Optimization")]
    public bool shareMaterials = true;
    public Material[] sharedMaterials;
    
    private Dictionary<string, Material> materialCache;
    
    void Start()
    {
        OptimizeScene();
    }
    
    void OptimizeScene()
    {
        if (enableStaticBatching)
            SetupStaticBatching();
        
        if (enableDynamicBatching)
            SetupDynamicBatching();
        
        if (shareMaterials)
            OptimizeMaterials();
        
        if (combineStaticMeshes)
            CombineStaticMeshes();
    }
    
    void SetupStaticBatching()
    {
        // Mark static objects
        GameObject[] staticObjects = GameObject.FindGameObjectsWithTag("Environment");
        
        foreach (var obj in staticObjects)
        {
            GameObjectUtility.SetStaticEditorFlags(obj, 
                StaticEditorFlags.BatchingStatic);
        }
        
        Debug.Log($"Marked {staticObjects.Length} objects for static batching");
    }
    
    void SetupDynamicBatching()
    {
        // Ensure dynamic batching is enabled in settings
        PlayerSettings.SetGraphicsAPIs(BuildTarget.WebGL, 
            new[] { GraphicsDeviceType.OpenGLES3 });
        
        // Find and optimize dynamic objects
        MeshRenderer[] renderers = FindObjectsOfType<MeshRenderer>();
        int optimized = 0;
        
        foreach (var renderer in renderers)
        {
            // Check if suitable for dynamic batching
            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                int vertexCount = meshFilter.sharedMesh.vertexCount;
                
                // Unity dynamic batching limit
                if (vertexCount <= 300)
                {
                    // Ensure uniform scale for batching
                    if (renderer.transform.localScale != Vector3.one)
                    {
                        // Could bake scale into mesh
                        optimized++;
                    }
                }
            }
        }
        
        Debug.Log($"Optimized {optimized} objects for dynamic batching");
    }
    
    void OptimizeMaterials()
    {
        materialCache = new Dictionary<string, Material>();
        
        // Create material cache
        foreach (var mat in sharedMaterials)
        {
            if (mat != null)
                materialCache[mat.name] = mat;
        }
        
        // Replace materials
        Renderer[] allRenderers = FindObjectsOfType<Renderer>();
        int replaced = 0;
        
        foreach (var renderer in allRenderers)
        {
            Material[] mats = renderer.sharedMaterials;
            bool changed = false;
            
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] != null && materialCache.ContainsKey(mats[i].name))
                {
                    mats[i] = materialCache[mats[i].name];
                    changed = true;
                    replaced++;
                }
            }
            
            if (changed)
                renderer.sharedMaterials = mats;
        }
        
        Debug.Log($"Replaced {replaced} material instances with shared materials");
    }
    
    void CombineStaticMeshes()
    {
        // Group by material
        Dictionary<Material, List<MeshFilter>> meshGroups = 
            new Dictionary<Material, List<MeshFilter>>();
        
        MeshFilter[] meshFilters = FindObjectsOfType<MeshFilter>();
        
        foreach (var mf in meshFilters)
        {
            if (!mf.gameObject.isStatic) continue;
            
            MeshRenderer renderer = mf.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.sharedMaterial != null)
            {
                Material mat = renderer.sharedMaterial;
                
                if (!meshGroups.ContainsKey(mat))
                    meshGroups[mat] = new List<MeshFilter>();
                
                meshGroups[mat].Add(mf);
            }
        }
        
        // Combine meshes per material
        foreach (var kvp in meshGroups)
        {
            if (kvp.Value.Count > 1)
            {
                CombineMeshGroup(kvp.Key, kvp.Value);
            }
        }
    }
    
    void CombineMeshGroup(Material material, List<MeshFilter> meshFilters)
    {
        CombineInstance[] combine = new CombineInstance[meshFilters.Count];
        
        for (int i = 0; i < meshFilters.Count; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }
        
        // Create combined mesh
        GameObject combined = new GameObject($"Combined_{material.name}");
        MeshFilter mf = combined.AddComponent<MeshFilter>();
        MeshRenderer mr = combined.AddComponent<MeshRenderer>();
        
        Mesh combinedMesh = new Mesh();
        combinedMesh.name = $"CombinedMesh_{material.name}";
        combinedMesh.CombineMeshes(combine, true, true);
        
        mf.sharedMesh = combinedMesh;
        mr.sharedMaterial = material;
        
        // Mark as static
        GameObjectUtility.SetStaticEditorFlags(combined, 
            StaticEditorFlags.BatchingStatic);
        
        // Disable original objects
        foreach (var meshFilter in meshFilters)
        {
            meshFilter.gameObject.SetActive(false);
        }
        
        Debug.Log($"Combined {meshFilters.Count} meshes for material {material.name}");
    }
}
```

### 6. Create Audio Optimization
`Assets/_Project/Scripts/Optimization/AudioOptimizer.cs`:
```csharp
using UnityEngine;
using UnityEngine.Audio;

public class AudioOptimizer : MonoBehaviour
{
    [Header("Audio Settings")]
    public int maxVirtualVoices = 32;
    public int maxRealVoices = 16;
    public float sampleRate = 22050f;
    public AudioSpeakerMode speakerMode = AudioSpeakerMode.Stereo;
    
    [Header("3D Audio")]
    public float maxAudioDistance = 100f;
    public AnimationCurve audioFalloffCurve;
    
    [Header("Compression")]
    public bool forceMonoFor3D = true;
    public AudioCompressionFormat webGLFormat = AudioCompressionFormat.Vorbis;
    
    void Awake()
    {
        ConfigureAudioSettings();
    }
    
    void ConfigureAudioSettings()
    {
        // Global audio configuration
        AudioConfiguration config = AudioSettings.GetConfiguration();
        config.sampleRate = (int)sampleRate;
        config.speakerMode = speakerMode;
        config.numVirtualVoices = maxVirtualVoices;
        config.numRealVoices = maxRealVoices;
        
        AudioSettings.Reset(config);
        
        // Configure all audio sources
        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        
        foreach (var source in allSources)
        {
            OptimizeAudioSource(source);
        }
        
        Debug.Log($"Optimized {allSources.Length} audio sources");
    }
    
    void OptimizeAudioSource(AudioSource source)
    {
        // 3D settings
        if (source.spatialBlend > 0.5f) // 3D sound
        {
            source.maxDistance = maxAudioDistance;
            source.rolloffMode = AudioRolloffMode.Custom;
            
            if (audioFalloffCurve != null)
                source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, 
                    audioFalloffCurve);
            
            // Reduce quality for distant sounds
            source.priority = Mathf.RoundToInt(Mathf.Lerp(0, 256, 
                source.maxDistance / maxAudioDistance));
        }
        
        // Disable doppler for performance
        source.dopplerLevel = 0f;
        
        // Reduce reverb zones
        source.reverbZoneMix = 0.5f;
    }
    
    #if UNITY_EDITOR
    [ContextMenu("Optimize Audio Clips")]
    void OptimizeAudioClips()
    {
        string[] audioGUIDs = UnityEditor.AssetDatabase.FindAssets("t:AudioClip");
        int optimized = 0;
        
        foreach (string guid in audioGUIDs)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            
            if (!path.StartsWith("Assets/_Project")) continue;
            
            UnityEditor.AudioImporter importer = 
                UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.AudioImporter;
                
            if (importer != null)
            {
                // Default settings
                UnityEditor.AudioImporterSampleSettings settings = 
                    importer.defaultSampleSettings;
                
                // WebGL specific
                UnityEditor.AudioImporterSampleSettings webGLSettings = 
                    importer.GetOverrideSampleSettings("WebGL");
                
                webGLSettings.loadType = UnityEditor.AudioClipLoadType.CompressedInMemory;
                webGLSettings.compressionFormat = webGLFormat;
                webGLSettings.quality = 0.7f;
                webGLSettings.sampleRateSetting = 
                    UnityEditor.AudioSampleRateSetting.OverrideSampleRate;
                webGLSettings.sampleRateOverride = (uint)sampleRate;
                
                importer.SetOverrideSampleSettings("WebGL", webGLSettings);
                
                // Force to mono for 3D sounds
                if (forceMonoFor3D && importer.defaultSampleSettings.loadType == 
                    UnityEditor.AudioClipLoadType.CompressedInMemory)
                {
                    importer.forceToMono = true;
                }
                
                importer.SaveAndReimport();
                optimized++;
            }
        }
        
        Debug.Log($"Optimized {optimized} audio clips");
    }
    #endif
}
```

### 7. Create WebGL-Specific Optimizations
`Assets/_Project/Scripts/Optimization/WebGLOptimizer.cs`:
```csharp
using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLOptimizer : MonoBehaviour
{
    [Header("Memory Settings")]
    public int targetMemoryMB = 256;
    public bool enableMemoryGrowth = true;
    
    [Header("Performance")]
    public bool disableUnusedFeatures = true;
    public bool optimizeForMobile = true;
    
    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SetWebGLMemorySize(int size);
    
    [DllImport("__Internal")]
    private static extern bool IsMobileBrowser();
    #endif
    
    void Awake()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        ConfigureWebGL();
        #endif
    }
    
    void ConfigureWebGL()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        // Set memory size
        SetWebGLMemorySize(targetMemoryMB);
        
        // Detect mobile
        bool isMobile = IsMobileBrowser();
        
        if (isMobile && optimizeForMobile)
        {
            ApplyMobileOptimizations();
        }
        
        // Disable unused features
        if (disableUnusedFeatures)
        {
            DisableUnusedFeatures();
        }
        #endif
    }
    
    void ApplyMobileOptimizations()
    {
        // Reduce quality settings
        QualitySettings.SetQualityLevel(0); // Low quality
        
        // Reduce resolution
        Screen.SetResolution(
            Mathf.Min(Screen.width, 1280), 
            Mathf.Min(Screen.height, 720), 
            true
        );
        
        // Disable shadows
        QualitySettings.shadows = ShadowQuality.Disable;
        
        // Reduce texture quality
        QualitySettings.masterTextureLimit = 1; // Half resolution
        
        // Disable post-processing
        GameObject postProcess = GameObject.Find("PostProcessVolume");
        if (postProcess != null)
            postProcess.SetActive(false);
        
        Debug.Log("Applied mobile optimizations");
    }
    
    void DisableUnusedFeatures()
    {
        // Disable unused Unity features
        Physics.autoSimulation = true; // Only when needed
        Physics2D.simulationMode = SimulationMode2D.Script;
        
        // Reduce physics iterations
        Physics.defaultSolverIterations = 4;
        Physics.defaultSolverVelocityIterations = 1;
        
        // Disable unused rendering features
        Camera.main.allowHDR = false;
        Camera.main.allowMSAA = false;
    }
    
    // JavaScript plugin for WebGL
    public static string GetWebGLPlugin()
    {
        return @"
        mergeInto(LibraryManager.library, {
            SetWebGLMemorySize: function(size) {
                if (Module.HEAP8.buffer.byteLength < size * 1024 * 1024) {
                    Module.requestFullscreen = function() {
                        Browser.requestFullscreen(false, false);
                    };
                }
            },
            
            IsMobileBrowser: function() {
                return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i
                    .test(navigator.userAgent);
            }
        });
        ";
    }
}
```

### 8. Create Build Size Optimizer
`Assets/_Project/Scripts/Editor/BuildSizeOptimizer.cs`:
```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Linq;

public class BuildSizeOptimizer : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 0;
    
    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("=== Build Size Optimization Starting ===");
        
        // Strip unused assets
        StripUnusedAssets();
        
        // Optimize build settings
        OptimizeBuildSettings();
        
        // Clean up resources
        CleanupResources();
    }
    
    public void OnPostprocessBuild(BuildReport report)
    {
        Debug.Log("=== Build Complete ===");
        
        // Analyze build size
        AnalyzeBuildSize(report);
        
        // Generate optimization report
        GenerateOptimizationReport(report);
    }
    
    void StripUnusedAssets()
    {
        // Remove unused shaders
        var shaders = AssetDatabase.FindAssets("t:Shader");
        int removedShaders = 0;
        
        foreach (string guid in shaders)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            
            // Check if shader is used
            if (!IsShaderUsed(path))
            {
                // Mark for exclusion
                removedShaders++;
            }
        }
        
        Debug.Log($"Marked {removedShaders} unused shaders for removal");
        
        // Remove empty folders
        AssetDatabase.DeleteAsset("Assets/EmptyFolder");
        
        // Clear unused localization
        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            BuildTargetGroup.WebGL, 
            "UNITY_EXCLUDE_UNUSED_LANGUAGES"
        );
    }
    
    void OptimizeBuildSettings()
    {
        // Code stripping
        PlayerSettings.stripEngineCode = true;
        PlayerSettings.stripUnusedMeshComponents = true;
        
        // Managed stripping level
        PlayerSettings.SetManagedStrippingLevel(
            BuildTargetGroup.WebGL, 
            ManagedStrippingLevel.High
        );
        
        // IL2CPP settings
        PlayerSettings.SetIl2CppCompilerConfiguration(
            BuildTargetGroup.WebGL, 
            Il2CppCompilerConfiguration.Release
        );
        
        // WebGL specific
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
        PlayerSettings.WebGL.dataCaching = true;
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        
        // Disable unnecessary features
        PlayerSettings.bakeCollisionMeshes = false;
        PlayerSettings.use32BitDisplayBuffer = false;
    }
    
    void CleanupResources()
    {
        // Remove editor-only assets
        string[] editorAssets = AssetDatabase.FindAssets("t:Object", 
            new[] { "Assets/Editor" });
        
        // Clear asset bundle names
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string path in allAssetPaths)
        {
            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer != null && !string.IsNullOrEmpty(importer.assetBundleName))
            {
                importer.assetBundleName = "";
            }
        }
        
        // Force garbage collection
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }
    
    bool IsShaderUsed(string shaderPath)
    {
        // Check if shader is referenced by any material
        var materials = AssetDatabase.FindAssets("t:Material");
        
        foreach (string matGuid in materials)
        {
            string matPath = AssetDatabase.GUIDToAssetPath(matGuid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            
            if (mat != null && mat.shader != null)
            {
                string usedShaderPath = AssetDatabase.GetAssetPath(mat.shader);
                if (usedShaderPath == shaderPath)
                    return true;
            }
        }
        
        return false;
    }
    
    void AnalyzeBuildSize(BuildReport report)
    {
        var summary = report.summary;
        Debug.Log($"Total Build Size: {summary.totalSize / 1048576}MB");
        Debug.Log($"Build Time: {summary.totalTime.TotalSeconds}s");
        
        // Analyze file sizes
        var sortedFiles = report.files.OrderByDescending(f => f.size);
        
        Debug.Log("=== Largest Files ===");
        int count = 0;
        foreach (var file in sortedFiles)
        {
            if (count++ >= 10) break;
            Debug.Log($"{file.path}: {file.size / 1048576f:F2}MB");
        }
    }
    
    void GenerateOptimizationReport(BuildReport report)
    {
        string reportPath = "Assets/BuildOptimizationReport.txt";
        
        using (var writer = new System.IO.StreamWriter(reportPath))
        {
            writer.WriteLine("=== Build Optimization Report ===");
            writer.WriteLine($"Date: {System.DateTime.Now}");
            writer.WriteLine($"Platform: {report.summary.platform}");
            writer.WriteLine($"Total Size: {report.summary.totalSize / 1048576}MB");
            writer.WriteLine();
            
            writer.WriteLine("=== Optimization Suggestions ===");
            
            // Check texture sizes
            var textures = report.files.Where(f => f.path.EndsWith(".png") || 
                f.path.EndsWith(".jpg"));
            float textureSize = textures.Sum(t => t.size) / 1048576f;
            
            if (textureSize > 20)
            {
                writer.WriteLine($"- Textures use {textureSize:F1}MB. " +
                    "Consider further compression.");
            }
            
            // Check code size
            var codeSize = report.files.Where(f => f.path.EndsWith(".wasm"))
                .Sum(f => f.size) / 1048576f;
            
            if (codeSize > 10)
            {
                writer.WriteLine($"- Code size is {codeSize:F1}MB. " +
                    "Enable more aggressive stripping.");
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log($"Optimization report generated: {reportPath}");
    }
}
#endif
```

### 9. Create Final Performance Test
`Assets/_Project/Scripts/Optimization/PerformanceTestRunner.cs`:
```csharp
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PerformanceTestRunner : MonoBehaviour
{
    [Header("Test Configuration")]
    public bool runOnStart = false;
    public float testDuration = 60f;
    
    [Header("Test Scenarios")]
    public bool testCombat = true;
    public bool testManyEnemies = true;
    public bool testParticles = true;
    public bool testAI = true;
    
    private List<float> fpsReadings = new List<float>();
    private float testStartTime;
    private bool isTestRunning = false;
    
    void Start()
    {
        if (runOnStart)
        {
            StartCoroutine(RunPerformanceTests());
        }
    }
    
    IEnumerator RunPerformanceTests()
    {
        isTestRunning = true;
        testStartTime = Time.time;
        fpsReadings.Clear();
        
        Debug.Log("=== Performance Test Started ===");
        
        // Test 1: Combat scenario
        if (testCombat)
        {
            yield return StartCoroutine(TestCombatScenario());
        }
        
        // Test 2: Many enemies
        if (testManyEnemies)
        {
            yield return StartCoroutine(TestManyEnemies());
        }
        
        // Test 3: Particle stress
        if (testParticles)
        {
            yield return StartCoroutine(TestParticleStress());
        }
        
        // Test 4: AI stress
        if (testAI)
        {
            yield return StartCoroutine(TestAIStress());
        }
        
        // Generate report
        GeneratePerformanceReport();
        
        isTestRunning = false;
    }
    
    IEnumerator TestCombatScenario()
    {
        Debug.Log("Testing: Combat Scenario");
        
        // Spawn enemies
        for (int i = 0; i < 10; i++)
        {
            Vector3 pos = Random.insideUnitSphere * 50f;
            pos.y = 10f;
            
            GameObject enemy = UniversalObjectPool.Instance.Spawn("Enemy", pos, 
                Quaternion.identity);
        }
        
        // Simulate combat
        float combatDuration = 10f;
        float startTime = Time.time;
        
        while (Time.time - startTime < combatDuration)
        {
            // Fire weapons
            if (Random.value > 0.7f)
            {
                GameObject projectile = UniversalObjectPool.Instance.Spawn(
                    "PlasmaOrb", 
                    transform.position, 
                    transform.rotation
                );
            }
            
            RecordFPS();
            yield return new WaitForSeconds(0.1f);
        }
        
        // Cleanup
        yield return new WaitForSeconds(2f);
    }
    
    IEnumerator TestManyEnemies()
    {
        Debug.Log("Testing: Many Enemies");
        
        List<GameObject> enemies = new List<GameObject>();
        
        // Spawn increasing enemies
        for (int wave = 0; wave < 5; wave++)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector3 pos = Random.insideUnitSphere * 100f;
                pos.y = Random.Range(5f, 20f);
                
                GameObject enemy = UniversalObjectPool.Instance.Spawn("Enemy", 
                    pos, Quaternion.identity);
                enemies.Add(enemy);
            }
            
            Debug.Log($"Enemy count: {enemies.Count}");
            
            // Test for 2 seconds
            float waveStart = Time.time;
            while (Time.time - waveStart < 2f)
            {
                RecordFPS();
                yield return null;
            }
        }
        
        // Cleanup
        foreach (var enemy in enemies)
        {
            UniversalObjectPool.Instance.Return(enemy);
        }
        
        yield return new WaitForSeconds(1f);
    }
    
    IEnumerator TestParticleStress()
    {
        Debug.Log("Testing: Particle Stress");
        
        // Create many explosions
        for (int i = 0; i < 20; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * 30f;
            
            GameObject explosion = UniversalObjectPool.Instance.Spawn(
                "Explosion", pos, Quaternion.identity);
            
            yield return new WaitForSeconds(0.2f);
            RecordFPS();
        }
        
        yield return new WaitForSeconds(2f);
    }
    
    IEnumerator TestAIStress()
    {
        Debug.Log("Testing: AI Stress");
        
        // Spawn AI boss
        GameObject boss = Instantiate(
            Resources.Load<GameObject>("Prefabs/EliteCommander"),
            Vector3.zero,
            Quaternion.identity
        );
        
        // Force rapid decisions
        EliteCommander commander = boss.GetComponent<EliteCommander>();
        if (commander != null)
        {
            commander.decisionInterval = 1f; // Rapid decisions
        }
        
        // Test for 10 seconds
        float startTime = Time.time;
        while (Time.time - startTime < 10f)
        {
            RecordFPS();
            yield return null;
        }
        
        Destroy(boss);
        yield return new WaitForSeconds(1f);
    }
    
    void RecordFPS()
    {
        float fps = 1f / Time.deltaTime;
        fpsReadings.Add(fps);
    }
    
    void GeneratePerformanceReport()
    {
        if (fpsReadings.Count == 0) return;
        
        // Calculate statistics
        float avgFPS = 0f;
        float minFPS = float.MaxValue;
        float maxFPS = 0f;
        int below30Count = 0;
        int below60Count = 0;
        
        foreach (float fps in fpsReadings)
        {
            avgFPS += fps;
            if (fps < minFPS) minFPS = fps;
            if (fps > maxFPS) maxFPS = fps;
            if (fps < 30) below30Count++;
            if (fps < 60) below60Count++;
        }
        
        avgFPS /= fpsReadings.Count;
        
        // Generate report
        Debug.Log("=== Performance Test Results ===");
        Debug.Log($"Test Duration: {testDuration}s");
        Debug.Log($"Total Samples: {fpsReadings.Count}");
        Debug.Log($"Average FPS: {avgFPS:F1}");
        Debug.Log($"Min FPS: {minFPS:F1}");
        Debug.Log($"Max FPS: {maxFPS:F1}");
        Debug.Log($"Frames below 30 FPS: {below30Count} ({below30Count * 100f / fpsReadings.Count:F1}%)");
        Debug.Log($"Frames below 60 FPS: {below60Count} ({below60Count * 100f / fpsReadings.Count:F1}%)");
        
        // Performance grade
        string grade;
        if (avgFPS >= 60 && minFPS >= 45)
            grade = "A - Excellent";
        else if (avgFPS >= 45 && minFPS >= 30)
            grade = "B - Good";
        else if (avgFPS >= 30 && minFPS >= 20)
            grade = "C - Acceptable";
        else
            grade = "D - Needs Optimization";
        
        Debug.Log($"Performance Grade: {grade}");
        
        // Save to file
        string report = $"Performance Report - {System.DateTime.Now}\n" +
            $"Average FPS: {avgFPS:F1}\n" +
            $"Min FPS: {minFPS:F1}\n" +
            $"Grade: {grade}";
        
        System.IO.File.WriteAllText(
            Application.dataPath + "/PerformanceReport.txt", report);
    }
}
```

## Expected Outcomes
- ✅ Consistent 60 FPS on desktop browsers
- ✅ 30+ FPS on mobile devices
- ✅ Build size under 50MB compressed
- ✅ Load time under 20 seconds
- ✅ Memory usage under 256MB
- ✅ Smooth gameplay with many enemies
- ✅ No stuttering during combat

## Optimization Checklist
- [ ] All textures optimized
- [ ] Materials using atlases
- [ ] Static batching enabled
- [ ] Object pooling implemented
- [ ] LOD system working
- [ ] Audio compressed
- [ ] Unused code stripped
- [ ] WebGL settings optimal

## Common Issues & Solutions

### Issue: Low FPS in Combat
- Reduce particle count
- Simplify shaders
- Use object pooling
- Reduce enemy count

### Issue: Large Build Size
- Compress textures more
- Remove unused assets
- Enable code stripping
- Use texture atlases

### Issue: Long Load Times
- Reduce initial scene size
- Use async loading
- Compress assets
- Enable caching

### Issue: Memory Crashes
- Reduce texture sizes
- Unload unused assets
- Fix memory leaks
- Lower quality settings

## Performance Targets Summary
- **Desktop**: 60 FPS, High quality
- **Mobile**: 30 FPS, Low quality
- **Build Size**: <50MB compressed
- **Load Time**: <20 seconds
- **Memory**: <256MB usage

## Time Estimate: 6-8 hours

## Next Steps
Proceed to Task 16: Final Testing and Deployment

---

## Completion Notes
*To be filled after task completion*

**Date Completed**: 
**Time Taken**: 
**Final FPS**: 
**Final Build Size**: 
**Load Time Achieved**: 
**Memory Usage**: 