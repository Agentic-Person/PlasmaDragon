using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Reflection;
using Newtonsoft.Json;

namespace PlasmaDragon.Combat
{
    [System.Serializable]
    public class AIBossStats
    {
        [Header("🧠 AI Boss Configuration")]
        public string bossName = "Lord Drakmoor";
        public BossType bossType = BossType.Tactical;
        
        [Header("❤️ Health & Defense")]
        public float maxHealth = 500f;
        public float armor = 25f;
        public float regenerationRate = 2f; // HP per second
        
        [Header("🎯 AI Decision System")]
        public float aiDecisionInterval = 12f; // Claude API calls every 12 seconds
        public float emergencyDecisionInterval = 5f; // Faster decisions when health < 25%
        public bool useAIDecisions = true;
        public bool cacheSimilarSituations = true;
        
        [Header("🏃 Movement")]
        public float moveSpeed = 8f;
        public float combatSpeed = 12f;
        public float retreatSpeed = 15f;
        public float rotationSpeed = 90f;
        
        [Header("⚔️ Combat Abilities")]
        public float primaryAttackDamage = 40f;
        public float specialAttackDamage = 80f;
        public float primaryAttackCooldown = 3f;
        public float specialAttackCooldown = 8f;
        public float detectionRange = 60f;
        public float attackRange = 25f;
        
        [Header("🔫 Projectiles & Effects")]
        public GameObject primaryProjectilePrefab;
        public GameObject specialProjectilePrefab;
        public GameObject teleportEffect;
        public Transform[] attackPoints;
        public ParticleSystem[] specialEffects;
        
        [Header("🔊 Audio")]
        public AudioClip[] tauntSounds;
        public AudioClip[] attackSounds;
        public AudioClip[] specialAbilitySounds;
        public AudioClip defeatSound;
    }
    
    public enum BossType
    {
        Aggressive,  // Rushes player, high damage
        Tactical,    // Uses cover, strategic positioning
        Defensive,   // Shields, healing, area denial
        Adaptive     // Changes strategy based on player
    }
    
    public enum BossState
    {
        Spawning,    // Initial entrance
        Hunting,     // Looking for player
        Engaging,    // Active combat
        Retreating,  // Low health, seeking cover
        Channeling,  // Casting special ability
        Stunned,     // Temporarily disabled
        Defeated     // Dead
    }
    
    [System.Serializable]
    public class PlayerBehaviorData
    {
        public Vector3 lastKnownPosition;
        public Vector3 averagePosition;
        public float averageAltitude;
        public float averageSpeed;
        public List<Vector3> flightPath = new List<Vector3>();
        public Dictionary<string, int> preferredTactics = new Dictionary<string, int>();
        public float combatAggression; // 0-1 scale
        public float evasionSkill; // 0-1 scale
        public DateTime lastUpdate;
        
        public void UpdateBehavior(Transform player, Rigidbody playerRb)
        {
            lastKnownPosition = player.position;
            averageSpeed = playerRb.linearVelocity.magnitude;
            
            // Track flight path (last 20 positions)
            flightPath.Add(player.position);
            if (flightPath.Count > 20)
                flightPath.RemoveAt(0);
                
            // Calculate average position and altitude
            if (flightPath.Count > 0)
            {
                Vector3 sum = Vector3.zero;
                foreach (var pos in flightPath)
                    sum += pos;
                averagePosition = sum / flightPath.Count;
                averageAltitude = averagePosition.y;
            }
            
            lastUpdate = DateTime.UtcNow;
        }
    }
    
    [System.Serializable]
    public class AIDecisionCache
    {
        public string situationHash;
        public string decision;
        public DateTime timestamp;
        public int useCount;
    }
    
    public class AIBossController : MonoBehaviour
    {
        [Header("🧠 AI BOSS SYSTEM")]
        public AIBossStats stats = new AIBossStats();
        
        [Header("🎯 Tactical Zones")]
        public Transform[] coverPoints;
        public Transform[] attackPositions;
        public Transform[] retreatPositions;
        
        [Header("🔍 Debug Info")]
        public bool showDebugInfo = true;
        public bool showRanges = true;
        public bool logAIDecisions = true;
        
        // Current state
        public BossState currentState { get; private set; } = BossState.Spawning;
        public float currentHealth { get; private set; }
        public string lastAIDecision { get; private set; } = "Initializing...";
        
