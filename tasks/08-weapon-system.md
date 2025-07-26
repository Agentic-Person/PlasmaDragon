# Task 08: Weapon System Implementation

## Status: Pending

## Priority: MEDIUM - Core combat mechanic

## Description
Implement the dual weapon system with plasma orbs (area damage) and rapid fireballs (machine-gun style). This creates the satisfying power fantasy of being a dragon.

## Prerequisites
- Task 07 (Dragon Model) completed
- Fire points positioned on dragon
- Basic understanding of Unity projectiles

## Step-by-Step Instructions

### 1. Create Projectile Prefabs Folder Structure
```
_Project/Prefabs/Weapons/
├── Projectiles/
│   ├── PRE_PlasmaOrb.prefab
│   └── PRE_Fireball.prefab
├── Effects/
│   ├── PRE_PlasmaExplosion.prefab
│   └── PRE_FireballImpact.prefab
└── Pools/
    └── (Object pools later)
```

### 2. Create Plasma Orb Prefab
GameObject structure:
```
PRE_PlasmaOrb
├── Sphere (Mesh Renderer)
├── Trail Effect
├── Glow Effect
├── Collider (Sphere, Trigger)
└── Audio Source
```

Components and settings:
- **Transform**: Scale (0.5, 0.5, 0.5)
- **Sphere Collider**: 
  - Is Trigger: ✓
  - Radius: 1
- **Rigidbody**:
  - Use Gravity: ✗
  - Is Kinematic: ✗
  - Collision Detection: Continuous
- **Trail Renderer**:
  - Time: 0.5
  - Start Width: 0.5
  - End Width: 0
  - Material: Additive shader

### 3. Create Plasma Orb Material
1. Create new material: `MAT_PlasmaOrb`
2. Shader: Standard
3. Settings:
   - Albedo: Deep purple/blue
   - Emission: Bright blue/purple
   - Emission intensity: 2
   - Rendering Mode: Transparent

### 4. Plasma Orb Script
`Assets/_Project/Scripts/Combat/PlasmaOrb.cs`:
```csharp
using UnityEngine;
using System.Collections;

public class PlasmaOrb : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 30f;
    public float lifetime = 5f;
    
    [Header("Damage")]
    public float damage = 100f;
    public float explosionRadius = 10f;
    public LayerMask damageableLayers;
    
    [Header("Effects")]
    public GameObject explosionPrefab;
    public AudioClip chargeSound;
    public AudioClip explosionSound;
    
    private Rigidbody rb;
    private AudioSource audioSource;
    private bool hasExploded = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
        // Play charge sound
        if (chargeSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(chargeSound);
        }
        
        // Set velocity
        rb.velocity = transform.forward * speed;
        
        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return;
        
        // Ignore the dragon that fired it
        if (other.CompareTag("Player")) return;
        
        // Explode on contact
        Explode();
    }
    
    void Explode()
    {
        hasExploded = true;
        
        // Visual explosion
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, 
                transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }
        
        // Audio
        if (explosionSound != null && audioSource != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }
        
        // Deal area damage
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position, explosionRadius, damageableLayers);
        
        foreach (Collider hit in hitColliders)
        {
            // Apply damage to enemies
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float distance = Vector3.Distance(transform.position, 
                    hit.transform.position);
                float falloff = 1f - (distance / explosionRadius);
                damageable.TakeDamage(damage * falloff);
            }
            
            // Apply force to rigidbodies
            Rigidbody hitRb = hit.GetComponent<Rigidbody>();
            if (hitRb != null)
            {
                hitRb.AddExplosionForce(500f, transform.position, 
                    explosionRadius, 1f);
            }
        }
        
        // Destroy projectile
        Destroy(gameObject);
    }
    
    void OnDrawGizmosSelected()
    {
        // Visualize explosion radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
```

### 5. Create Fireball Prefab
Simpler, faster projectile:
```
PRE_Fireball
├── Sphere (Mesh Renderer, scale 0.2)
├── Trail Effect (shorter)
├── Collider (Sphere, Trigger)
└── Light (optional)
```

### 6. Fireball Script
`Assets/_Project/Scripts/Combat/Fireball.cs`:
```csharp
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 50f;
    public float lifetime = 3f;
    
    [Header("Damage")]
    public float damage = 25f;
    
    [Header("Effects")]
    public GameObject impactPrefab;
    public AudioClip fireSound;
    public AudioClip impactSound;
    
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        
        // Play fire sound
        if (fireSound != null)
        {
            AudioSource.PlayClipAtPoint(fireSound, transform.position, 0.5f);
        }
        
        Destroy(gameObject, lifetime);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Ignore player
        if (other.CompareTag("Player")) return;
        
        // Deal damage
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
        
        // Impact effect
        if (impactPrefab != null)
        {
            GameObject impact = Instantiate(impactPrefab, 
                transform.position, Quaternion.identity);
            Destroy(impact, 1f);
        }
        
        // Impact sound
        if (impactSound != null)
        {
            AudioSource.PlayClipAtPoint(impactSound, transform.position, 0.7f);
        }
        
        // Destroy projectile
        Destroy(gameObject);
    }
}
```

### 7. Create Damage Interface
`Assets/_Project/Scripts/Combat/IDamageable.cs`:
```csharp
public interface IDamageable
{
    void TakeDamage(float damage);
    void Die();
    float GetHealth();
    float GetMaxHealth();
}
```

