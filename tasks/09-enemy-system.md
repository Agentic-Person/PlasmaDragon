# Task 09: Enemy System Setup

## Status: Pending

## Priority: MEDIUM - Core gameplay targets

## Description
Create basic enemy types (archers, ballistas, military patrols) that provide satisfying targets for the dragon's weapons. Enemies should be easy to destroy to maintain the power fantasy.

## Prerequisites
- Task 08 (Weapon System) completed
- IDamageable interface created
- Basic understanding of AI behavior

## Step-by-Step Instructions

### 1. Create Enemy Prefab Structure
```
_Project/Prefabs/Enemies/
├── Turrets/
│   ├── PRE_ArcherTower.prefab
│   └── PRE_BallistaTurret.prefab
├── Units/
│   ├── PRE_Soldier.prefab
│   └── PRE_MilitaryPatrol.prefab
└── Bosses/
    └── PRE_EliteCommander.prefab (later)
```

### 2. Create Base Enemy Script
`Assets/_Project/Scripts/AI/BaseEnemy.cs`:
```csharp
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isInvulnerable = false;
    
    [Header("Death")]
    public GameObject deathEffectPrefab;
    public float deathDelay = 0f;
    public AudioClip deathSound;
    
    [Header("Events")]
    public UnityEvent onSpawn;
    public UnityEvent onDamage;
    public UnityEvent onDeath;
    
    protected bool isDead = false;
    protected AudioSource audioSource;
    
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        onSpawn?.Invoke();
    }
    
    public virtual void TakeDamage(float damage)
    {
        if (isDead || isInvulnerable) return;
        
        currentHealth -= damage;
        onDamage?.Invoke();
        
        // Visual feedback
        StartCoroutine(DamageFlash());
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        
        onDeath?.Invoke();
        
        // Death effect
        if (deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(deathEffectPrefab, 
                transform.position, Quaternion.identity);
            Destroy(effect, 3f);
        }
        
        // Death sound
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }
        
        // Destroy
        Destroy(gameObject, deathDelay);
    }
    
    System.Collections.IEnumerator DamageFlash()
    {
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            Color originalColor = mat.color;
            mat.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            mat.color = originalColor;
        }
    }
    
    public float GetHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
```

### 3. Create Archer Tower
`Assets/_Project/Scripts/AI/ArcherTower.cs`:
```csharp
using UnityEngine;

public class ArcherTower : BaseEnemy
{
    [Header("Archer Settings")]
    public float detectionRange = 50f;
    public float fireRate = 1f; // shots per second
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    
    [Header("Targeting")]
    public float leadTargetMultiplier = 0.5f;
    public LayerMask targetLayer;
    
    private Transform target;
    private float nextFireTime;
    private Transform turretHead;
    
    protected override void Start()
    {
        base.Start();
        maxHealth = 1f; // One-shot by plasma orb
        currentHealth = maxHealth;
        
        // Find turret head for rotation
        turretHead = transform.Find("TurretHead") ?? transform;
    }
    
    void Update()
    {
        if (isDead) return;
        
        // Find target
        FindTarget();
        
        if (target != null)
        {
            // Rotate towards target
            RotateTowardsTarget();
            
            // Fire if ready
            if (Time.time >= nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + (1f / fireRate);
            }
        }
    }
    
    void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position, detectionRange, targetLayer);
        
        float closestDistance = detectionRange;
        Transform closestTarget = null;
        
        foreach (Collider col in colliders)
        {
            float distance = Vector3.Distance(transform.position, 
                col.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = col.transform;
            }
        }
        
        target = closestTarget;
    }
    
    void RotateTowardsTarget()
    {
        if (target == null) return;
        
        // Calculate lead position
        Vector3 targetPosition = target.position;
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        
        if (targetRb != null)
        {
            float timeToTarget = Vector3.Distance(transform.position, 
                target.position) / projectileSpeed;
            targetPosition += targetRb.velocity * timeToTarget * 
                leadTargetMultiplier;
        }
        
        // Rotate turret
        Vector3 direction = targetPosition - turretHead.position;
        direction.y = 0; // Keep horizontal
        
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            turretHead.rotation = Quaternion.Slerp(turretHead.rotation, 
                targetRotation, Time.deltaTime * 5f);
        }
    }
    
    void Fire()
    {
        if (arrowPrefab == null || firePoint == null) return;
        
        GameObject arrow = Instantiate(arrowPrefab, 
            firePoint.position, firePoint.rotation);
        
        // Set arrow velocity
        Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
        if (arrowRb != null)
        {
            arrowRb.velocity = firePoint.forward * projectileSpeed;
        }
        
        // Destroy arrow after time
        Destroy(arrow, 5f);
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
```