        // Player tracking
        private Transform playerTarget;
        private Rigidbody playerRigidbody;
        private PlayerBehaviorData playerBehavior = new PlayerBehaviorData();
        
        // AI system
        private float lastAIDecisionTime;
        private List<AIDecisionCache> decisionCache = new List<AIDecisionCache>();
        private string currentStrategy = "aggressive";
        
        // Components
        private NavMeshAgent navAgent;
        private Animator animator;
        private AudioSource audioSource;
        
        // Combat timing
        private float lastPrimaryAttack;
        private float lastSpecialAttack;
        private bool isChannelingSpecial = false;
        
        // Animation hashes
        private readonly int animSpeed = Animator.StringToHash("Speed");
        private readonly int animAttack = Animator.StringToHash("Attack");
        private readonly int animSpecial = Animator.StringToHash("Special");
        private readonly int animHit = Animator.StringToHash("Hit");
        private readonly int animDead = Animator.StringToHash("Dead");
        
        void Start()
        {
            InitializeBoss();
        }
        
        void InitializeBoss()
        {
            // Setup health
            currentHealth = stats.maxHealth;
            
            // Find components
            navAgent = GetComponent<NavMeshAgent>();
            if (navAgent == null)
                navAgent = gameObject.AddComponent<NavMeshAgent>();
                
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
            
            // Configure NavMeshAgent
            navAgent.speed = stats.moveSpeed;
            navAgent.angularSpeed = stats.rotationSpeed;
            navAgent.stoppingDistance = stats.attackRange * 0.8f;
            
            // Find player
            FindPlayer();
            
            // Start with spawning state
            ChangeState(BossState.Spawning);
            
            Debug.Log($"🧠 AI Boss '{stats.bossName}' initialized - Type: {stats.bossType}, AI Interval: {stats.aiDecisionInterval}s");
        }
        
        void Update()
        {
            if (currentState == BossState.Defeated) return;
            
            // Find player if lost
            if (playerTarget == null)
                FindPlayer();
            
            // Update player behavior tracking
            if (playerTarget != null && playerRigidbody != null)
                playerBehavior.UpdateBehavior(playerTarget, playerRigidbody);
            
            // Health regeneration
            if (currentHealth < stats.maxHealth && currentState != BossState.Retreating)
            {
                currentHealth = Mathf.Min(stats.maxHealth, currentHealth + stats.regenerationRate * Time.deltaTime);
            }
            
            // AI decision making
            HandleAIDecisions();
            
            // State machine
            switch (currentState)
            {
                case BossState.Spawning:
                    HandleSpawningState();
                    break;
                case BossState.Hunting:
                    HandleHuntingState();
                    break;
                case BossState.Engaging:
                    HandleEngagingState();
                    break;
                case BossState.Retreating:
                    HandleRetreatingState();
                    break;
                case BossState.Channeling:
                    HandleChannelingState();
                    break;
                case BossState.Stunned:
                    HandleStunnedState();
                    break;
            }
            
            // Update animations
            UpdateAnimations();
        }
        
        void FindPlayer()
        {
            // Look for the dragon by name patterns
            string[] dragonNames = { "Unka Toon", "🐉 Plasma Dragon", "Dragon", "Player" };
            
            foreach (string dragonName in dragonNames)
            {
                GameObject dragon = GameObject.Find(dragonName);
                if (dragon != null)
                {
                    playerTarget = dragon.transform;
                    playerRigidbody = dragon.GetComponent<Rigidbody>();
                    Debug.Log($"🎯 Boss found target: {playerTarget.name}");
                    return;
                }
            }
            
            // Fallback: Look for BasicFlightController component
            var flightController = FindObjectOfType<BasicFlightController>();
            if (flightController != null)
            {
                playerTarget = flightController.transform;
                playerRigidbody = flightController.GetComponent<Rigidbody>();
            }
        }
        
        async void HandleAIDecisions()
        {
            if (!stats.useAIDecisions || playerTarget == null) return;
            
            // Determine AI decision interval based on health
            float currentInterval = currentHealth < (stats.maxHealth * 0.25f) ? 
                stats.emergencyDecisionInterval : stats.aiDecisionInterval;
            
            if (Time.time - lastAIDecisionTime >= currentInterval)
            {
                await MakeAIDecision();
                lastAIDecisionTime = Time.time;
            }
        }
        
