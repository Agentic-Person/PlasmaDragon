# Task 13: Level 2 Adaptive AI Implementation

## Status: Pending

## Priority: LOW - Enhanced feature

## Description
Implement Level 2 where enemies remember and adapt to player behavior from Level 1. This includes heat map tracking, patrol route adjustments, and boss strategy evolution.

## Prerequisites
- Task 10 (AI Boss) completed
- Basic enemy system working
- Player tracking system ready

## Step-by-Step Instructions

### 1. Create Player Behavior Tracking System
`Assets/_Project/Scripts/AI/PlayerBehaviorTracker.cs`:
```csharp
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PlayerBehaviorData
{
    public float preferredAltitude;
    public float averageSpeed;
    public string favoriteWeapon;
    public List<Vector3> commonFlightPaths;
    public Dictionary<string, int> weaponUsageCount;
    public float averageEngagementDistance;
    public List<string> bossDefeatStrategies;
    public float totalPlayTime;
    
    public PlayerBehaviorData()
    {
        commonFlightPaths = new List<Vector3>();
        weaponUsageCount = new Dictionary<string, int>();
        bossDefeatStrategies = new List<string>();
    }
}

public class PlayerBehaviorTracker : MonoBehaviour
{
    [Header("Tracking Settings")]
    public float positionRecordInterval = 0.5f;
    public int maxPathPoints = 1000;
    
    private static PlayerBehaviorTracker instance;
    public static PlayerBehaviorTracker Instance => instance;
    
    private PlayerBehaviorData currentData;
    private float nextRecordTime;
    private Transform player;
    private List<float> altitudeHistory = new List<float>();
    private List<float> speedHistory = new List<float>();
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        LoadBehaviorData();
    }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    void Update()
    {
        if (player == null) return;
        
        // Track position
        if (Time.time >= nextRecordTime)
        {
            RecordPlayerPosition();
            nextRecordTime = Time.time + positionRecordInterval;
        }
        
        // Track altitude and speed
        TrackFlightMetrics();
        
        // Track weapon usage
        TrackWeaponUsage();
        
        // Update play time
        currentData.totalPlayTime += Time.deltaTime;
    }
    
    void RecordPlayerPosition()
    {
        Vector3 position = player.position;
        currentData.commonFlightPaths.Add(position);
        
        // Limit path points
        if (currentData.commonFlightPaths.Count > maxPathPoints)
        {
            currentData.commonFlightPaths.RemoveAt(0);
        }
    }
    
    void TrackFlightMetrics()
    {
        // Altitude tracking
        altitudeHistory.Add(player.position.y);
        if (altitudeHistory.Count > 100)
            altitudeHistory.RemoveAt(0);
        
        // Speed tracking
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            speedHistory.Add(rb.velocity.magnitude);
            if (speedHistory.Count > 100)
                speedHistory.RemoveAt(0);
        }
        
        // Update averages
        if (altitudeHistory.Count > 0)
            currentData.preferredAltitude = altitudeHistory.Average();
        
        if (speedHistory.Count > 0)
            currentData.averageSpeed = speedHistory.Average();
    }
    
    void TrackWeaponUsage()
    {
        // Monitor weapon firing
        if (Input.GetMouseButtonDown(0))
        {
            IncrementWeaponUsage("plasma_orb");
        }
        
        if (Input.GetMouseButton(1))
        {
            IncrementWeaponUsage("rapid_fire");
        }
    }
    
    void IncrementWeaponUsage(string weaponType)
    {
        if (!currentData.weaponUsageCount.ContainsKey(weaponType))
            currentData.weaponUsageCount[weaponType] = 0;
        
        currentData.weaponUsageCount[weaponType]++;
        
        // Update favorite weapon
        currentData.favoriteWeapon = currentData.weaponUsageCount
            .OrderByDescending(kvp => kvp.Value)
            .First().Key;
    }
    
    public void RecordBossDefeatStrategy(string strategy)
    {
        currentData.bossDefeatStrategies.Add(strategy);
    }
    
    public void RecordEngagementDistance(float distance)
    {
        // Running average
        if (currentData.averageEngagementDistance == 0)
            currentData.averageEngagementDistance = distance;
        else
            currentData.averageEngagementDistance = 
                (currentData.averageEngagementDistance + distance) / 2f;
    }
    
    public PlayerBehaviorData GetBehaviorData()
    {
        return currentData;
    }
    
    void SaveBehaviorData()
    {
        string json = JsonUtility.ToJson(currentData);
        PlayerPrefs.SetString("PlayerBehaviorData", json);
        PlayerPrefs.Save();
    }
    
    void LoadBehaviorData()
    {
        string json = PlayerPrefs.GetString("PlayerBehaviorData", "");
        if (!string.IsNullOrEmpty(json))
        {
            currentData = JsonUtility.FromJson<PlayerBehaviorData>(json);
        }
        else
        {
            currentData = new PlayerBehaviorData();
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveBehaviorData();
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) SaveBehaviorData();
    }
    
    void OnDestroy()
    {
        SaveBehaviorData();
    }
}
```

