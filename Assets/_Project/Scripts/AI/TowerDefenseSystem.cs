using UnityEngine;

namespace PlasmaDragon.Combat
{
    [System.Serializable]
    public class TowerStats
    {
        [Header("🏰 Tower Configuration")]
        public float detectionRange = 50f;
        public float fireRate = 1f; // shots per second
        public float projectileSpeed = 25f;
        public float damage = 10f;
        public LayerMask targetLayers = -1;
        
        [Header("🎯 Targeting")]
        public bool leadTarget = true; // Predict where dragon will be
        public float maxTurnSpeed = 90f; // degrees per second
        
        [Header("🔫 Projectile")]
        public GameObject projectilePrefab;
        public Transform firePoint;
        public ParticleSystem muzzleFlash;
        
        [Header("🔊 Audio")]
        public AudioClip fireSound;
        public AudioClip reloadSound;
    }

    public class TowerDefenseSystem : MonoBehaviour
    {
        [Header("🏰 TOWER DEFENSE SYSTEM")]
        public TowerStats stats = new TowerStats();
        
        [Header("🎯 Debug Info")]
        public bool showDebugInfo = true;
        public bool showDetectionRange = true;
        public Color debugColor = Color.red;
        
        // Private variables
        private Transform currentTarget;
        private float lastFireTime;
        private AudioSource audioSource;
        private Transform turretBarrel; // Child object that rotates to aim
        
        // Components that will be auto-found
        private Transform dragonTarget;
        
        void Start()
        {
            // Auto-find the dragon target
            FindDragonTarget();
            
            // Setup audio
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
                
            // Find turret barrel (look for child with "Barrel" in name)
            turretBarrel = transform.Find("Barrel") ?? transform.Find("Gun") ?? transform;
            
            Debug.Log($"🏰 Tower '{name}' initialized - Range: {stats.detectionRange}m, Fire Rate: {stats.fireRate}/sec");
        }
        
        void Update()
        {
            if (dragonTarget == null)
            {
                FindDragonTarget();
                return;
            }
            
            // Check if dragon is in range
            float distanceToTarget = Vector3.Distance(transform.position, dragonTarget.position);
            
            if (distanceToTarget <= stats.detectionRange)
            {
                currentTarget = dragonTarget;
                AimAtTarget();
                TryToFire();
            }
            else
            {
                currentTarget = null;
            }
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
                    Debug.Log($"🎯 Tower '{name}' found target: {dragonTarget.name}");
                    return;
                }
            }
            
            // Fallback: Look for BasicFlightController component
            var flightController = FindObjectOfType<BasicFlightController>();
            if (flightController != null)
            {
                dragonTarget = flightController.transform;
                Debug.Log($"🎯 Tower '{name}' found target via FlightController: {dragonTarget.name}");
            }
        }
        
        void AimAtTarget()
        {
            if (currentTarget == null || turretBarrel == null) return;
            
            Vector3 targetPosition = currentTarget.position;
            
            // Lead target prediction
            if (stats.leadTarget && currentTarget.GetComponent<Rigidbody>() != null)
            {
                Rigidbody targetRb = currentTarget.GetComponent<Rigidbody>();
                float timeToTarget = Vector3.Distance(transform.position, targetPosition) / stats.projectileSpeed;
                targetPosition += targetRb.linearVelocity * timeToTarget;
            }
            
            // Calculate aim direction
            Vector3 aimDirection = (targetPosition - turretBarrel.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
            
            // Smooth rotation
            float rotationSpeed = stats.maxTurnSpeed * Time.deltaTime;
            turretBarrel.rotation = Quaternion.RotateTowards(turretBarrel.rotation, targetRotation, rotationSpeed);
        }
        
        void TryToFire()
        {
            if (Time.time - lastFireTime < 1f / stats.fireRate) return;
            if (currentTarget == null) return;
            
            // Check if we're aimed close enough to fire
            Vector3 aimDirection = turretBarrel.forward;
            Vector3 targetDirection = (currentTarget.position - turretBarrel.position).normalized;
            float aimAccuracy = Vector3.Dot(aimDirection, targetDirection);
            
            if (aimAccuracy > 0.95f) // 95% accuracy required
            {
                Fire();
                lastFireTime = Time.time;
            }
        }
        
        void Fire()
        {
            if (stats.projectilePrefab == null || stats.firePoint == null) return;
            
            // Instantiate projectile
            GameObject projectile = Instantiate(stats.projectilePrefab, stats.firePoint.position, stats.firePoint.rotation);
            
            // Add velocity to projectile
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
            {
                projectileRb.linearVelocity = stats.firePoint.forward * stats.projectileSpeed;
            }
            
            // Add projectile component if needed
            var projectileScript = projectile.GetComponent<TowerProjectile>();
            if (projectileScript == null)
                projectileScript = projectile.AddComponent<TowerProjectile>();
            
            projectileScript.damage = stats.damage;
            projectileScript.shooter = this;
            
            // Visual and audio effects
            if (stats.muzzleFlash != null)
                stats.muzzleFlash.Play();
                
            if (stats.fireSound != null && audioSource != null)
                audioSource.PlayOneShot(stats.fireSound);
                
            Debug.Log($"🔫 Tower '{name}' fired at {currentTarget.name}!");
        }
        
        void OnDrawGizmosSelected()
        {
            if (!showDebugInfo) return;
            
            // Draw detection range
            if (showDetectionRange)
            {
                Gizmos.color = debugColor;
                Gizmos.DrawWireSphere(transform.position, stats.detectionRange);
            }
            
            // Draw line to current target
            if (currentTarget != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, currentTarget.position);
            }
        }
    }
    
    [System.Serializable]
    public class TowerProjectile : MonoBehaviour
    {
        [HideInInspector] public float damage = 10f;
        [HideInInspector] public TowerDefenseSystem shooter;
        
        public float lifetime = 5f;
        public GameObject explosionEffect;
        public LayerMask hitLayers = -1;
        
        void Start()
        {
            // Auto-destroy after lifetime
            Destroy(gameObject, lifetime);
        }
        
        void OnTriggerEnter(Collider other)
        {
            // Check if we hit the dragon
            if (other.GetComponent<BasicFlightController>() != null)
            {
                // Hit the dragon!
                Debug.Log($"💥 Dragon hit by tower projectile! Damage: {damage}");
                
                // Apply damage (you'll implement this later)
                // var dragonHealth = other.GetComponent<DragonHealth>();
                // if (dragonHealth != null) dragonHealth.TakeDamage(damage);
                
                CreateExplosion();
                Destroy(gameObject);
            }
            else if (((1 << other.gameObject.layer) & hitLayers) != 0)
            {
                // Hit something else (terrain, building)
                CreateExplosion();
                Destroy(gameObject);
            }
        }
        
        void CreateExplosion()
        {
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }
        }
    }
} 