        async Task MakeAIDecision()
        {
            try
            {
                // Create context for AI decision
                var gameContext = CreateGameContext();
                string situationHash = GetSituationHash(gameContext);
                
                // Check cache first
                if (stats.cacheSimilarSituations)
                {
                    var cachedDecision = GetCachedDecision(situationHash);
                    if (cachedDecision != null)
                    {
                        ExecuteAIDecision(cachedDecision.decision);
                        cachedDecision.useCount++;
                        if (logAIDecisions)
                            Debug.Log($"🧠 Boss used cached decision: {cachedDecision.decision}");
                        return;
                    }
                }
                
                // Make new AI decision
                string decision = await CallClaudeAPI(gameContext);
                
                // Cache the decision
                if (stats.cacheSimilarSituations && !string.IsNullOrEmpty(decision))
                {
                    CacheDecision(situationHash, decision);
                }
                
                // Execute the decision
                ExecuteAIDecision(decision);
                lastAIDecision = decision;
                
                if (logAIDecisions)
                    Debug.Log($"🧠 Boss AI Decision: {decision}");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"🧠 AI Decision failed, using fallback: {e.Message}");
                ExecuteFallbackBehavior();
            }
        }
        
        object CreateGameContext()
        {
            float distanceToPlayer = playerTarget != null ? 
                Vector3.Distance(transform.position, playerTarget.position) : 999f;
            
            return new
            {
                // Boss status
                bossHealth = currentHealth / stats.maxHealth,
                bossPosition = transform.position,
                currentState = currentState.ToString(),
                currentStrategy = currentStrategy,
                
                // Player status
                playerPosition = playerTarget?.position ?? Vector3.zero,
                playerAltitude = playerTarget?.position.y ?? 0f,
                playerSpeed = playerBehavior.averageSpeed,
                distanceToPlayer = distanceToPlayer,
                
                // Player behavior analysis
                playerAverageAltitude = playerBehavior.averageAltitude,
                playerCombatAggression = playerBehavior.combatAggression,
                playerEvasionSkill = playerBehavior.evasionSkill,
                playerPreferredTactics = playerBehavior.preferredTactics,
                
                // Combat context
                lastAttackTime = Time.time - lastPrimaryAttack,
                lastSpecialTime = Time.time - lastSpecialAttack,
                canUseSpecial = Time.time - lastSpecialAttack >= stats.specialAttackCooldown,
                
                // Environmental context
                coverAvailable = coverPoints?.Length > 0,
                retreatAvailable = retreatPositions?.Length > 0,
                attackPositionsAvailable = attackPositions?.Length > 0
            };
        }
        
        async Task<string> CallClaudeAPI(object context)
        {
            // This is a simplified version - you'll need to implement actual Claude API calls
            // For now, return intelligent fallback decisions based on context
            
            var contextData = JsonConvert.SerializeObject(context);
            
            // TODO: Replace with actual Claude API call
            // var response = await ClaudeAPIClient.GetCompletion($"You are an AI boss in a dragon combat game. Given this context: {contextData}, decide your next action. Respond with one of: 'aggressive_attack', 'defensive_retreat', 'special_ability', 'tactical_reposition', 'area_denial'");
            
            // Intelligent fallback for demo
            return GenerateIntelligentFallback(context);
        }
        
        string GenerateIntelligentFallback(object context)
        {
            // Use reflection to access properties instead of dynamic
            var contextType = context.GetType();
            float healthPercent = (float)contextType.GetProperty("bossHealth").GetValue(context);
            float distanceToPlayer = (float)contextType.GetProperty("distanceToPlayer").GetValue(context);
            bool canUseSpecial = (bool)contextType.GetProperty("canUseSpecial").GetValue(context);
            
            // Decision logic based on health and distance
            if (healthPercent < 0.25f)
            {
                return "defensive_retreat";
            }
            else if (distanceToPlayer > stats.attackRange * 1.5f)
            {
                return "tactical_reposition";
            }
            else if (canUseSpecial && healthPercent < 0.5f)
            {
                return "special_ability";
            }
            else if (distanceToPlayer <= stats.attackRange)
            {
                return "aggressive_attack";
            }
            else
            {
                return "area_denial";
            }
        }
        