### 2. Create Heat Map System
`Assets/_Project/Scripts/AI/HeatMapSystem.cs`:
```csharp
using UnityEngine;
using System.Collections.Generic;

public class HeatMapSystem : MonoBehaviour
{
    [Header("Heat Map Settings")]
    public int gridSizeX = 50;
    public int gridSizeZ = 50;
    public float cellSize = 10f;
    public float heatDecayRate = 0.1f;
    public float maxHeatValue = 100f;
    
    [Header("Visualization")]
    public bool showDebugVisualization = false;
    public Gradient heatGradient;
    
    private float[,] heatGrid;
    private Vector3 gridOrigin;
    private float updateInterval = 0.5f;
    private float nextUpdateTime;
    
    void Start()
    {
        InitializeGrid();
        LoadHeatMapData();
    }
    
    void InitializeGrid()
    {
        heatGrid = new float[gridSizeX, gridSizeZ];
        
        // Center grid on world
        gridOrigin = new Vector3(
            -(gridSizeX * cellSize) / 2f,
            0,
            -(gridSizeZ * cellSize) / 2f
        );
    }
    
    void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateHeatMap();
            DecayHeat();
            nextUpdateTime = Time.time + updateInterval;
        }
    }
    
    void UpdateHeatMap()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        
        Vector3 playerPos = player.transform.position;
        Vector2Int gridPos = WorldToGrid(playerPos);
        
        if (IsValidGridPosition(gridPos))
        {
            // Add heat at player position
            heatGrid[gridPos.x, gridPos.y] += 10f;
            
            // Add heat to neighboring cells
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    Vector2Int neighborPos = new Vector2Int(
                        gridPos.x + x,
                        gridPos.y + z
                    );
                    
                    if (IsValidGridPosition(neighborPos))
                    {
                        heatGrid[neighborPos.x, neighborPos.y] += 5f;
                    }
                }
            }
            
            // Clamp heat values
            heatGrid[gridPos.x, gridPos.y] = Mathf.Min(
                heatGrid[gridPos.x, gridPos.y], 
                maxHeatValue
            );
        }
    }
    
    void DecayHeat()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                heatGrid[x, z] = Mathf.Max(0, heatGrid[x, z] - heatDecayRate);
            }
        }
    }
    
    public float GetHeatAtPosition(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        
        if (IsValidGridPosition(gridPos))
        {
            return heatGrid[gridPos.x, gridPos.y];
        }
        
        return 0f;
    }
    
    public Vector3[] GetHighHeatPositions(float threshold = 50f)
    {
        List<Vector3> positions = new List<Vector3>();
        
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                if (heatGrid[x, z] >= threshold)
                {
                    positions.Add(GridToWorld(new Vector2Int(x, z)));
                }
            }
        }
        
        return positions.ToArray();
    }
    
    public Vector3 GetHottestPosition()
    {
        float maxHeat = 0f;
        Vector2Int hottestCell = Vector2Int.zero;
        
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                if (heatGrid[x, z] > maxHeat)
                {
                    maxHeat = heatGrid[x, z];
                    hottestCell = new Vector2Int(x, z);
                }
            }
        }
        
        return GridToWorld(hottestCell);
    }
    
    Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - gridOrigin;
        return new Vector2Int(
            Mathf.FloorToInt(localPos.x / cellSize),
            Mathf.FloorToInt(localPos.z / cellSize)
        );
    }
    
    Vector3 GridToWorld(Vector2Int gridPos)
    {
        return gridOrigin + new Vector3(
            gridPos.x * cellSize + cellSize / 2f,
            0,
            gridPos.y * cellSize + cellSize / 2f
        );
    }
    
    bool IsValidGridPosition(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < gridSizeX &&
               gridPos.y >= 0 && gridPos.y < gridSizeZ;
    }
    
    void SaveHeatMapData()
    {
        // Convert 2D array to 1D for serialization
        float[] flatArray = new float[gridSizeX * gridSizeZ];
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                flatArray[x * gridSizeZ + z] = heatGrid[x, z];
            }
        }
        
        string json = JsonUtility.ToJson(new SerializableHeatMap 
        { 
            data = flatArray,
            sizeX = gridSizeX,
            sizeZ = gridSizeZ
        });
        
        PlayerPrefs.SetString("HeatMapData", json);
    }
    
    void LoadHeatMapData()
    {
        string json = PlayerPrefs.GetString("HeatMapData", "");
        if (!string.IsNullOrEmpty(json))
        {
            var loaded = JsonUtility.FromJson<SerializableHeatMap>(json);
            
            // Convert back to 2D array
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    heatGrid[x, z] = loaded.data[x * gridSizeZ + z];
                }
            }
        }
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugVisualization || heatGrid == null) return;
        
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                float heat = heatGrid[x, z];
                if (heat > 0)
                {
                    Vector3 cellCenter = GridToWorld(new Vector2Int(x, z));
                    float normalizedHeat = heat / maxHeatValue;
                    
                    Gizmos.color = heatGradient.Evaluate(normalizedHeat);
                    Gizmos.DrawCube(cellCenter, Vector3.one * cellSize * 0.9f);
                }
            }
        }
    }
    
    [System.Serializable]
    class SerializableHeatMap
    {
        public float[] data;
        public int sizeX;
        public int sizeZ;
    }
}
```

