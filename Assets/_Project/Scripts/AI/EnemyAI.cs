using UnityEngine;
using UnityEngine.AI;

namespace PlasmaDragon.Combat
{
    public enum EnemyType
    {
        Soldier,    // Melee combat, tries to get close
        Archer,     // Ranged combat, keeps distance
        Guard       // Stationary, shoots when in range
    }
    
    public enum EnemyState
    {
        Idle,       // Standing around, looking for targets
        Patrol,     // Moving along patrol points
        Tracking,   // Following the dragon
        Attacking,  // In combat
        Retreating, // Moving back to safe distance
        Dead        // Defeated
    }
    
    [System.Serializable]
    public class EnemyStats
    {
        [Header("👤 Basic Info")]
        public EnemyType enemyType = EnemyType.Soldier;
        public string enemyName = "Enemy Soldier";
        
        [Header("❤️ Health & Defense")]
        public float maxHealth = 100f;
        public float armor = 0f; // Damage reduction
        
        [Header("🎯 Detection")]
        public float detectionRange = 30f;
        public float loseTargetRange = 50f; // Larger than detection to prevent flickering
        public LayerMask targetLayers = -1;
        
        [Header("🏃 Movement")]
        public float moveSpeed = 3.5f;
        public float runSpeed = 5.5f;
        public float rotationSpeed = 180f; // degrees per second
        
        [Header("⚔️ Combat")]
        public float attackRange = 2f; // For soldiers
        public float preferredDistance = 15f; // For archers
        public float attackDamage = 20f;
        public float attackCooldown = 2f;
        public GameObject projectilePrefab; // For archers
        public Transform attackPoint;
        
        [Header("🔊 Audio")]
        public AudioClip[] alertSounds;
        public AudioClip[] attackSounds;
        public AudioClip[] damageSounds;
        public AudioClip deathSound;
    }
    
    public class EnemyAI : MonoBehaviour
    {
        [Header("👤 ENEMY AI SYSTEM")]
        public EnemyStats stats = new EnemyStats();
        
        [Header("🎯 Patrol System")]
        public Transform[] patrolPoints;
        public float patrolWaitTime = 2f;
        public bool patrolInOrder = true;
        
        [Header("🔍 Debug Info")]
        public bool showDebugInfo = true;
        public bool showDetectionRange = true;
        public Color debugColor = Color.orange;
        
        // Current state
        public EnemyState currentState { get; private set; } = EnemyState.Idle;
        public float currentHealth { get; private set; }
        
        // Private variables
        private Transform dragonTarget;
        private NavMeshAgent navAgent;
        private Animator animator;
        private AudioSource audioSource;
        private float lastAttackTime;
        private float lastPatrolTime;
        private int currentPatrolIndex = 0;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        
        // Animation parameters (if using Animator)
        private readonly int animSpeed = Animator.StringToHash("Speed");
        private readonly int animAttack = Animator.StringToHash("Attack");
        private readonly int animDead = Animator.StringToHash("Dead");
        
        void Start()
        {
            InitializeEnemy();
        }
        
        void InitializeEnemy()
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
            navAgent.stoppingDistance = stats.attackRange;
            
            // Store original position for returning
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            
            // Auto-find dragon target
            FindDragonTarget();
            
            // Set initial state
            if (patrolPoints != null && patrolPoints.Length > 0)
                ChangeState(EnemyState.Patrol);
            else
                ChangeState(EnemyState.Idle);
                
            Debug.Log($"👤 {stats.enemyName} '{name}' initialized - Type: {stats.enemyType}, Health: {stats.maxHealth}");
        }
        