        void ExecuteAIDecision(string decision)
        {
            switch (decision.ToLower())
            {
                case "aggressive_attack":
                    ExecuteAggressiveAttack();
                    break;
                case "defensive_retreat":
                    ExecuteDefensiveRetreat();
                    break;
                case "special_ability":
                    ExecuteSpecialAbility();
                    break;
                case "tactical_reposition":
                    ExecuteTacticalReposition();
                    break;
                case "area_denial":
                    ExecuteAreaDenial();
                    break;
                default:
                    ExecuteFallbackBehavior();
                    break;
            }
        }
        
        void ExecuteAggressiveAttack()
        {
            currentStrategy = "aggressive";
            if (currentState != BossState.Engaging)
                ChangeState(BossState.Engaging);
                
            navAgent.speed = stats.combatSpeed;
            if (playerTarget != null)
                navAgent.SetDestination(playerTarget.position);
        }
        
        void ExecuteDefensiveRetreat()
        {
            currentStrategy = "defensive";
            ChangeState(BossState.Retreating);
            
            // Find nearest retreat position
            if (retreatPositions != null && retreatPositions.Length > 0)
            {
                Transform nearestRetreat = GetNearestPosition(retreatPositions);
                navAgent.speed = stats.retreatSpeed;
                navAgent.SetDestination(nearestRetreat.position);
            }
        }
        
        void ExecuteSpecialAbility()
        {
            if (Time.time - lastSpecialAttack >= stats.specialAttackCooldown)
            {
                ChangeState(BossState.Channeling);
                isChannelingSpecial = true;
                Invoke(nameof(FinishSpecialAbility), 2f); // 2 second channel time
            }
        }
        
        void ExecuteTacticalReposition()
        {
            currentStrategy = "tactical";
            
            // Find best attack position
            if (attackPositions != null && attackPositions.Length > 0)
            {
                Transform bestPosition = GetBestAttackPosition();
                navAgent.speed = stats.moveSpeed;
                navAgent.SetDestination(bestPosition.position);
            }
        }
        
        void ExecuteAreaDenial()
        {
            // Create area denial attacks (multiple projectiles)
            if (stats.attackPoints != null && stats.attackPoints.Length > 0)
            {
                for (int i = 0; i < stats.attackPoints.Length; i++)
                {
                    StartCoroutine(DelayedAreaAttack(i * 0.3f, stats.attackPoints[i]));
                }
            }
        }
        
        System.Collections.IEnumerator DelayedAreaAttack(float delay, Transform attackPoint)
        {
            yield return new WaitForSeconds(delay);
            FireProjectile(attackPoint, stats.primaryProjectilePrefab, stats.primaryAttackDamage);
        }
        
        Transform GetNearestPosition(Transform[] positions)
        {
            Transform nearest = positions[0];
            float nearestDistance = Vector3.Distance(transform.position, nearest.position);
            
            foreach (Transform pos in positions)
            {
                float distance = Vector3.Distance(transform.position, pos.position);
                if (distance < nearestDistance)
                {
                    nearest = pos;
                    nearestDistance = distance;
                }
            }
            
            return nearest;
        }
        
        Transform GetBestAttackPosition()
        {
            if (attackPositions == null || attackPositions.Length == 0) return transform;
            
            Transform best = attackPositions[0];
            float bestScore = CalculatePositionScore(best);
            
            foreach (Transform pos in attackPositions)
            {
                float score = CalculatePositionScore(pos);
                if (score > bestScore)
                {
                    best = pos;
                    bestScore = score;
                }
            }
            
            return best;
        }
        
        float CalculatePositionScore(Transform position)
        {
            if (playerTarget == null) return 0f;
            
            float distanceToPlayer = Vector3.Distance(position.position, playerTarget.position);
            float optimalDistance = stats.attackRange * 0.8f;
            
            // Prefer positions near optimal attack range
            float distanceScore = 1f - Mathf.Abs(distanceToPlayer - optimalDistance) / optimalDistance;
            
            // Add height advantage bonus
            float heightAdvantage = position.position.y > playerTarget.position.y ? 0.2f : 0f;
            
            return distanceScore + heightAdvantage;
        }
        
        void HandleSpawningState()
        {
            // Play entrance animation, then start hunting
            ChangeState(BossState.Hunting);
        }
        
        void HandleHuntingState()
        {
            if (playerTarget == null) return;
            
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
            
            if (distanceToPlayer <= stats.detectionRange)
            {
                ChangeState(BossState.Engaging);
            }
            else
            {
                // Move towards last known player position
                navAgent.SetDestination(playerBehavior.lastKnownPosition);
            }
        }
        
