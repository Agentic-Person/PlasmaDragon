# Task 07: Dragon Model Integration

## Status: Pending

## Priority: HIGH - Visual core of the game

## Description
Import and configure a dragon model with animations, replacing the placeholder cube with an actual dragon that has flying, attacking, and idle animations.

## Prerequisites
- Task 06 (Flight Controller) completed
- Dragon controller GameObject set up
- Unity Asset Store or 3D model source ready

## Step-by-Step Instructions

### 1. Dragon Asset Options

#### Option A: Free Unity Asset Store Dragons
Search for these free options:
- "Dragon for Boss Monster: HP" (FREE)
- "Mini Dragon" (often free)
- "Low Poly Dragon" (free versions)
- "Dragon Lite" packages

#### Option B: Affordable Purchase ($20-40)
Recommended paid options:
- "Animated Dragon" by Mesh Terrain
- "Flying Dragon" by Polygon Blacksmith
- "Low Poly Dragon Pack" 
- Ensure includes: Fly, Attack, Idle, Death animations

#### Option C: 3D AI Generation (Backup)
If no suitable asset found:
1. Use 3D AI Studio or Meshy AI
2. Generate with prompt: "Low poly dragon suitable for flying game"
3. Export as FBX
4. Apply Mixamo animations

### 2. Import Dragon Asset
1. Download chosen dragon asset
2. Import to: `Assets/_ImportedAssets/Dragon/`
3. Import settings:
   - Model: 
     - Scale Factor: 1
     - Generate Colliders: ✓
     - Animation Type: Humanoid or Generic
   - Rig:
     - Animation Type: Generic (usually)
     - Avatar Definition: Create From This Model
   - Animations:
     - Import Animations: ✓
     - Bake Animations: ✓

### 3. Create Dragon Prefab Structure
Replace placeholder cube:
```
Dragon_Controller
├── DragonModel (imported model)
│   ├── Mesh
│   ├── Armature/Bones
│   └── Animator Component
├── Colliders
│   ├── BodyCollider (Capsule)
│   └── WingColliders (Boxes)
├── CameraTarget
├── FirePoints
│   ├── FirePoint_Primary (mouth position)
│   └── FirePoint_Secondary
└── Effects
    ├── WingTrails
    └── BreathEffect
```

### 4. Configure Animator Controller
1. Create Animator Controller: `AC_Dragon`
2. Save in: `_Project/Animations/Controllers/`
3. Set up states:
   - Idle (default)
   - Flying (main state)
   - FlyingFast (boost)
   - Attack01 (plasma orb)
   - Attack02 (rapid fire)
   - Death

### 5. Animation State Machine
```
Parameters:
- Speed (float): 0-1
- IsFlying (bool)
- IsBoosting (bool)
- Attack (trigger)
- RapidFire (bool)

Transitions:
- Any State → Flying (IsFlying = true)
- Flying → FlyingFast (IsBoosting = true)
- Flying → Attack01 (Attack trigger)
- Flying → Idle (IsFlying = false)
```

### 6. Create Dragon Animation Controller Script
`Assets/_Project/Scripts/Player/DragonAnimationController.cs`:
```csharp
using UnityEngine;

public class DragonAnimationController : MonoBehaviour
{
    [Header("References")]
    public Animator dragonAnimator;
    public DragonFlightController flightController;
    
    [Header("Animation Settings")]
    public float speedSmoothTime = 0.1f;
    public float animationSpeedMultiplier = 1f;
    
    private float currentAnimSpeed;
    private bool wasFlying = false;
    
    void Start()
    {
        if (dragonAnimator == null)
            dragonAnimator = GetComponentInChildren<Animator>();
            
        if (flightController == null)
            flightController = GetComponent<DragonFlightController>();
    }
    
    void Update()
    {
        // Calculate animation speed based on movement
        float targetSpeed = flightController.GetSpeedPercentage();
        currentAnimSpeed = Mathf.SmoothDamp(
            currentAnimSpeed, 
            targetSpeed, 
            ref currentAnimSpeed, 
            speedSmoothTime);
        
        // Update animator parameters
        dragonAnimator.SetFloat("Speed", currentAnimSpeed);
        dragonAnimator.SetBool("IsFlying", currentAnimSpeed > 0.1f);
        dragonAnimator.SetBool("IsBoosting", flightController.IsBoosting());
        
        // Adjust animation playback speed
        dragonAnimator.speed = Mathf.Lerp(0.8f, 1.3f, currentAnimSpeed) 
            * animationSpeedMultiplier;
        
        // Handle attack animations
        if (Input.GetMouseButtonDown(0))
        {
            dragonAnimator.SetTrigger("Attack");
        }
        
        dragonAnimator.SetBool("RapidFire", Input.GetMouseButton(1));
        
        // Landing/Takeoff events
        bool isFlying = currentAnimSpeed > 0.1f;
        if (isFlying != wasFlying)
        {
            if (isFlying)
                OnTakeoff();
            else
                OnLanding();
            wasFlying = isFlying;
        }
    }
    
    void OnTakeoff()
    {
        // Play takeoff effects
        Debug.Log("Dragon taking off!");
    }
    
    void OnLanding()
    {
        // Play landing effects
        Debug.Log("Dragon landing!");
    }
    
    // Animation event callbacks
    public void OnAttackHit()
    {
        // Called from animation event
        // Spawn projectile here
    }
    
    public void OnWingFlap()
    {
        // Called from animation event
        // Play wing sound effect
    }
}
```