### 3. Create Adaptive Enemy Behavior
`Assets/_Project/Scripts/AI/AdaptiveEnemy.cs`:
```csharp
using UnityEngine;

public class AdaptiveEnemy : BaseEnemy
{
    [Header("Adaptation Settings")]
    public float adaptationStrength = 0.5f;
    public float heatThreshold = 30f;
    
    protected HeatMapSystem heatMap;
    protected PlayerBehaviorData playerData;
    
    protected override void Start()
    {
        base.Start();
        
        heatMap = FindObjectOfType<HeatMapSystem>();
        playerData = PlayerBehaviorTracker.Instance?.GetBehaviorData();
        
        AdaptToPlayerBehavior();
    }
    
    protected virtual void AdaptToPlayerBehavior()
    {
        if (playerData == null) return;
        
        // Adapt based on player preferences
        if (playerData.preferredAltitude > 50f)
        {
            // Player likes high altitude
            EnhanceAntiAirCapabilities();
        }
        
        if (playerData.averageSpeed > 25f)
        {
            // Fast player
            ImproveTargetPrediction();
        }
        
        if (playerData.favoriteWeapon == "plasma_orb")
        {
            // Player uses AOE attacks
            IncreaseSpacing();
        }
    }
    
    protected virtual void EnhanceAntiAirCapabilities()
    {
        // Override in subclasses
    }
    
    protected virtual void ImproveTargetPrediction()
    {
        // Override in subclasses
    }
    
    protected virtual void IncreaseSpacing()
    {
        // Override in subclasses
    }
}
```