### 4. Create Ballista Turret
`Assets/_Project/Scripts/AI/BallistaTurret.cs`:
```csharp
using UnityEngine;

public class BallistaTurret : BaseEnemy
{
    [Header("Ballista Settings")]
    public float detectionRange = 70f;
    public float fireRate = 0.3f; // slower but more powerful
    public GameObject boltPrefab;
    public Transform firePoint;
    public float projectileSpeed = 40f;
    public float projectileDamage = 50f;
    
    [Header("Targeting")]
    public float leadTargetMultiplier = 0.8f; // Better prediction
    public float chargeTime = 1f; // Visual charge before firing
    public ParticleSystem chargeEffect;
    
    private Transform target;
    private float nextFireTime;
    private bool isCharging = false;
    
    protected override void Start()
    {
        base.Start();
        maxHealth = 100f; // Takes 1 plasma orb or 4 fireballs
        currentHealth = maxHealth;
    }
    
    void Update()
    {
        if (isDead) return;
        
        FindTarget();
        
        if (target != null)
        {
            AimAtTarget();
            
            if (!isCharging && Time.time >= nextFireTime)
            {
                StartCoroutine(ChargeAndFire());
            }
        }
    }
    
    void FindTarget()
    {
        // Similar to archer but with longer range
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, 
                player.transform.position);
            if (distance <= detectionRange)
            {
                target = player.transform;
            }
            else
            {
                target = null;
            }
        }
    }
    
    void AimAtTarget()
    {
        // More sophisticated aiming with elevation
        if (target == null) return;
        
        Vector3 targetPos = PredictTargetPosition();
        Vector3 direction = targetPos - firePoint.position;
        
        // Calculate ballistic trajectory
        float distance = Vector3.Distance(transform.position, targetPos);
        float elevationAngle = CalculateBallisticAngle(distance);
        
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation *= Quaternion.Euler(-elevationAngle, 0, 0);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, 
            targetRotation, Time.deltaTime * 3f);
    }
    
    Vector3 PredictTargetPosition()
    {
        Vector3 targetPos = target.position;
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        
        if (targetRb != null)
        {
            float timeToTarget = Vector3.Distance(firePoint.position, 
                target.position) / projectileSpeed;
            targetPos += targetRb.velocity * timeToTarget * leadTargetMultiplier;
        }
        
        return targetPos;
    }
    
    float CalculateBallisticAngle(float distance)
    {
        // Simple ballistic calculation
        return Mathf.Clamp(distance * 0.1f, 0f, 45f);
    }
    
    System.Collections.IEnumerator ChargeAndFire()
    {
        isCharging = true;
        
        // Visual charge effect
        if (chargeEffect != null)
        {
            chargeEffect.Play();
        }
        
        // Audio cue
        if (audioSource != null)
        {
            audioSource.Play();
        }
        
        yield return new WaitForSeconds(chargeTime);
        
        // Fire
        if (target != null && !isDead)
        {
            Fire();
        }
        
        if (chargeEffect != null)
        {
            chargeEffect.Stop();
        }
        
        isCharging = false;
        nextFireTime = Time.time + (1f / fireRate);
    }
    
    void Fire()
    {
        GameObject bolt = Instantiate(boltPrefab, 
            firePoint.position, firePoint.rotation);
        
        // More powerful projectile
        BallistaBolt boltScript = bolt.GetComponent<BallistaBolt>();
        if (boltScript != null)
        {
            boltScript.damage = projectileDamage;
            boltScript.speed = projectileSpeed;
        }
    }
}
```

