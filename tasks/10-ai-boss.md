# Task 10: AI Boss Implementation

## Status: Pending

## Priority: HIGH - Unique selling point of the game

## Description
Implement the Elite Commander boss that uses AI API calls (Claude/GPT-4) to make strategic decisions, creating unique and unpredictable encounters each playthrough.

## Prerequisites
- Task 09 (Enemy System) completed
- API credentials for Claude or OpenAI
- Understanding of async/await in Unity

## Step-by-Step Instructions

### 1. Create Boss Prefab Structure
```
PRE_EliteCommander
├── Model (Knight/Commander mesh)
├── Animator
├── Colliders
│   ├── BodyCollider (Capsule)
│   └── HitBox (for melee)
├── AttackPoints
│   ├── RangedAttackPoint
│   ├── ShieldPosition
│   └── SummonPoint
├── Effects
│   ├── ShieldEffect
│   └── CommandAura
└── UI
    └── HealthBarCanvas
```

### 2. Create AI Decision System
`Assets/_Project/Scripts/AI/AIDecisionSystem.cs`:
```csharp
using UnityEngine;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

[Serializable]
public class BossContext
{
    public float bossHealth;
    public float bossMaxHealth;
    public Vector3 playerPosition;
    public float playerAltitude;
    public Vector3 playerVelocity;
    public float distanceToPlayer;
    public string[] availableActions;
    public string lastAction;
    public float timeSinceLastAction;
    public int currentLevel;
    public string[] playerPreviousActions;
}

[Serializable]
public class BossDecision
{
    public string action;
    public string reasoning;
    public float confidence;
    public Dictionary<string, float> parameters;
}

public class AIDecisionSystem : MonoBehaviour
{
    [Header("API Configuration")]
    public string apiEndpoint = "https://api.anthropic.com/v1/messages";
    public string apiKey = ""; // Set via environment variable
    
    [Header("Decision Settings")]
    public float decisionInterval = 10f;
    public float emergencyDecisionThreshold = 0.3f; // If health below 30%
    
    [Header("Caching")]
    public bool enableCaching = true;
    public int maxCacheSize = 50;
    
    private Dictionary<string, BossDecision> decisionCache;
    private float lastDecisionTime;
    
    void Awake()
    {
        decisionCache = new Dictionary<string, BossDecision>();
        
        // Get API key from environment
        apiKey = Environment.GetEnvironmentVariable("CLAUDE_API_KEY") ?? 
                 Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    }
    
    public async Task<BossDecision> GetDecision(BossContext context)
    {
        // Check cache first
        string contextHash = GetContextHash(context);
        if (enableCaching && decisionCache.ContainsKey(contextHash))
        {
            Debug.Log("Using cached decision");
            return decisionCache[contextHash];
        }
        
        // Build prompt
        string prompt = BuildPrompt(context);
        
        try
        {
            // Make API call
            BossDecision decision = await CallAIAPI(prompt);
            
            // Cache result
            if (enableCaching)
            {
                CacheDecision(contextHash, decision);
            }
            
            lastDecisionTime = Time.time;
            return decision;
        }
        catch (Exception e)
        {
            Debug.LogError($"AI API Error: {e.Message}");
            return GetFallbackDecision(context);
        }
    }
    
    string BuildPrompt(BossContext context)
    {
        return $@"You are controlling an Elite Commander boss in an aerial combat game. 
The player is riding a dragon and attacking your fortress.

Current Situation:
- Your Health: {context.bossHealth}/{context.bossMaxHealth} ({context.bossHealth/context.bossMaxHealth*100:F0}%)
- Player Distance: {context.distanceToPlayer:F1}m
- Player Altitude: {context.playerAltitude:F1}m
- Player Speed: {context.playerVelocity.magnitude:F1}m/s
- Time Since Last Action: {context.timeSinceLastAction:F1}s

Available Actions:
{string.Join("\n", context.availableActions)}

Recent Player Actions:
{string.Join("\n", context.playerPreviousActions)}

Choose the best action to defeat the player. Consider:
1. Your current health status
2. Player's position and movement
3. What actions might surprise the player
4. Previous player behavior patterns

Respond with JSON:
{{
    ""action"": ""action_name"",
    ""reasoning"": ""brief explanation"",
    ""confidence"": 0.0-1.0,
    ""parameters"": {{""key"": value}}
}}";
    }
    
    async Task<BossDecision> CallAIAPI(string prompt)
    {
        // Implementation depends on chosen API
        // This is a simplified example
        
        using (var client = new System.Net.Http.HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            
            var requestBody = new
            {
                model = "claude-3-sonnet-20240229",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                max_tokens = 200,
                temperature = 0.7
            };
            
            var response = await client.PostAsync(apiEndpoint,
                new System.Net.Http.StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    System.Text.Encoding.UTF8,
                    "application/json"));
            
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // Parse response and extract decision
            var decision = ParseAIResponse(responseContent);
            return decision;
        }
    }
    
    BossDecision ParseAIResponse(string response)
    {
        try
        {
            // Extract JSON from response
            dynamic parsed = JsonConvert.DeserializeObject(response);
            string content = parsed.content[0].text;
            
            // Parse the JSON decision
            return JsonConvert.DeserializeObject<BossDecision>(content);
        }
        catch
        {
            return GetFallbackDecision(null);
        }
    }
    
    BossDecision GetFallbackDecision(BossContext context)
    {
        // Procedural fallback when API fails
        var decision = new BossDecision();
        
        if (context != null)
        {
            if (context.bossHealth < context.bossMaxHealth * 0.3f)
            {
                decision.action = "defensive_shield";
                decision.reasoning = "Low health, need defense";
            }
            else if (context.distanceToPlayer < 20f)
            {
                decision.action = "smoke_escape";
                decision.reasoning = "Player too close";
            }
            else
            {
                decision.action = "ranged_barrage";
                decision.reasoning = "Standard attack pattern";
            }
        }
        else
        {
            decision.action = "ranged_barrage";
            decision.reasoning = "Default action";
        }
        
        decision.confidence = 0.5f;
        return decision;
    }
    
    string GetContextHash(BossContext context)
    {
        // Create simplified hash for caching
        int healthBracket = Mathf.FloorToInt(context.bossHealth / 10f);
        int distanceBracket = Mathf.FloorToInt(context.distanceToPlayer / 10f);
        return $"{healthBracket}_{distanceBracket}_{context.lastAction}";
    }
    
    void CacheDecision(string hash, BossDecision decision)
    {
        if (decisionCache.Count >= maxCacheSize)
        {
            // Remove oldest entry
            decisionCache.Clear(); // Simple clearing, could be smarter
        }
        
        decisionCache[hash] = decision;
    }
}
```