        void HandleEngagingState()
        {
            if (playerTarget == null) return;
            
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
            
            // Face the player
            Vector3 direction = (playerTarget.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 
                stats.rotationSpeed * Time.deltaTime);
            
            // Attack if in range and off cooldown
            if (distanceToPlayer <= stats.attackRange && 
                Time.time - lastPrimaryAttack >= stats.primaryAttackCooldown)
            {
                PerformPrimaryAttack();
            }
        }
        
        void HandleRetreatingState()
        {
            // Continue moving to retreat position
            if (!navAgent.hasPath || navAgent.remainingDistance < 2f)
            {
                // Reached retreat position, start hunting again
                ChangeState(BossState.Hunting);
            }
        }
        
        void HandleChannelingState()
        {
            // Stop moving while channeling
            navAgent.ResetPath();
            
            if (!isChannelingSpecial)
            {
                ChangeState(BossState.Engaging);
            }
        }
        
        void HandleStunnedState()
        {
            // Disabled for a few seconds - handled by external stun system
        }
        
        void PerformPrimaryAttack()
        {
            if (stats.attackPoints != null && stats.attackPoints.Length > 0)
            {
                Transform attackPoint = stats.attackPoints[0];
                FireProjectile(attackPoint, stats.primaryProjectilePrefab, stats.primaryAttackDamage);
            }
            
            lastPrimaryAttack = Time.time;
            
            // Play attack animation
            if (animator != null)
                animator.SetTrigger(animAttack);
                
            // Play attack sound
            PlayRandomSound(stats.attackSounds);
        }
        
        void FinishSpecialAbility()
        {
            if (stats.attackPoints != null && stats.attackPoints.Length > 0)
            {
                foreach (Transform attackPoint in stats.attackPoints)
                {
                    FireProjectile(attackPoint, stats.specialProjectilePrefab, stats.specialAttackDamage);
                }
            }
            
            lastSpecialAttack = Time.time;
            isChannelingSpecial = false;
            
            // Play special animation
            if (animator != null)
                animator.SetTrigger(animSpecial);
                
            // Play special sound
            PlayRandomSound(stats.specialAbilitySounds);
            
            // Activate special effects
            if (stats.specialEffects != null)
            {
                foreach (ParticleSystem effect in stats.specialEffects)
                {
                    if (effect != null) effect.Play();
                }
            }
        }
        
        void FireProjectile(Transform firePoint, GameObject projectilePrefab, float damage)
        {
            if (projectilePrefab == null || firePoint == null || playerTarget == null) return;
            
            // Create projectile
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            
            // Add velocity toward player with prediction
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null && playerRigidbody != null)
            {
                Vector3 targetPosition = playerTarget.position;
                
                // Lead target prediction
                float projectileSpeed = 30f; // Adjust as needed
                float timeToTarget = Vector3.Distance(firePoint.position, targetPosition) / projectileSpeed;
                targetPosition += playerRigidbody.linearVelocity * timeToTarget;
                
                Vector3 direction = (targetPosition - firePoint.position).normalized;
                projectileRb.linearVelocity = direction * projectileSpeed;
            }
            
            // Add damage component
            var projectileScript = projectile.GetComponent<BossProjectile>();
            if (projectileScript == null)
                projectileScript = projectile.AddComponent<BossProjectile>();
                