### 5. Create Military Patrol
`Assets/_Project/Scripts/AI/MilitaryPatrol.cs`:
```csharp
using UnityEngine;
using System.Collections.Generic;

public class MilitaryPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public GameObject soldierPrefab;
    public int patrolSize = 8;
    public float spacing = 2f;
    public float moveSpeed = 5f;
    
    [Header("Formation")]
    public enum FormationType { Line, Square, Circle }
    public FormationType formation = FormationType.Square;
    
    [Header("Patrol Route")]
    public Transform[] waypoints;
    public float waypointRadius = 2f;
    
    private List<GameObject> soldiers = new List<GameObject>();
    private int currentWaypoint = 0;
    
    void Start()
    {
        SpawnPatrol();
    }
    
    void SpawnPatrol()
    {
        for (int i = 0; i < patrolSize; i++)
        {
            Vector3 position = GetFormationPosition(i);
            GameObject soldier = Instantiate(soldierPrefab, 
                transform.position + position, Quaternion.identity, transform);
            
            // Configure soldier
            Soldier soldierScript = soldier.GetComponent<Soldier>();
            if (soldierScript != null)
            {
                soldierScript.patrol = this;
                soldierScript.formationIndex = i;
            }
            
            soldiers.Add(soldier);
        }
    }
    
    Vector3 GetFormationPosition(int index)
    {
        switch (formation)
        {
            case FormationType.Line:
                return new Vector3(index * spacing - (patrolSize * spacing / 2f), 
                    0, 0);
                
            case FormationType.Square:
                int gridSize = Mathf.CeilToInt(Mathf.Sqrt(patrolSize));
                int x = index % gridSize;
                int z = index / gridSize;
                return new Vector3(x * spacing - (gridSize * spacing / 2f), 
                    0, z * spacing - (gridSize * spacing / 2f));
                
            case FormationType.Circle:
                float angle = (360f / patrolSize) * index;
                float radius = spacing * patrolSize / (2 * Mathf.PI);
                return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, 
                    0, Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
                
            default:
                return Vector3.zero;
        }
    }
    
    void Update()
    {
        if (waypoints.Length == 0) return;
        
        // Move patrol
        Transform targetWaypoint = waypoints[currentWaypoint];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Check if reached waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) 
            < waypointRadius)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
        
        // Update formation
        UpdateFormation();
    }
    
    void UpdateFormation()
    {
        // Keep soldiers in formation
        for (int i = 0; i < soldiers.Count; i++)
        {
            if (soldiers[i] != null)
            {
                Vector3 targetPos = transform.position + GetFormationPosition(i);
                soldiers[i].transform.position = Vector3.Lerp(
                    soldiers[i].transform.position, 
                    targetPos, 
                    Time.deltaTime * 5f);
            }
        }
    }
    
    public void OnSoldierKilled(GameObject soldier)
    {
        soldiers.Remove(soldier);
        
        // If all soldiers dead, destroy patrol
        if (soldiers.Count == 0)
        {
            Destroy(gameObject);
        }
    }
}
```