### 4. Create Adaptive Archer
`Assets/_Project/Scripts/AI/AdaptiveArcher.cs`:
```csharp
using UnityEngine;

public class AdaptiveArcher : ArcherTower
{
    [Header("Adaptive Features")]
    public float predictiveMultiplier = 1.5f;
    public float enhancedRange = 70f;
    public float quickFireRate = 2f;
    
    private HeatMapSystem heatMap;
    private bool isAdapted = false;
    
    protected override void Start()
    {
        base.Start();
        
        heatMap = FindObjectOfType<HeatMapSystem>();
        AdaptFromLevel1();
    }
    
    void AdaptFromLevel1()
    {
        var playerData = PlayerBehaviorTracker.Instance?.GetBehaviorData();
        if (playerData == null) return;
        
        isAdapted = true;
        
        // High altitude preference - increase range
        if (playerData.preferredAltitude > 50f)
        {
            detectionRange = enhancedRange;
            Debug.Log("Archer: Enhanced range for high-flying dragon");
        }
        
        // Fast player - better prediction
        if (playerData.averageSpeed > 25f)
        {
            leadTargetMultiplier = predictiveMultiplier;
            Debug.Log("Archer: Improved prediction for fast dragon");
        }
        
        // Frequent attacker - faster fire rate
        if (playerData.weaponUsageCount.Count > 0)
        {
            fireRate = quickFireRate;
            Debug.Log("Archer: Increased fire rate for aggressive player");
        }
    }
    
    void Update()
    {
        base.Update();
        
        if (isAdapted && heatMap != null)
        {
            // Check if in high-traffic area
            float localHeat = heatMap.GetHeatAtPosition(transform.position);
            if (localHeat > 50f)
            {
                // Extra alert in hot zones
                fireRate = quickFireRate * 1.5f;
            }
        }
    }
}
```

### 5. Create Adaptive Patrol
`Assets/_Project/Scripts/AI/AdaptivePatrol.cs`:
```csharp
using UnityEngine;
using System.Linq;

public class AdaptivePatrol : MilitaryPatrol
{
    [Header("Adaptive Behavior")]
    public float spreadDistance = 5f;
    public bool useHeatMapRoutes = true;
    
    private HeatMapSystem heatMap;
    private PlayerBehaviorData playerData;
    
    protected override void Start()
    {
        base.Start();
        
        heatMap = FindObjectOfType<HeatMapSystem>();
        playerData = PlayerBehaviorTracker.Instance?.GetBehaviorData();
        
        AdaptPatrolBehavior();
    }
    
    void AdaptPatrolBehavior()
    {
        if (playerData == null) return;
        
        // AOE weapon user - spread out formation
        if (playerData.favoriteWeapon == "plasma_orb")
        {
            formation = FormationType.Circle;
            spacing = spreadDistance;
            Debug.Log("Patrol: Spreading out to avoid AOE damage");
        }
        
        // Adjust patrol routes based on heat map
        if (useHeatMapRoutes && heatMap != null)
        {
            UpdatePatrolRoutes();
        }
    }
    
    void UpdatePatrolRoutes()
    {
        // Get high-traffic positions
        Vector3[] hotSpots = heatMap.GetHighHeatPositions(40f);
        
        if (hotSpots.Length > 0)
        {
            // Create waypoints through hot zones
            waypoints = new Transform[hotSpots.Length];
            
            for (int i = 0; i < hotSpots.Length; i++)
            {
                GameObject wp = new GameObject($"AdaptiveWaypoint_{i}");
                wp.transform.position = hotSpots[i];
                waypoints[i] = wp.transform;
            }
            
            Debug.Log($"Patrol: Created {waypoints.Length} waypoints in high-traffic areas");
        }
    }
    
    protected override void UpdateFormation()
    {
        base.UpdateFormation();
        
        // Dynamic formation based on player proximity
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, 
                player.transform.position);
            
            if (distanceToPlayer < 50f && playerData?.favoriteWeapon == "plasma_orb")
            {
                // Emergency spread when player is close with AOE weapon
                for (int i = 0; i < soldiers.Count; i++)
                {
                    if (soldiers[i] != null)
                    {
                        Vector3 spreadDir = (soldiers[i].transform.position - 
                            transform.position).normalized;
                        Vector3 targetPos = transform.position + 
                            spreadDir * (spreadDistance * 1.5f);
                        
                        soldiers[i].transform.position = Vector3.Lerp(
                            soldiers[i].transform.position,
                            targetPos,
                            Time.deltaTime * 10f
                        );
                    }
                }
            }
        }
    }
}
```