            projectileScript.damage = damage;
            projectileScript.shooter = this;
        }
        
        public void TakeDamage(float damage)
        {
            if (currentState == BossState.Defeated) return;
            
            // Apply armor reduction
            float actualDamage = Mathf.Max(1f, damage - stats.armor);
            currentHealth -= actualDamage;
            
            Debug.Log($"💔 Boss '{stats.bossName}' took {actualDamage} damage! Health: {currentHealth}/{stats.maxHealth}");
            
            // Play hit animation
            if (animator != null)
                animator.SetTrigger(animHit);
            
            // Check if defeated
            if (currentHealth <= 0)
            {
                Die();
            }
            else if (currentHealth < stats.maxHealth * 0.25f && currentState != BossState.Retreating)
            {
                // Low health - force a quick AI decision
                lastAIDecisionTime = 0f;
            }
        }
        
        void Die()
        {
            ChangeState(BossState.Defeated);
            
            // Stop all movement
            if (navAgent != null)
                navAgent.enabled = false;
            
            // Play death animation
            if (animator != null)
                animator.SetBool(animDead, true);
            
            // Play death sound
            if (stats.defeatSound != null && audioSource != null)
                audioSource.PlayOneShot(stats.defeatSound);
            
            Debug.Log($"💀 AI Boss '{stats.bossName}' has been defeated!");
            
            // Trigger any boss defeat events
            // TODO: Award tokens, trigger victory sequence, etc.
        }
        
        void ChangeState(BossState newState)
        {
            if (currentState == newState) return;
            
            Debug.Log($"🧠 Boss state: {currentState} → {newState}");
            currentState = newState;
        }
        
        void UpdateAnimations()
        {
            if (animator == null) return;
            
            // Set speed parameter
            float speed = navAgent.velocity.magnitude;
            animator.SetFloat(animSpeed, speed);
        }
        
        void PlayRandomSound(AudioClip[] clips)
        {
            if (clips != null && clips.Length > 0 && audioSource != null)
            {
                AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
                audioSource.PlayOneShot(clip);
            }
        }
        
        void ExecuteFallbackBehavior()
        {
            // Simple fallback when AI fails
            if (playerTarget != null)
            {
                float distance = Vector3.Distance(transform.position, playerTarget.position);
                if (distance > stats.attackRange)
                {
                    navAgent.SetDestination(playerTarget.position);
                }
                else
                {
                    PerformPrimaryAttack();
                }
            }
        }
        
        // AI Decision Caching
        string GetSituationHash(object context)
        {
            // Create a simplified hash of the current situation using reflection
            var contextType = context.GetType();
            float bossHealth = (float)contextType.GetProperty("bossHealth").GetValue(context);
            float distanceToPlayer = (float)contextType.GetProperty("distanceToPlayer").GetValue(context);
            float playerAltitude = (float)contextType.GetProperty("playerAltitude").GetValue(context);
            return $"{bossHealth:F1}_{distanceToPlayer:F0}_{playerAltitude:F0}_{currentState}";
        }
        
        AIDecisionCache GetCachedDecision(string situationHash)
        {
            return decisionCache.Find(cache => cache.situationHash == situationHash);
        }
        
        void CacheDecision(string situationHash, string decision)
        {
            decisionCache.Add(new AIDecisionCache
            {
                situationHash = situationHash,
                decision = decision,
                timestamp = DateTime.UtcNow,
                useCount = 1
            });
            
            // Limit cache size
            if (decisionCache.Count > 50)
            {
                decisionCache.RemoveAt(0);
            }
        }
        
        void OnDrawGizmosSelected()
        {
            if (!showDebugInfo) return;
            
            // Draw ranges
            if (showRanges)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, stats.detectionRange);
                
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, stats.attackRange);
            }
            
            // Draw tactical positions
            if (attackPositions != null)
            {
                Gizmos.color = Color.blue;
                foreach (Transform pos in attackPositions)
                {
                    if (pos != null)
                        Gizmos.DrawWireCube(pos.position, Vector3.one * 2f);
                }
            }
            
            if (retreatPositions != null)
            {
                Gizmos.color = Color.green;
                foreach (Transform pos in retreatPositions)
                {
                    if (pos != null)
                        Gizmos.DrawWireCube(pos.position, Vector3.one * 2f);
                }
            }
        }
    }
    
    public class BossProjectile : MonoBehaviour
    {
        [HideInInspector] public float damage = 40f;
        [HideInInspector] public AIBossController shooter;
        
        public float lifetime = 5f;
        public GameObject hitEffect;
        
        void Start()
        {
            Destroy(gameObject, lifetime);
        }
        
        void OnTriggerEnter(Collider other)
        {
            // Check if we hit the dragon
            if (other.GetComponent<BasicFlightController>() != null)
            {
                Debug.Log($"💥 Dragon hit by boss projectile! Damage: {damage}");
                
                // TODO: Apply damage to dragon
                // var dragonHealth = other.GetComponent<DragonHealth>();
                // if (dragonHealth != null) dragonHealth.TakeDamage(damage);
                
                CreateHitEffect();
                Destroy(gameObject);
            }
            // Hit terrain or other objects
            else if (!other.isTrigger)
            {
                CreateHitEffect();
                Destroy(gameObject);
            }
        }
        
        void CreateHitEffect()
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
        }
    }
} 