        void Update()
        {
            if (currentState == EnemyState.Dead) return;
            
            // Auto-find dragon if we lost it
            if (dragonTarget == null)
                FindDragonTarget();
            
            // State machine
            switch (currentState)
            {
                case EnemyState.Idle:
                    HandleIdleState();
                    break;
                case EnemyState.Patrol:
                    HandlePatrolState();
                    break;
                case EnemyState.Tracking:
                    HandleTrackingState();
                    break;
                case EnemyState.Attacking:
                    HandleAttackingState();
                    break;
                case EnemyState.Retreating:
                    HandleRetreatingState();
                    break;
            }
            
            // Check for dragon detection
            CheckForDragonTarget();
            
            // Update animation
            UpdateAnimation();
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
                    return;
                }
            }
            
            // Fallback: Look for BasicFlightController component
            var flightController = FindObjectOfType<BasicFlightController>();
            if (flightController != null)
            {
                dragonTarget = flightController.transform;
            }
        }
        
        void CheckForDragonTarget()
        {
            if (dragonTarget == null) return;
            
            float distanceToTarget = Vector3.Distance(transform.position, dragonTarget.position);
            
            // If we see the dragon and aren't already tracking/attacking
            if (distanceToTarget <= stats.detectionRange && 
                currentState != EnemyState.Tracking && 
                currentState != EnemyState.Attacking)
            {
                // Alert! Found the dragon
                PlayAlertSound();
                ChangeState(EnemyState.Tracking);
            }
            // If we lose the dragon
            else if (distanceToTarget > stats.loseTargetRange && 
                     (currentState == EnemyState.Tracking || currentState == EnemyState.Attacking))
            {
                // Lost target, return to patrol or idle
                if (patrolPoints != null && patrolPoints.Length > 0)
                    ChangeState(EnemyState.Patrol);
                else
                    ChangeState(EnemyState.Idle);
            }
        }
        
        void HandleIdleState()
        {
            // Just stand around and look menacing
            if (navAgent.hasPath)
                navAgent.ResetPath();
        }
        
        void HandlePatrolState()
        {
            if (patrolPoints == null || patrolPoints.Length == 0) return;
            
            // Move to current patrol point
            navAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
            
            // Check if we reached the patrol point
            if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
            {
                // Wait at patrol point
                if (Time.time - lastPatrolTime > patrolWaitTime)
                {
                    // Move to next patrol point
                    if (patrolInOrder)
                    {
                        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                    }
                    else
                    {
                        currentPatrolIndex = UnityEngine.Random.Range(0, patrolPoints.Length);
                    }
                    lastPatrolTime = Time.time;
                }
            }
        }
        
        void HandleTrackingState()
        {
            if (dragonTarget == null) return;
            
            float distanceToTarget = Vector3.Distance(transform.position, dragonTarget.position);
            
            // Different behavior based on enemy type
            switch (stats.enemyType)
            {
                case EnemyType.Soldier:
                    // Soldiers try to get close for melee
                    navAgent.speed = stats.runSpeed;
                    navAgent.SetDestination(dragonTarget.position);
                    
                    if (distanceToTarget <= stats.attackRange)
                        ChangeState(EnemyState.Attacking);
                    break;
                    
                case EnemyType.Archer:
                    // Archers try to maintain preferred distance
                    if (distanceToTarget > stats.preferredDistance)
                    {
                        // Move closer
                        navAgent.speed = stats.moveSpeed;
                        navAgent.SetDestination(dragonTarget.position);
                    }
                    else if (distanceToTarget < stats.preferredDistance * 0.7f)
                    {
                        // Too close, retreat
                        ChangeState(EnemyState.Retreating);
                    }
                    else
                    {
                        // Perfect distance, start attacking
                        ChangeState(EnemyState.Attacking);
                    }
                    break;
                    
                case EnemyType.Guard:
                    // Guards don't move, just turn to face target
                    Vector3 direction = (dragonTarget.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 
                        stats.rotationSpeed * Time.deltaTime);
                    
                    if (distanceToTarget <= stats.attackRange)
                        ChangeState(EnemyState.Attacking);
                    break;
            }
        }
        
        void HandleAttackingState()
        {
            if (dragonTarget == null) return;
            
            float distanceToTarget = Vector3.Distance(transform.position, dragonTarget.position);
            
            // Stop moving and face target
            navAgent.ResetPath();
            Vector3 direction = (dragonTarget.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 
                stats.rotationSpeed * Time.deltaTime);
            
            // Check if still in range
            if (distanceToTarget > stats.attackRange && stats.enemyType != EnemyType.Archer)
            {
                ChangeState(EnemyState.Tracking);
                return;
            }
            
            // Attack on cooldown
            if (Time.time - lastAttackTime >= stats.attackCooldown)
            {
                PerformAttack();
                lastAttackTime = Time.time;
            }
        }
        
        void HandleRetreatingState()
        {
            if (dragonTarget == null) return;
            
            // Move away from dragon
            Vector3 retreatDirection = (transform.position - dragonTarget.position).normalized;
            Vector3 retreatPosition = transform.position + retreatDirection * 10f;
            
            navAgent.speed = stats.runSpeed;
            navAgent.SetDestination(retreatPosition);
            
            float distanceToTarget = Vector3.Distance(transform.position, dragonTarget.position);
            if (distanceToTarget >= stats.preferredDistance)
            {
                ChangeState(EnemyState.Tracking);
            }
        }
        
        void PerformAttack()
        {
            switch (stats.enemyType)
            {
                case EnemyType.Soldier:
                    PerformMeleeAttack();
                    break;
                case EnemyType.Archer:
                case EnemyType.Guard:
                    PerformRangedAttack();
                    break;
            }
            
            // Play attack animation
            if (animator != null)
                animator.SetTrigger(animAttack);
                
            PlayAttackSound();
        }
        
        void PerformMeleeAttack()
        {
            // For melee attacks, just deal damage if dragon is close enough
            if (dragonTarget != null)
            {
                float distance = Vector3.Distance(transform.position, dragonTarget.position);
                if (distance <= stats.attackRange)
                {
                    Debug.Log($"⚔️ {stats.enemyName} meleed dragon for {stats.attackDamage} damage!");
                    // TODO: Apply damage to dragon
                    // var dragonHealth = dragonTarget.GetComponent<DragonHealth>();
                    // if (dragonHealth != null) dragonHealth.TakeDamage(stats.attackDamage);
                }
            }
        }
        
        void PerformRangedAttack()
        {
            if (stats.projectilePrefab == null || stats.attackPoint == null || dragonTarget == null) 
                return;
            
            // Create projectile
            GameObject projectile = Instantiate(stats.projectilePrefab, 
                stats.attackPoint.position, stats.attackPoint.rotation);
            
            // Add velocity toward dragon
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
            {
                Vector3 direction = (dragonTarget.position - stats.attackPoint.position).normalized;
                projectileRb.linearVelocity = direction * 20f; // Adjust speed as needed
            }
            
            // Add damage component
            var projectileScript = projectile.GetComponent<EnemyProjectile>();
            if (projectileScript == null)
                projectileScript = projectile.AddComponent<EnemyProjectile>();
                
            projectileScript.damage = stats.attackDamage;
            projectileScript.shooter = this;
            
            Debug.Log($"🏹 {stats.enemyName} shot at dragon!");
        }
        
        public void TakeDamage(float damage)
        {
            if (currentState == EnemyState.Dead) return;
            
            // Apply armor reduction
            float actualDamage = Mathf.Max(1f, damage - stats.armor);
            currentHealth -= actualDamage;
            
            Debug.Log($"💔 {stats.enemyName} took {actualDamage} damage! Health: {currentHealth}/{stats.maxHealth}");
            
            // Play damage sound
            PlayDamageSound();
            
            // Check if dead
            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                // If not already fighting, start tracking the attacker
                if (currentState != EnemyState.Attacking && currentState != EnemyState.Tracking)
                {
                    ChangeState(EnemyState.Tracking);
                }
            }
        }
        
        void Die()
        {
            ChangeState(EnemyState.Dead);
            
            // Stop all movement
            if (navAgent != null)
                navAgent.enabled = false;
            
            // Play death animation
            if (animator != null)
                animator.SetBool(animDead, true);
            
            // Play death sound
            if (stats.deathSound != null && audioSource != null)
                audioSource.PlayOneShot(stats.deathSound);
            
            // Disable collider so dragon can pass through
            var collider = GetComponent<Collider>();
            if (collider != null)
                collider.enabled = false;
            
            Debug.Log($"💀 {stats.enemyName} has been defeated!");
            
            // TODO: Drop loot, update score, etc.
        }
        
        void ChangeState(EnemyState newState)
        {
            if (currentState == newState) return;
            
            Debug.Log($"👤 {stats.enemyName} state: {currentState} → {newState}");
            currentState = newState;
        }
        
        void UpdateAnimation()
        {
            if (animator == null) return;
            
            // Set speed parameter based on movement
            float speed = navAgent.velocity.magnitude;
            animator.SetFloat(animSpeed, speed);
        }
        
        void PlayAlertSound()
        {
            if (stats.alertSounds != null && stats.alertSounds.Length > 0 && audioSource != null)
            {
                AudioClip clip = stats.alertSounds[UnityEngine.Random.Range(0, stats.alertSounds.Length)];
                audioSource.PlayOneShot(clip);
            }
        }
        
        void PlayAttackSound()
        {
            if (stats.attackSounds != null && stats.attackSounds.Length > 0 && audioSource != null)
            {
                AudioClip clip = stats.attackSounds[UnityEngine.Random.Range(0, stats.attackSounds.Length)];
                audioSource.PlayOneShot(clip);
            }
        }
        
        void PlayDamageSound()
        {
            if (stats.damageSounds != null && stats.damageSounds.Length > 0 && audioSource != null)
            {
                AudioClip clip = stats.damageSounds[UnityEngine.Random.Range(0, stats.damageSounds.Length)];
                audioSource.PlayOneShot(clip);
            }
        }
        
        void OnDrawGizmosSelected()
        {
            if (!showDebugInfo) return;
            
            // Draw detection range
            if (showDetectionRange)
            {
                Gizmos.color = debugColor;
                Gizmos.DrawWireSphere(transform.position, stats.detectionRange);
                
                // Draw attack range
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, stats.attackRange);
            }
            
            // Draw line to current target
            if (dragonTarget != null && currentState == EnemyState.Tracking)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, dragonTarget.position);
            }
            
            // Draw patrol points
            if (patrolPoints != null)
            {
                Gizmos.color = Color.blue;
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    if (patrolPoints[i] != null)
                    {
                        Gizmos.DrawWireSphere(patrolPoints[i].position, 1f);
                        if (patrolInOrder && i < patrolPoints.Length - 1)
                        {
                            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                        }
                    }
                }
            }
        }
    }
    
    public class EnemyProjectile : MonoBehaviour
    {
        [HideInInspector] public float damage = 20f;
        [HideInInspector] public EnemyAI shooter;
        
        public float lifetime = 3f;
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
                Debug.Log($"💥 Dragon hit by {shooter?.stats.enemyName ?? "enemy"} projectile! Damage: {damage}");
                
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