### 3. Create Elite Commander Boss
`Assets/_Project/Scripts/AI/EliteCommander.cs`:
```csharp
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EliteCommander : BaseEnemy
{
    [Header("Boss Configuration")]
    public float actionCooldown = 2f;
    public float decisionInterval = 10f;
    
    [Header("Abilities")]
    public GameObject crossbowPrefab;
    public Transform rangedAttackPoint;
    public GameObject shieldEffectPrefab;
    public GameObject reinforcementPrefab;
    public GameObject smokeEffectPrefab;
    public ParticleSystem commandAura;
    
    [Header("AI System")]
    public AIDecisionSystem aiSystem;
    
    private Transform player;
    private List<string> playerActionHistory = new List<string>();
    private string currentAction = "idle";
    private float nextActionTime = 0f;
    private float nextDecisionTime = 0f;
    private bool isPerformingAction = false;
    
    // Ability states
    private bool shieldActive = false;
    private float shieldEndTime;
    private int reinforcementsSpawned = 0;
    private const int maxReinforcements = 10;
    
    protected override void Start()
    {
        base.Start();
        maxHealth = 500f; // Tanky boss
        currentHealth = maxHealth;
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
        aiSystem = GetComponent<AIDecisionSystem>();
        
        // Start command aura
        if (commandAura != null)
            commandAura.Play();
    }
    
    void Update()
    {
        if (isDead || player == null) return;
        
        // Update shield
        if (shieldActive && Time.time > shieldEndTime)
        {
            DeactivateShield();
        }
        
        // Get AI decision
        if (Time.time >= nextDecisionTime && !isPerformingAction)
        {
            StartCoroutine(MakeDecision());
            nextDecisionTime = Time.time + decisionInterval;
        }
        
        // Track player actions
        TrackPlayerActions();
    }
    
    IEnumerator MakeDecision()
    {
        // Build context
        BossContext context = new BossContext
        {
            bossHealth = currentHealth,
            bossMaxHealth = maxHealth,
            playerPosition = player.position,
            playerAltitude = player.position.y,
            playerVelocity = player.GetComponent<Rigidbody>()?.velocity ?? Vector3.zero,
            distanceToPlayer = Vector3.Distance(transform.position, player.position),
            availableActions = GetAvailableActions(),
            lastAction = currentAction,
            timeSinceLastAction = Time.time - nextActionTime + actionCooldown,
            currentLevel = 1, // Could get from GameManager
            playerPreviousActions = playerActionHistory.ToArray()
        };
        
        // Get AI decision
        Task<BossDecision> decisionTask = aiSystem.GetDecision(context);
        yield return new WaitUntil(() => decisionTask.IsCompleted);
        
        BossDecision decision = decisionTask.Result;
        
        // Execute decision
        ExecuteAction(decision);
    }
    
    string[] GetAvailableActions()
    {
        List<string> actions = new List<string> { "ranged_barrage" };
        
        if (!shieldActive)
            actions.Add("defensive_shield");
            
        if (reinforcementsSpawned < maxReinforcements)
            actions.Add("summon_reinforcements");
            
        if (Vector3.Distance(transform.position, player.position) < 30f)
            actions.Add("smoke_escape");
            
        return actions.ToArray();
    }
    
    void ExecuteAction(BossDecision decision)
    {
        if (isPerformingAction) return;
        
        currentAction = decision.action;
        
        switch (decision.action)
        {
            case "ranged_barrage":
                StartCoroutine(RangedBarrage());
                break;
                
            case "defensive_shield":
                ActivateShield();
                break;
                
            case "summon_reinforcements":
                StartCoroutine(SummonReinforcements());
                break;
                
            case "smoke_escape":
                StartCoroutine(SmokeEscape());
                break;
                
            default:
                StartCoroutine(RangedBarrage()); // Default
                break;
        }
        
        // Announce action (optional)
        Debug.Log($"Boss Decision: {decision.action} - {decision.reasoning}");
    }
    
    IEnumerator RangedBarrage()
    {
        isPerformingAction = true;
        
        // Fire multiple crossbow bolts
        for (int i = 0; i < 5; i++)
        {
            if (isDead) break;
            
            // Aim at player with prediction
            Vector3 aimPoint = PredictPlayerPosition(0.5f);
            transform.LookAt(new Vector3(aimPoint.x, transform.position.y, aimPoint.z));
            
            // Fire bolt
            GameObject bolt = Instantiate(crossbowPrefab, 
                rangedAttackPoint.position, rangedAttackPoint.rotation);
            
            // Add force
            Rigidbody boltRb = bolt.GetComponent<Rigidbody>();
            if (boltRb != null)
            {
                Vector3 direction = (aimPoint - rangedAttackPoint.position).normalized;
                boltRb.velocity = direction * 40f;
            }
            
            yield return new WaitForSeconds(0.3f);
        }
        
        isPerformingAction = false;
        nextActionTime = Time.time + actionCooldown;
    }
    
    void ActivateShield()
    {
        if (shieldActive) return;
        
        shieldActive = true;
        isInvulnerable = true;
        shieldEndTime = Time.time + 5f;
        
        // Visual effect
        if (shieldEffectPrefab != null)
        {
            GameObject shield = Instantiate(shieldEffectPrefab, transform);
            Destroy(shield, 5f);
        }
        
        // Audio feedback
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
    
    void DeactivateShield()
    {
        shieldActive = false;
        isInvulnerable = false;
    }
    
    IEnumerator SummonReinforcements()
    {
        isPerformingAction = true;
        
        // Summon animation
        GetComponent<Animator>()?.SetTrigger("Summon");
        
        yield return new WaitForSeconds(1f);
        
        // Spawn reinforcements
        for (int i = 0; i < 3; i++)
        {
            if (reinforcementsSpawned >= maxReinforcements) break;
            
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * 10f;
            spawnPos.y = transform.position.y;
            
            Instantiate(reinforcementPrefab, spawnPos, Quaternion.identity);
            reinforcementsSpawned++;
        }
        
        isPerformingAction = false;
        nextActionTime = Time.time + actionCooldown;
    }
    
    IEnumerator SmokeEscape()
    {
        isPerformingAction = true;
        
        // Create smoke
        if (smokeEffectPrefab != null)
        {
            GameObject smoke = Instantiate(smokeEffectPrefab, transform.position, 
                Quaternion.identity);
            Destroy(smoke, 5f);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // Teleport to new position
        Vector3 escapeDirection = (transform.position - player.position).normalized;
        Vector3 newPosition = transform.position + escapeDirection * 20f;
        
        // Ensure valid position
        RaycastHit hit;
        if (Physics.Raycast(newPosition + Vector3.up * 50f, Vector3.down, 
            out hit, 100f))
        {
            newPosition = hit.point + Vector3.up * 2f;
        }
        
        transform.position = newPosition;
        
        isPerformingAction = false;
        nextActionTime = Time.time + actionCooldown;
    }
    
    Vector3 PredictPlayerPosition(float predictionTime)
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            return player.position + playerRb.velocity * predictionTime;
        }
        return player.position;
    }
    
    void TrackPlayerActions()
    {
        // Simple tracking - could be more sophisticated
        DragonWeaponController weaponController = 
            player.GetComponent<DragonWeaponController>();
            
        if (weaponController != null)
        {
            if (Input.GetMouseButtonDown(0))
                playerActionHistory.Add("plasma_orb");
            if (Input.GetMouseButton(1))
                playerActionHistory.Add("rapid_fire");
        }
        
        // Keep history size manageable
        if (playerActionHistory.Count > 20)
        {
            playerActionHistory.RemoveAt(0);
        }
    }
    
    public override void TakeDamage(float damage)
    {
        if (!shieldActive)
        {
            base.TakeDamage(damage);
            
            // Emergency decision on low health
            if (currentHealth < maxHealth * 0.3f && 
                Time.time > nextDecisionTime - 5f)
            {
                nextDecisionTime = Time.time; // Force immediate decision
            }
        }
    }
}
```