### 7. Set Up Colliders
1. Add Capsule Collider to dragon body:
   - Center: (0, 0, 0)
   - Radius: 1
   - Height: 4
   - Direction: Z-Axis

2. Add Box Colliders for wings (optional):
   - For better collision detection
   - Set as triggers for effects

### 8. Configure Materials
1. Check dragon materials
2. Ensure using mobile-friendly shaders:
   - Standard shader for most parts
   - Emission for eyes/effects
   - Keep texture sizes reasonable (1024x1024 max)

### 9. Add Dragon Effects
Wing trail setup:
```csharp
public class DragonWingTrails : MonoBehaviour
{
    public TrailRenderer leftWingTrail;
    public TrailRenderer rightWingTrail;
    public Gradient normalGradient;
    public Gradient boostGradient;
    
    private DragonFlightController flightController;
    
    void Start()
    {
        flightController = GetComponentInParent<DragonFlightController>();
        
        // Configure trail renderers
        ConfigureTrail(leftWingTrail);
        ConfigureTrail(rightWingTrail);
    }
    
    void ConfigureTrail(TrailRenderer trail)
    {
        trail.time = 0.5f;
        trail.startWidth = 0.5f;
        trail.endWidth = 0.1f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
    }
    
    void Update()
    {
        float speed = flightController.GetSpeedPercentage();
        bool boosting = flightController.IsBoosting();
        
        // Adjust trail properties
        leftWingTrail.time = Mathf.Lerp(0.1f, 1f, speed);
        rightWingTrail.time = Mathf.Lerp(0.1f, 1f, speed);
        
        // Change color when boosting
        Gradient gradient = boosting ? boostGradient : normalGradient;
        leftWingTrail.colorGradient = gradient;
        rightWingTrail.colorGradient = gradient;
    }
}
```

### 10. Final Integration Checklist
- [ ] Dragon model replaces placeholder
- [ ] All animations play correctly
- [ ] No animation glitches
- [ ] Colliders properly sized
- [ ] Fire points positioned at mouth
- [ ] Wing trails look good
- [ ] Scale appropriate for world
- [ ] Performance still 60+ FPS

## Expected Outcomes
- ✅ Dragon model integrated and animated
- ✅ Smooth transitions between animations
- ✅ Attack animations trigger correctly
- ✅ Visual effects enhance movement
- ✅ Dragon feels powerful and responsive
- ✅ No performance degradation
- ✅ Professional appearance

## Common Issues & Solutions

### Issue: Animation Not Playing
- Check Animator Controller setup
- Verify parameter names match
- Ensure Has Exit Time unchecked
- Check animation import settings

### Issue: Dragon Wrong Size
- Adjust model import scale
- Scale parent GameObject
- Reconfigure colliders
- Adjust camera distance

### Issue: Animations Look Bad
- Check animation compression
- Verify rig setup correct
- Adjust animation speed
- Enable root motion if needed

### Issue: Performance Drop
- Reduce polygon count
- Optimize textures
- Simplify materials
- Use LOD system

## Dragon Specifications
- Polygon count: < 10,000 (ideally 5,000)
- Texture size: 1024x1024 maximum
- Animation count: 5-8 minimum
- Bone count: < 50 for mobile

## Time Estimate: 2-3 hours

## Next Steps
Proceed to Task 08: Weapon System Implementation

---

## Completion Notes
*To be filled after task completion*

**Date Completed**: 
**Time Taken**: 
**Dragon Asset Used**: 
**Polygon Count**: 
**Animation Count**: 
**Performance Impact**: 