### 6. Create Individual Soldier
`Assets/_Project/Scripts/AI/Soldier.cs`:
```csharp
using UnityEngine;

public class Soldier : BaseEnemy
{
    [Header("Soldier Settings")]
    public float shootRange = 30f;
    public float shootInterval = 2f;
    public GameObject bulletPrefab;
    public Transform gunPoint;
    
    [HideInInspector]
    public MilitaryPatrol patrol;
    [HideInInspector]
    public int formationIndex;
    
    private Transform target;
    private float nextShootTime;
    
    protected override void Start()
    {
        base.Start();
        maxHealth = 1f; // One-shot kill
        currentHealth = maxHealth;
    }
    
    void Update()
    {
        if (isDead) return;
        
        // Look for dragon
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, 
                player.transform.position);
            
            if (distance <= shootRange)
            {
                target = player.transform;
                
                // Rotate to face target
                Vector3 lookDir = target.position - transform.position;
                lookDir.y = 0;
                transform.rotation = Quaternion.LookRotation(lookDir);
                
                // Shoot
                if (Time.time >= nextShootTime)
                {
                    Shoot();
                    nextShootTime = Time.time + shootInterval;
                }
            }
        }
    }
    
    void Shoot()
    {
        if (bulletPrefab != null && gunPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, 
                gunPoint.position, gunPoint.rotation);
            
            // Simple bullet
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = gunPoint.forward * 20f;
            }
            
            Destroy(bullet, 3f);
        }
    }
    
    public override void Die()
    {
        base.Die();
        
        // Notify patrol
        if (patrol != null)
        {
            patrol.OnSoldierKilled(gameObject);
        }
    }
}
```

### 7. Create Enemy Projectiles
Simple arrow/bullet prefab with damage script:
```csharp
public class EnemyProjectile : MonoBehaviour
{
    public float damage = 10f;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Deal damage to player
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            
            Destroy(gameObject);
        }
    }
}
```

### 8. Enemy Spawn Manager
`Assets/_Project/Scripts/AI/EnemySpawnManager.cs`:
```csharp
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform position;
        public GameObject enemyPrefab;
        public bool isActive = true;
    }
    
    public List<SpawnPoint> spawnPoints;
    public float respawnDelay = 30f;
    public bool autoRespawn = false;
    
    void Start()
    {
        SpawnAllEnemies();
    }
    
    void SpawnAllEnemies()
    {
        foreach (var spawn in spawnPoints)
        {
            if (spawn.isActive && spawn.enemyPrefab != null)
            {
                GameObject enemy = Instantiate(spawn.enemyPrefab, 
                    spawn.position.position, spawn.position.rotation);
                
                // Track for respawn
                if (autoRespawn)
                {
                    BaseEnemy enemyScript = enemy.GetComponent<BaseEnemy>();
                    if (enemyScript != null)
                    {
                        enemyScript.onDeath.AddListener(() => 
                            StartCoroutine(RespawnEnemy(spawn)));
                    }
                }
            }
        }
    }
    
    System.Collections.IEnumerator RespawnEnemy(SpawnPoint spawn)
    {
        yield return new WaitForSeconds(respawnDelay);
        
        if (spawn.isActive)
        {
            Instantiate(spawn.enemyPrefab, 
                spawn.position.position, spawn.position.rotation);
        }
    }
}
```

## Expected Outcomes
- ✅ Archer towers shoot at dragon when in range
- ✅ Ballista turrets predict movement
- ✅ Military patrols move in formation
- ✅ All enemies die to single plasma orb
- ✅ Satisfying death effects
- ✅ Clear visual feedback
- ✅ Performance stable with 20-30 enemies

## Enemy Placement Guidelines
- Archers on walls and towers
- Ballistas at key defensive points
- Patrols along paths
- Groups close enough for AoE
- Clear sightlines for player
- Variety in positioning

## Common Issues & Solutions

### Issue: Enemies Not Detecting Player
- Check player tag is "Player"
- Verify detection range
- Check layer masks
- Ensure colliders present

### Issue: Projectiles Missing Player
- Adjust lead calculation
- Increase projectile speed
- Check prediction math
- Add slight homing

### Issue: Too Many Enemies Tank Performance
- Use object pooling
- Reduce update frequency
- Simplify AI calculations
- LOD for distant enemies

## Time Estimate: 4-5 hours

## Next Steps
Proceed to Task 10: AI Boss Implementation

---

## Completion Notes
*To be filled after task completion*

**Date Completed**: 
**Time Taken**: 
**Enemy Count**: 
**Balance Notes**: 
**Performance Impact**: 
**Visual Polish**: 