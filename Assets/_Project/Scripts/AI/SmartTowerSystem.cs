using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PlasmaDragon.Combat
{
    [System.Serializable]
    public class SmartTowerStats
    {
        [Header("🤖 Smart Tower Configuration")]
        public string towerID = "SmartTower_01";
        public TowerAIType aiType = TowerAIType.Adaptive;
        
        [Header("🎯 Base Tower Stats")]
        public float detectionRange = 45f;
        public float fireRate = 1.2f;
        public float projectileSpeed = 30f;
        public float damage = 15f;
        public float maxTurnSpeed = 120f;
        
        [Header("🧠 AI Learning")]
        public bool enableLearning = true;
        public float learningRate = 0.1f;
        public int maxPlayerHistorySize = 100;
        public float adaptationInterval = 5f; // Analyze every 5 seconds
        
        [Header("🌐 Tower Network")]
        public bool enableCoordination = true;
        public float coordinationRange = 60f;
        public float communicationInterval = 2f;
        
        [Header("🎯 Prediction System")]
        public bool enablePrediction = true;
        public float predictionAccuracy = 0.8f; // 0-1 scale
        public float maxPredictionTime = 3f;
        
        [Header("🔫 Advanced Targeting")]
        public bool useInterceptCourse = true;
        public bool prioritizeVulnerableMoments = true;
        public LayerMask obstacleLayer = -1;
        
        [Header("🔊 Audio & Effects")]
        public AudioClip[] adaptationSounds;
        public ParticleSystem learningEffect;
        public GameObject smartProjectilePrefab;
    }
    
    public enum TowerAIType
    {
        Adaptive,    // Learns from player behavior
        Coordinator, // Focuses on team tactics
        Predictor,   // Specializes in movement prediction
        Ambusher     // Waits for optimal moments
    }
    
    [System.Serializable]
    public class PlayerMovementPattern
    {
        public Vector3 position;
        public Vector3 velocity;
        public float altitude;
        public float timestamp;
        public bool wasEvasive;
        public bool wasAttacking;
    }
    
    [System.Serializable]
    public class TowerCoordination
    {
        public string towerID;
        public Vector3 position;
        public bool isEngaged;
        public Vector3 targetPosition;
        public float lastCommunication;
        public TowerAIType aiType;
    }
    
    public class SmartTowerSystem : MonoBehaviour
    {
        [Header("🤖 SMART TOWER AI")]
        public SmartTowerStats stats = new SmartTowerStats();
        
        [Header("🎯 Tactical Setup")]
        public Transform turretBarrel;
        public Transform firePoint;
        public ParticleSystem muzzleFlash;
        
        [Header("🔍 Debug Info")]
        public bool showDebugInfo = true;
        public bool showPrediction = true;
        public bool showCoordination = true;
        public Color debugColor = Color.cyan;
        
        // Player tracking and AI
        private Transform dragonTarget;
        private Rigidbody dragonRigidbody;
        private List<PlayerMovementPattern> movementHistory = new List<PlayerMovementPattern>();
        private Vector3 predictedTargetPosition;
        private float lastAdaptationTime;
        private float lastCoordinationTime;
        
        // Tower network
        private List<TowerCoordination> nearbyTowers = new List<TowerCoordination>();
        private static List<SmartTowerSystem> allSmartTowers = new List<SmartTowerSystem>();
        
        // Combat state
        private Transform currentTarget;
        private float lastFireTime;
        private bool isEngaged = false;
        private float engagementStartTime;
        
        // AI learning variables
        private float playerAverageSpeed;
        private float playerPreferredAltitude;
        private Vector3 playerMostCommonDirection;
        private float playerEvasionPattern; // 0-1, how evasive they are
        private Dictionary<string, float> tacticEffectiveness = new Dictionary<string, float>();
        
        // Components
        private AudioSource audioSource;
        
        void Start()
        {
            InitializeSmartTower();
        }
        
        void InitializeSmartTower()
        {
            // Setup components
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
            
            // Find turret barrel if not assigned
            if (turretBarrel == null)
                turretBarrel = transform.Find("Barrel") ?? transform.Find("Gun") ?? transform;
            
            // Find fire point if not assigned
            if (firePoint == null)
                firePoint = turretBarrel.Find("FirePoint") ?? turretBarrel;
            
            // Register with tower network
            if (!allSmartTowers.Contains(this))
                allSmartTowers.Add(this);
            
            // Find dragon target
            FindDragonTarget();
            
            // Initialize AI learning
            InitializeLearningSystem();
            
            Debug.Log($"🤖 Smart Tower '{stats.towerID}' initialized - AI Type: {stats.aiType}");
        }
        
        void Update()
        {
            if (dragonTarget == null)
            {
                FindDragonTarget();
                return;
            }
            
            // Update player tracking
            UpdatePlayerTracking();
            
            // AI adaptation
            if (stats.enableLearning && Time.time - lastAdaptationTime >= stats.adaptationInterval)
            {
                PerformAIAdaptation();
                lastAdaptationTime = Time.time;
            }
            
            // Tower coordination
            if (stats.enableCoordination && Time.time - lastCoordinationTime >= stats.communicationInterval)
            {
                UpdateTowerCoordination();
                lastCoordinationTime = Time.time;
            }
            
            // Targeting and combat
            HandleTargeting();
            HandleCombat();
        }
        
        void FindDragonTarget()
        {
            // Look for the dragon by name patterns
            string[] dragonNames = { "Unka Toon", "🐉 Plasma Dragon", "Dragon", "Player" };
            
            foreach (string dragonName in dragonNames)
            {
                GameObject dragon = GameObject.Find(dragonName);
                if (dragon != null)
                {
                    dragonTarget = dragon.transform;
                    dragonRigidbody = dragon.GetComponent<Rigidbody>();
                    Debug.Log($"🎯 Smart Tower '{stats.towerID}' found target: {dragonTarget.name}");
                    return;
                }
            }
            
            // Fallback: Look for BasicFlightController component
            var flightController = FindObjectOfType<BasicFlightController>();
            if (flightController != null)
            {
                dragonTarget = flightController.transform;
                dragonRigidbody = flightController.GetComponent<Rigidbody>();
            }
        }
        
        void UpdatePlayerTracking()
        {
            if (dragonTarget == null || dragonRigidbody == null) return;
            
            // Record current player state
            var pattern = new PlayerMovementPattern
            {
                position = dragonTarget.position,
                velocity = dragonRigidbody.linearVelocity,
                altitude = dragonTarget.position.y,
                timestamp = Time.time,
                wasEvasive = IsPlayerBeingEvasive(),
                wasAttacking = IsPlayerAttacking()
            };
            
            movementHistory.Add(pattern);
            
            // Limit history size for performance
            if (movementHistory.Count > stats.maxPlayerHistorySize)
                movementHistory.RemoveAt(0);
            
            // Update predictions if enabled
            if (stats.enablePrediction)
                UpdateTargetPrediction();
        }
        
        bool IsPlayerBeingEvasive()
        {
            if (movementHistory.Count < 3) return false;
            
            // Check if player is changing direction frequently
            var recent = movementHistory.TakeLast(3).ToArray();
            float directionVariance = 0f;
            
            for (int i = 1; i < recent.Length; i++)
            {
                Vector3 dir1 = recent[i - 1].velocity.normalized;
                Vector3 dir2 = recent[i].velocity.normalized;
                directionVariance += 1f - Vector3.Dot(dir1, dir2);
            }
            
            return directionVariance > 0.5f; // Threshold for evasive behavior
        }
        
        bool IsPlayerAttacking()
        {
            // TODO: Detect if player is in attack mode
            // This could check for firing input, weapon positioning, etc.
            return false;
        }
        
        void PerformAIAdaptation()
        {
            if (movementHistory.Count < 10) return;
            
            // Analyze player behavior patterns
            AnalyzePlayerBehavior();
            
            // Adjust tactics based on AI type
            switch (stats.aiType)
            {
                case TowerAIType.Adaptive:
                    AdaptiveAdjustments();
                    break;
                case TowerAIType.Coordinator:
                    CoordinatorAdjustments();
                    break;
                case TowerAIType.Predictor:
                    PredictorAdjustments();
                    break;
                case TowerAIType.Ambusher:
                    AmbusherAdjustments();
                    break;
            }
            
            // Visual feedback for learning
            if (stats.learningEffect != null)
                stats.learningEffect.Play();
                
            PlayRandomSound(stats.adaptationSounds);
            
            Debug.Log($"🧠 Tower '{stats.towerID}' adapted - Evasion: {playerEvasionPattern:F2}, Avg Speed: {playerAverageSpeed:F1}");
        }
        
        void AnalyzePlayerBehavior()
        {
            if (movementHistory.Count == 0) return;
            
            // Calculate average speed
            playerAverageSpeed = movementHistory.Average(p => p.velocity.magnitude);
            
            // Calculate preferred altitude
            playerPreferredAltitude = movementHistory.Average(p => p.altitude);
            
            // Calculate evasion pattern
            int evasiveCount = movementHistory.Count(p => p.wasEvasive);
            playerEvasionPattern = (float)evasiveCount / movementHistory.Count;
            
            // Find most common direction
            Vector3 directionSum = Vector3.zero;
            foreach (var pattern in movementHistory)
            {
                if (pattern.velocity.magnitude > 1f)
                    directionSum += pattern.velocity.normalized;
            }
            playerMostCommonDirection = directionSum.normalized;
        }
        
        void AdaptiveAdjustments()
        {
            // Adjust fire rate based on player evasion
            float baseFireRate = 1.2f;
            stats.fireRate = baseFireRate * (1f + playerEvasionPattern * 0.5f);
            
            // Adjust prediction time based on player speed
            if (stats.enablePrediction)
            {
                stats.maxPredictionTime = Mathf.Clamp(playerAverageSpeed * 0.1f, 1f, 4f);
            }
            
            // Improve accuracy over time
            stats.predictionAccuracy = Mathf.Min(0.95f, stats.predictionAccuracy + stats.learningRate * 0.01f);
        }
        
        void CoordinatorAdjustments()
        {
            // Focus on coordination with other towers
            // Adjust firing timing to avoid overlapping shots
            if (nearbyTowers.Count > 0)
            {
                float staggerDelay = 0.3f * nearbyTowers.FindIndex(t => t.towerID == stats.towerID);
                // Apply stagger delay in firing logic
            }
        }
        
        void PredictorAdjustments()
        {
            // Specialize in movement prediction
            stats.predictionAccuracy = Mathf.Min(0.98f, stats.predictionAccuracy + stats.learningRate * 0.02f);
            stats.maxPredictionTime = Mathf.Min(5f, stats.maxPredictionTime + 0.1f);
        }
        
        void AmbusherAdjustments()
        {
            // Wait for vulnerable moments
            // Reduce fire rate but increase damage when player is distracted
            stats.fireRate *= 0.7f;
            stats.damage *= 1.3f;
        }
        
        void UpdateTargetPrediction()
        {
            if (dragonTarget == null || dragonRigidbody == null) return;
            
            Vector3 currentPos = dragonTarget.position;
            Vector3 currentVel = dragonRigidbody.linearVelocity;
            
            // Basic kinematic prediction
            float predictionTime = Vector3.Distance(transform.position, currentPos) / stats.projectileSpeed;
            predictionTime = Mathf.Min(predictionTime, stats.maxPredictionTime);
            
            // Apply AI learning to prediction
            Vector3 predictedPos = currentPos + currentVel * predictionTime;
            
            // Factor in learned behavior patterns
            if (playerEvasionPattern > 0.5f)
            {
                // Player is evasive, predict evasion direction
                Vector3 evasionOffset = Vector3.Cross(currentVel.normalized, Vector3.up) * playerEvasionPattern * 5f;
                predictedPos += evasionOffset;
            }
            
            // Apply accuracy factor
            Vector3 accuracyOffset = UnityEngine.Random.insideUnitSphere * (1f - stats.predictionAccuracy) * 3f;
            predictedTargetPosition = predictedPos + accuracyOffset;
        }
        
        void UpdateTowerCoordination()
        {
            if (!stats.enableCoordination) return;
            
            // Find nearby smart towers
            nearbyTowers.Clear();
            foreach (var tower in allSmartTowers)
            {
                if (tower != this && tower != null)
                {
                    float distance = Vector3.Distance(transform.position, tower.transform.position);
                    if (distance <= stats.coordinationRange)
                    {
                        nearbyTowers.Add(new TowerCoordination
                        {
                            towerID = tower.stats.towerID,
                            position = tower.transform.position,
                            isEngaged = tower.isEngaged,
                            targetPosition = tower.predictedTargetPosition,
                            lastCommunication = Time.time,
                            aiType = tower.stats.aiType
                        });
                    }
                }
            }
        }
        
        void HandleTargeting()
        {
            if (dragonTarget == null) return;
            
            float distanceToTarget = Vector3.Distance(transform.position, dragonTarget.position);
            
            if (distanceToTarget <= stats.detectionRange)
            {
                currentTarget = dragonTarget;
                
                if (!isEngaged)
                {
                    isEngaged = true;
                    engagementStartTime = Time.time;
                }
                
                AimAtTarget();
            }
            else
            {
                currentTarget = null;
                isEngaged = false;
            }
        }
        
        void AimAtTarget()
        {
            if (currentTarget == null || turretBarrel == null) return;
            
            Vector3 targetPosition;
            
            // Choose targeting method based on AI type and settings
            if (stats.enablePrediction && stats.aiType == TowerAIType.Predictor)
            {
                targetPosition = predictedTargetPosition;
            }
            else if (stats.useInterceptCourse)
            {
                targetPosition = CalculateInterceptCourse();
            }
            else
            {
                targetPosition = currentTarget.position;
            }
            
            // Calculate aim direction
            Vector3 aimDirection = (targetPosition - turretBarrel.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
            
            // Smooth rotation
            float rotationSpeed = stats.maxTurnSpeed * Time.deltaTime;
            turretBarrel.rotation = Quaternion.RotateTowards(turretBarrel.rotation, targetRotation, rotationSpeed);
        }
        
        Vector3 CalculateInterceptCourse()
        {
            if (dragonRigidbody == null) return dragonTarget.position;
            
            Vector3 targetPos = dragonTarget.position;
            Vector3 targetVel = dragonRigidbody.linearVelocity;
            Vector3 toTarget = targetPos - firePoint.position;
            
            // Solve for intercept time
            float a = Vector3.Dot(targetVel, targetVel) - (stats.projectileSpeed * stats.projectileSpeed);
            float b = 2 * Vector3.Dot(targetVel, toTarget);
            float c = Vector3.Dot(toTarget, toTarget);
            
            float discriminant = b * b - 4 * a * c;
            
            if (discriminant < 0) return targetPos; // No solution, aim at current position
            
            float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
            
            float t = (t1 > 0 && t1 < t2) || t2 < 0 ? t1 : t2;
            
            if (t < 0) return targetPos; // Target moving away faster than projectile
            
            return targetPos + targetVel * t;
        }
        
        void HandleCombat()
        {
            if (currentTarget == null || Time.time - lastFireTime < 1f / stats.fireRate) return;
            
            // Check if we're aimed close enough to fire
            Vector3 aimDirection = turretBarrel.forward;
            Vector3 targetDirection = (predictedTargetPosition - turretBarrel.position).normalized;
            float aimAccuracy = Vector3.Dot(aimDirection, targetDirection);
            
            // AI type affects firing threshold
            float requiredAccuracy = stats.aiType == TowerAIType.Ambusher ? 0.98f : 0.92f;
            
            if (aimAccuracy > requiredAccuracy)
            {
                // Check for optimal firing moment
                if (ShouldFireNow())
                {
                    FireSmartProjectile();
                    lastFireTime = Time.time;
                }
            }
        }
        
        bool ShouldFireNow()
        {
            switch (stats.aiType)
            {
                case TowerAIType.Ambusher:
                    // Wait for player to be distracted or vulnerable
                    return !IsPlayerBeingEvasive();
                    
                case TowerAIType.Coordinator:
                    // Coordinate with other towers to avoid wasting shots
                    return !AreNearbyTowersEngaged();
                    
                default:
                    return true;
            }
        }
        
        bool AreNearbyTowersEngaged()
        {
            return nearbyTowers.Any(tower => tower.isEngaged);
        }
        
        void FireSmartProjectile()
        {
            if (stats.smartProjectilePrefab == null || firePoint == null) return;
            
            // Create smart projectile
            GameObject projectile = Instantiate(stats.smartProjectilePrefab, firePoint.position, firePoint.rotation);
            
            // Add velocity toward predicted position
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
            {
                Vector3 direction = (predictedTargetPosition - firePoint.position).normalized;
                projectileRb.linearVelocity = direction * stats.projectileSpeed;
            }
            
            // Add smart projectile component
            var smartProjectileScript = projectile.GetComponent<SmartProjectile>();
            if (smartProjectileScript == null)
                smartProjectileScript = projectile.AddComponent<SmartProjectile>();
                
            smartProjectileScript.damage = stats.damage;
            smartProjectileScript.shooter = this;
            smartProjectileScript.aiType = stats.aiType;
            
            // Visual and audio effects
            if (muzzleFlash != null)
                muzzleFlash.Play();
            
            Debug.Log($"🤖 Smart Tower '{stats.towerID}' fired at predicted position!");
        }
        
        void PlayRandomSound(AudioClip[] clips)
        {
            if (clips != null && clips.Length > 0 && audioSource != null)
            {
                AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
                audioSource.PlayOneShot(clip);
            }
        }
        
        void InitializeLearningSystem()
        {
            // Initialize tactic effectiveness tracking
            tacticEffectiveness["direct_fire"] = 0.5f;
            tacticEffectiveness["predicted_fire"] = 0.5f;
            tacticEffectiveness["intercept_fire"] = 0.5f;
            tacticEffectiveness["ambush_fire"] = 0.5f;
        }
        
        void OnDestroy()
        {
            // Remove from tower network
            if (allSmartTowers.Contains(this))
                allSmartTowers.Remove(this);
        }
        
        void OnDrawGizmosSelected()
        {
            if (!showDebugInfo) return;
            
            // Draw detection range
            Gizmos.color = debugColor;
            Gizmos.DrawWireSphere(transform.position, stats.detectionRange);
            
            // Draw coordination range
            if (showCoordination && stats.enableCoordination)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, stats.coordinationRange);
            }
            
            // Draw prediction
            if (showPrediction && stats.enablePrediction)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(predictedTargetPosition, 2f);
                
                if (dragonTarget != null)
                {
                    Gizmos.DrawLine(dragonTarget.position, predictedTargetPosition);
                }
            }
            
            // Draw line to current target
            if (currentTarget != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, currentTarget.position);
            }
            
            // Draw coordination lines
            if (showCoordination)
            {
                Gizmos.color = Color.cyan;
                foreach (var tower in nearbyTowers)
                {
                    Gizmos.DrawLine(transform.position, tower.position);
                }
            }
        }
    }
    
    public class SmartProjectile : MonoBehaviour
    {
        [HideInInspector] public float damage = 15f;
        [HideInInspector] public SmartTowerSystem shooter;
        [HideInInspector] public TowerAIType aiType;
        
        public float lifetime = 4f;
        public GameObject hitEffect;
        public GameObject smartHitEffect;
        
        void Start()
        {
            Destroy(gameObject, lifetime);
        }
        
        void OnTriggerEnter(Collider other)
        {
            // Check if we hit the dragon
            if (other.GetComponent<BasicFlightController>() != null)
            {
                Debug.Log($"💥 Dragon hit by smart projectile from {shooter?.stats.towerID ?? "Smart Tower"}! Damage: {damage}");
                
                // TODO: Apply damage to dragon
                // var dragonHealth = other.GetComponent<DragonHealth>();
                // if (dragonHealth != null) dragonHealth.TakeDamage(damage);
                
                CreateSmartHitEffect();
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
            GameObject effect = hitEffect ?? smartHitEffect;
            if (effect != null)
            {
                Instantiate(effect, transform.position, Quaternion.identity);
            }
        }
        
        void CreateSmartHitEffect()
        {
            GameObject effect = smartHitEffect ?? hitEffect;
            if (effect != null)
            {
                Instantiate(effect, transform.position, Quaternion.identity);
            }
        }
    }
} 