### 8. Dragon Weapon Controller
`Assets/_Project/Scripts/Player/DragonWeaponController.cs`:
```csharp
using UnityEngine;
using System.Collections;

public class DragonWeaponController : MonoBehaviour
{
    [Header("Weapon Prefabs")]
    public GameObject plasmaOrbPrefab;
    public GameObject fireballPrefab;
    
    [Header("Fire Points")]
    public Transform primaryFirePoint;
    public Transform secondaryFirePoint;
    
    [Header("Weapon Stats")]
    public float plasmaOrbCooldown = 2f;
    public float fireballFireRate = 5f; // shots per second
    
    [Header("Effects")]
    public ParticleSystem chargingEffect;
    public AudioClip chargeUpSound;
    
    private float nextPlasmaTime = 0f;
    private float fireballInterval;
    private bool isFiringRapid = false;
    private Coroutine rapidFireCoroutine;
    private AudioSource audioSource;
    
    void Start()
    {
        fireballInterval = 1f / fireballFireRate;
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        // Primary weapon - Plasma Orb
        if (Input.GetMouseButtonDown(0) && Time.time >= nextPlasmaTime)
        {
            FirePlasmaOrb();
        }
        
        // Secondary weapon - Rapid Fireballs
        if (Input.GetMouseButtonDown(1))
        {
            StartRapidFire();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopRapidFire();
        }
    }
    
    void FirePlasmaOrb()
    {
        if (plasmaOrbPrefab == null || primaryFirePoint == null) return;
        
        // Charge effect
        if (chargingEffect != null)
        {
            chargingEffect.Play();
        }
        
        // Charge sound
        if (chargeUpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(chargeUpSound);
        }
        
        // Create projectile
        GameObject orb = Instantiate(plasmaOrbPrefab, 
            primaryFirePoint.position, 
            primaryFirePoint.rotation);
        
        // Set cooldown
        nextPlasmaTime = Time.time + plasmaOrbCooldown;
        
        // Trigger animation
        GetComponent<DragonAnimationController>()?.OnAttackHit();
    }
    
    void StartRapidFire()
    {
        if (isFiringRapid) return;
        
        isFiringRapid = true;
        rapidFireCoroutine = StartCoroutine(RapidFireCoroutine());
    }
    
    void StopRapidFire()
    {
        isFiringRapid = false;
        
        if (rapidFireCoroutine != null)
        {
            StopCoroutine(rapidFireCoroutine);
            rapidFireCoroutine = null;
        }
    }
    
    IEnumerator RapidFireCoroutine()
    {
        while (isFiringRapid)
        {
            FireSingleFireball();
            yield return new WaitForSeconds(fireballInterval);
        }
    }
    
    void FireSingleFireball()
    {
        if (fireballPrefab == null || secondaryFirePoint == null) return;
        
        // Alternate between fire points for visual variety
        Vector3 offset = Random.insideUnitCircle * 0.2f;
        Vector3 firePosition = secondaryFirePoint.position + 
            secondaryFirePoint.TransformDirection(offset);
        
        GameObject fireball = Instantiate(fireballPrefab, 
            firePosition, 
            secondaryFirePoint.rotation);
    }
    
    // UI Helper methods
    public float GetPlasmaOrbCooldownPercent()
    {
        float remaining = nextPlasmaTime - Time.time;
        return 1f - Mathf.Clamp01(remaining / plasmaOrbCooldown);
    }
    
    public bool IsPlasmaReady()
    {
        return Time.time >= nextPlasmaTime;
    }
}
```

### 9. Create Explosion Effects
Using Unity Particle System:
1. Create new GameObject: `PRE_PlasmaExplosion`
2. Add Particle System component
3. Configure:
   - Duration: 0.5
   - Start Lifetime: 0.5-1
   - Start Speed: 10-20
   - Start Size: 0.5-2
   - Emission: Burst of 50-100
   - Shape: Sphere
   - Color over Lifetime: Blue to transparent
   - Size over Lifetime: Grow then shrink

### 10. Object Pooling (Performance)
`Assets/_Project/Scripts/Utilities/ProjectilePool.cs`:
```csharp
using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    
    private static ProjectilePool instance;
    public static ProjectilePool Instance => instance;
    
    void Awake()
    {
        instance = this;
        
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            
            poolDictionary.Add(pool.tag, objectPool);
        }
    }
    
    public GameObject SpawnFromPool(string tag, Vector3 position, 
        Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist!");
            return null;
        }
        
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        
        poolDictionary[tag].Enqueue(objectToSpawn);
        
        return objectToSpawn;
    }
}
```

## Expected Outcomes
- ✅ Plasma orbs fire with 2-second cooldown
- ✅ Area damage clears groups of enemies
- ✅ Fireballs fire continuously when holding right-click
- ✅ Visual effects enhance impact
- ✅ Sound effects provide feedback
- ✅ Performance remains stable
- ✅ Combat feels powerful and satisfying

## Balancing Guidelines
- Plasma Orb: One-shots basic enemies, high impact
- Fireballs: 4 hits to kill basic enemy, spam-friendly
- Visual feedback must be clear
- Audio must not be annoying with rapid fire

## Common Issues & Solutions

### Issue: Projectiles Not Firing
- Check fire point references
- Verify prefabs assigned
- Check input detection
- Ensure no null references

### Issue: Performance Drop with Many Projectiles
- Implement object pooling
- Reduce particle count
- Simplify trail renderers
- Limit max projectiles

### Issue: Projectiles Going Through Enemies
- Enable continuous collision
- Increase collider size
- Reduce projectile speed
- Check layer settings

## Time Estimate: 3-4 hours

## Next Steps
Proceed to Task 09: Enemy System Setup

---

## Completion Notes
*To be filled after task completion*

**Date Completed**: 
**Time Taken**: 
**Weapon Feel**: 
**Performance Impact**: 
**Balance Notes**: 
**Visual Polish**: 