### 4. Create Boss Health UI
`Assets/_Project/Scripts/UI/BossHealthBar.cs`:
```csharp
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public EliteCommander boss;
    public Slider healthSlider;
    public Text bossNameText;
    public Text healthText;
    public Image fillImage;
    public Gradient healthGradient;
    
    void Start()
    {
        if (bossNameText != null)
            bossNameText.text = "Elite Commander";
            
        healthSlider.maxValue = boss.GetMaxHealth();
    }
    
    void Update()
    {
        if (boss == null) return;
        
        float health = boss.GetHealth();
        float maxHealth = boss.GetMaxHealth();
        float healthPercent = health / maxHealth;
        
        healthSlider.value = health;
        
        if (healthText != null)
            healthText.text = $"{health:F0} / {maxHealth:F0}";
            
        if (fillImage != null)
            fillImage.color = healthGradient.Evaluate(healthPercent);
            
        // Hide when boss dies
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
```

### 5. Level 2 Adaptive Boss
`Assets/_Project/Scripts/AI/AdaptiveCommander.cs`:
```csharp
public class AdaptiveCommander : EliteCommander
{
    [Header("Adaptation")]
    public PlayerMemorySystem playerMemory;
    
    protected override void Start()
    {
        base.Start();
        
        // Load player patterns from Level 1
        LoadPlayerPatterns();
    }
    
    void LoadPlayerPatterns()
    {
        // Get data from previous level
        var patterns = playerMemory.GetPlayerPatterns();
        
        // Adjust AI behavior based on patterns
        if (patterns.preferredAltitude > 50f)
        {
            // Player likes high altitude - prepare anti-air
        }
        
        if (patterns.favoriteWeapon == "plasma_orb")
        {
            // Player uses AOE - spread out reinforcements
        }
    }
}
```