### 6. Create Level 2 Adaptive Commander
`Assets/_Project/Scripts/AI/AdaptiveCommander.cs`:
```csharp
using UnityEngine;
using System.Collections;
using System.Linq;

public class AdaptiveCommander : EliteCommander
{
    [Header("Level 2 Adaptations")]
    public bool rememberPlayerTactics = true;
    public float adaptiveDecisionWeight = 0.7f;
    
    private PlayerBehaviorData level1Data;
    private Dictionary<string, float> tacticCounters;
    
    protected override void Start()
    {
        base.Start();
        
        level1Data = PlayerBehaviorTracker.Instance?.GetBehaviorData();
        InitializeTacticCounters();
        
        if (level1Data != null)
        {
            AnnounceAdaptation();
        }
    }
    
    void InitializeTacticCounters()
    {
        tacticCounters = new Dictionary<string, float>
        {
            { "high_altitude_counter", 0f },
            { "speed_counter", 0f },
            { "aoe_counter", 0f },
            { "range_counter", 0f }
        };
        
        if (level1Data == null) return;
        
        // Analyze Level 1 behavior
        if (level1Data.preferredAltitude > 50f)
        {
            tacticCounters["high_altitude_counter"] = 0.8f;
        }
        
        if (level1Data.averageSpeed > 25f)
        {
            tacticCounters["speed_counter"] = 0.8f;
        }
        
        if (level1Data.favoriteWeapon == "plasma_orb")
        {
            tacticCounters["aoe_counter"] = 0.9f;
        }
        
        if (level1Data.averageEngagementDistance > 40f)
        {
            tacticCounters["range_counter"] = 0.7f;
        }
    }
    
    void AnnounceAdaptation()
    {
        // Visual/audio cue that boss has adapted
        StartCoroutine(AdaptationSequence());
    }
    
    IEnumerator AdaptationSequence()
    {
        // Play special effect
        if (commandAura != null)
        {
            var main = commandAura.main;
            main.startColor = Color.red;
        }
        
        // Boss dialogue
        Debug.Log("Elite Commander: 'I've studied your tactics, dragon rider!'");
        
        yield return new WaitForSeconds(2f);
        
        // List adaptations
        if (tacticCounters["high_altitude_counter"] > 0.5f)
        {
            Debug.Log("Elite Commander: 'Your high-flying won't save you this time!'");
        }
        
        if (tacticCounters["aoe_counter"] > 0.5f)
        {
            Debug.Log("Elite Commander: 'My forces have learned to spread out!'");
        }
        
        // Return aura to normal
        if (commandAura != null)
        {
            var main = commandAura.main;
            main.startColor = Color.white;
        }
    }
    
    protected override void ExecuteAction(BossDecision decision)
    {
        // Modify decision based on Level 1 data
        if (rememberPlayerTactics && level1Data != null)
        {
            decision = AdaptDecision(decision);
        }
        
        base.ExecuteAction(decision);
    }
    
    BossDecision AdaptDecision(BossDecision originalDecision)
    {
        // Counter high-altitude tactics
        if (tacticCounters["high_altitude_counter"] > 0.5f && 
            player.position.y > 50f)
        {
            if (Random.value < adaptiveDecisionWeight)
            {
                originalDecision.action = "enhanced_anti_air";
                originalDecision.reasoning = "Countering high-altitude tactics from Level 1";
            }
        }
        
        // Counter AOE spam
        if (tacticCounters["aoe_counter"] > 0.5f && 
            originalDecision.action == "summon_reinforcements")
        {
            // Modify summon pattern
            originalDecision.parameters = new Dictionary<string, float>
            {
                { "spread_radius", 20f },
                { "formation", 2f } // Spread formation
            };
        }
        
        return originalDecision;
    }
    
    protected override IEnumerator SummonReinforcements()
    {
        isPerformingAction = true;
        
        GetComponent<Animator>()?.SetTrigger("Summon");
        
        yield return new WaitForSeconds(1f);
        
        // Adaptive summon based on player tactics
        if (tacticCounters["aoe_counter"] > 0.5f)
        {
            // Summon in spread formation
            for (int i = 0; i < 5; i++)
            {
                if (reinforcementsSpawned >= maxReinforcements) break;
                
                // Wide spread to avoid AOE
                Vector3 spawnPos = transform.position + 
                    Quaternion.Euler(0, i * 72, 0) * Vector3.forward * 15f;
                spawnPos.y = transform.position.y;
                
                GameObject unit = Instantiate(reinforcementPrefab, spawnPos, 
                    Quaternion.identity);
                
                // Make them adaptive too
                AdaptiveSoldier adaptive = unit.GetComponent<AdaptiveSoldier>();
                if (adaptive != null)
                {
                    adaptive.EnableAOEAvoidance();
                }
                
                reinforcementsSpawned++;
            }
        }
        else
        {
            // Normal summon
            yield return base.SummonReinforcements();
        }
        
        isPerformingAction = false;
        nextActionTime = Time.time + actionCooldown;
    }
    
    // New ability for Level 2
    IEnumerator EnhancedAntiAir()
    {
        isPerformingAction = true;
        
        Debug.Log("Elite Commander: 'Anti-air barrage!'");
        
        // Create vertical projectile spread
        for (int i = 0; i < 8; i++)
        {
            if (isDead) break;
            
            float angle = i * 45f;
            Vector3 direction = Quaternion.Euler(-45, angle, 0) * Vector3.forward;
            
            GameObject projectile = Instantiate(crossbowPrefab, 
                rangedAttackPoint.position, Quaternion.LookRotation(direction));
            
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * 50f;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
        
        isPerformingAction = false;
        nextActionTime = Time.time + actionCooldown;
    }
}
```