### 6. Cost Control System
`Assets/_Project/Scripts/AI/AICostController.cs`:
```csharp
public class AICostController : MonoBehaviour
{
    [Header("Cost Limits")]
    public float maxCostPerSession = 0.10f; // $0.10 max
    public int maxAPICallsPerSession = 10;
    
    private float currentCost = 0f;
    private int apiCallCount = 0;
    
    public bool CanMakeAPICall()
    {
        return currentCost < maxCostPerSession && 
               apiCallCount < maxAPICallsPerSession;
    }
    
    public void RecordAPICall(int tokens)
    {
        // Estimate cost (varies by provider)
        float costPerToken = 0.00001f; // Example rate
        float callCost = tokens * costPerToken;
        
        currentCost += callCost;
        apiCallCount++;
        
        Debug.Log($"API Call #{apiCallCount} - Cost: ${callCost:F4} - " +
                  $"Total: ${currentCost:F4}");
    }
}
```

## Expected Outcomes
- ✅ Boss makes intelligent decisions via AI
- ✅ Each encounter feels unique
- ✅ Visible adaptation to player tactics
- ✅ Fallback behavior when API fails
- ✅ Cost controlled under $0.02/session
- ✅ Dramatic boss fight experience
- ✅ Clear visual/audio feedback

## API Configuration
1. Sign up for Claude API or OpenAI
2. Set environment variable: `CLAUDE_API_KEY` or `OPENAI_API_KEY`
3. Configure rate limiting
4. Monitor usage dashboard
5. Set up billing alerts

## Common Issues & Solutions

### Issue: API Calls Too Expensive
- Increase decision interval
- Implement better caching
- Use simpler prompts
- Batch similar contexts

### Issue: API Response Too Slow
- Add timeout handling
- Use async properly
- Preload decisions
- Increase fallback usage

### Issue: Boss Too Predictable
- Add randomization
- Vary available actions
- Include more context
- Adjust temperature

## Time Estimate: 6-8 hours

## Next Steps
Proceed to Task 11: Web3 Integration

---

## Completion Notes
*To be filled after task completion*

**Date Completed**: 
**Time Taken**: 
**API Used**: 
**Average Cost/Session**: 
**Decision Quality**: 
**Player Feedback**: 