### 7. Create Adaptive Soldier
`Assets/_Project/Scripts/AI/AdaptiveSoldier.cs`:
```csharp
using UnityEngine;

public class AdaptiveSoldier : Soldier
{
    [Header("Adaptive Features")]
    public float scatterRadius = 10f;
    public float scatterSpeed = 15f;
    private bool aoeAvoidanceEnabled = false;
    private Vector3 scatterTarget;
    
    public void EnableAOEAvoidance()
    {
        aoeAvoidanceEnabled = true;
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (aoeAvoidanceEnabled && !isDead)
        {
            CheckForIncomingAOE();
        }
    }
    
    void CheckForIncomingAOE()
    {
        // Detect plasma orbs
        Collider[] projectiles = Physics.OverlapSphere(transform.position, 
            20f, LayerMask.GetMask("Projectile"));
        
        foreach (var col in projectiles)
        {
            PlasmaOrb orb = col.GetComponent<PlasmaOrb>();
            if (orb != null)
            {
                // Calculate if we're in danger
                Vector3 orbVelocity = col.GetComponent<Rigidbody>().velocity;
                Vector3 predictedImpact = col.transform.position + 
                    orbVelocity.normalized * 10f;
                
                float dangerDistance = Vector3.Distance(transform.position, 
                    predictedImpact);
                
                if (dangerDistance < 15f)
                {
                    // Scatter!
                    InitiateScatter();
                    break;
                }
            }
        }
    }
    
    void InitiateScatter()
    {
        // Find safe position
        Vector3 scatterDirection = Random.insideUnitSphere;
        scatterDirection.y = 0;
        scatterDirection = scatterDirection.normalized;
        
        scatterTarget = transform.position + scatterDirection * scatterRadius;
        
        // Start moving
        StartCoroutine(ScatterMovement());
    }
    
    IEnumerator ScatterMovement()
    {
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        
        while (Vector3.Distance(transform.position, scatterTarget) > 1f && 
               Time.time - startTime < 2f)
        {
            // Move towards scatter target
            Vector3 moveDir = (scatterTarget - transform.position).normalized;
            transform.position += moveDir * scatterSpeed * Time.deltaTime;
            
            // Face movement direction
            if (moveDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveDir);
            }
            
            yield return null;
        }
    }
}
```

### 8. Create Level Transition Data
`Assets/_Project/Scripts/Utilities/LevelTransitionData.cs`:
```csharp
using UnityEngine;

public class LevelTransitionData : MonoBehaviour
{
    private static LevelTransitionData instance;
    public static LevelTransitionData Instance => instance;
    
    [Header("Level 1 Results")]
    public int enemiesDefeated;
    public int damageDealt;
    public int damageTaken;
    public float completionTime;
    public bool perfectRun;
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void RecordLevel1Completion(GameStats stats)
    {
        enemiesDefeated = stats.enemiesDefeated;
        damageDealt = stats.totalDamageDealt;
        damageTaken = stats.totalDamageTaken;
        completionTime = stats.levelTime;
        perfectRun = stats.totalDamageTaken == 0;
        
        // Save for next level
        SaveTransitionData();
    }
    
    void SaveTransitionData()
    {
        PlayerPrefs.SetInt("L1_EnemiesDefeated", enemiesDefeated);
        PlayerPrefs.SetInt("L1_DamageDealt", damageDealt);
        PlayerPrefs.SetInt("L1_DamageTaken", damageTaken);
        PlayerPrefs.SetFloat("L1_CompletionTime", completionTime);
        PlayerPrefs.SetInt("L1_PerfectRun", perfectRun ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public void LoadTransitionData()
    {
        enemiesDefeated = PlayerPrefs.GetInt("L1_EnemiesDefeated", 0);
        damageDealt = PlayerPrefs.GetInt("L1_DamageDealt", 0);
        damageTaken = PlayerPrefs.GetInt("L1_DamageTaken", 0);
        completionTime = PlayerPrefs.GetFloat("L1_CompletionTime", 0);
        perfectRun = PlayerPrefs.GetInt("L1_PerfectRun", 0) == 1;
    }
}
```

### 9. Create Visual Adaptation Indicators
`Assets/_Project/Scripts/UI/AdaptationIndicator.cs`:
```csharp
using UnityEngine;
using UnityEngine.UI;

public class AdaptationIndicator : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject adaptationPanel;
    public Text adaptationText;
    public Image adaptationIcon;
    
    [Header("Animation")]
    public float displayDuration = 5f;
    public AnimationCurve fadeCurve;
    
    public void ShowAdaptation(string message, Sprite icon = null)
    {
        adaptationPanel.SetActive(true);
        adaptationText.text = message;
        
        if (icon != null)
            adaptationIcon.sprite = icon;
            
        StartCoroutine(AnimateIndicator());
    }
    
    IEnumerator AnimateIndicator()
    {
        CanvasGroup group = adaptationPanel.GetComponent<CanvasGroup>();
        if (group == null)
            group = adaptationPanel.AddComponent<CanvasGroup>();
        
        // Fade in
        float elapsed = 0;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            group.alpha = fadeCurve.Evaluate(elapsed / 0.5f);
            yield return null;
        }
        
        // Display
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out
        elapsed = 0;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            group.alpha = fadeCurve.Evaluate(1f - (elapsed / 0.5f));
            yield return null;
        }
        
        adaptationPanel.SetActive(false);
    }
}
```

## Expected Outcomes
- ✅ Heat map tracks player movement patterns
- ✅ Enemies patrol high-traffic areas
- ✅ Boss remembers Level 1 tactics
- ✅ Soldiers spread out vs AOE attacks
- ✅ Archers have better aim prediction
- ✅ Visual indicators show adaptations
- ✅ Clear difficulty increase from Level 1

## Adaptation Examples
1. **High Flyer**: Enemies increase range and vertical tracking
2. **Speed Demon**: Better target prediction algorithms
3. **AOE Spammer**: Units maintain spacing, scatter on projectile detection
4. **Sniper**: Enemies use cover more effectively
5. **Aggressive**: Faster enemy response times

## Common Issues & Solutions

### Issue: Adaptations Too Subtle
- Increase adaptation multipliers
- Add visual feedback
- Show comparison to Level 1
- Make changes more dramatic

### Issue: Too Difficult
- Cap adaptation strength
- Provide player upgrades
- Balance enemy health
- Add difficulty options

### Issue: Performance with Heat Map
- Reduce grid resolution
- Update less frequently
- Use spatial partitioning
- Limit tracked positions

## Time Estimate: 5-6 hours

## Next Steps
Proceed to Task 14: Polish and VFX

---

## Completion Notes
*To be filled after task completion*

**Date Completed**: 
**Time Taken**: 
**Adaptation Types**: 
**Difficulty Balance**: 
**Player Feedback**: 
**Performance Impact**: 