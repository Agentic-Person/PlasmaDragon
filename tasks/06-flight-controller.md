# Task 06: Flight Controller Implementation

## Status: Pending

## Priority: HIGH - Core gameplay mechanic

## Description
Import and configure the 3rd Person Controller + Fly Mode asset to create smooth, responsive dragon flight controls. This is the foundation of all gameplay.

## Prerequisites
- Task 05 (Environment Setup) completed
- Unity Asset Store account
- Basic understanding of Unity Input System

## Step-by-Step Instructions

### 1. Import Flight Controller Asset
1. Open Unity Asset Store (in browser)
2. Search for "3rd Person Controller + Fly Mode" (Asset ID: 28647)
3. Add to My Assets (FREE)
4. Open Package Manager > My Assets
5. Download and Import
6. Import to: `Assets/_ImportedAssets/FlightController/`

### 2. Create Dragon GameObject Structure
In Level01_FortressIsland scene:
```
Dragon_Controller (Empty GameObject)
├── Dragon_Model (placeholder cube for now)
│   └── Mesh Renderer
├── CameraTarget (Empty, offset: 0, 2, 0)
├── FirePoint_Primary (Empty, offset: 0, 0, 3)
├── FirePoint_Secondary (Empty, offset: 0, -0.5, 3)
└── Effects (Empty)
    ├── EngineTrail
    └── WindEffect
```

### 3. Apply Flight Controller
1. Add flight controller script to Dragon_Controller
2. Configure basic parameters:
   - Move Speed: 15
   - Sprint Speed: 30
   - Turn Speed: 2
   - Tilt Amount: 30
3. Add Rigidbody:
   - Mass: 10
   - Drag: 2
   - Angular Drag: 5
   - Use Gravity: ✓
   - Constraints: None

### 4. Enhance with Dragon-Specific Controls
Create `Assets/_Project/Scripts/Player/DragonFlightController.cs`:
```csharp
using UnityEngine;

public class DragonFlightController : MonoBehaviour
{
    [Header("Flight Parameters")]
    public float normalSpeed = 20f;
    public float boostSpeed = 40f;
    public float verticalSpeed = 15f;
    public float turnSpeed = 90f;
    public float tiltAmount = 45f;
    
    [Header("Physics")]
    public float acceleration = 10f;
    public float deceleration = 5f;
    public float gravity = 9.8f;
    
    [Header("Input Smoothing")]
    public float inputSmoothing = 0.1f;
    
    private Rigidbody rb;
    private float currentSpeed;
    private Vector3 movement;
    private Vector2 smoothedInput;
    private bool isBoosting;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // We'll handle gravity
    }
    
    void Update()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float ascend = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        float descend = Input.GetKey(KeyCode.LeftControl) ? 1f : 0f;
        isBoosting = Input.GetKey(KeyCode.LeftShift);
        
        // Smooth input
        smoothedInput = Vector2.Lerp(smoothedInput, 
            new Vector2(horizontal, vertical), 
            inputSmoothing);
        
        // Calculate movement
        Vector3 forward = transform.forward * smoothedInput.y;
        Vector3 right = transform.right * smoothedInput.x * 0.5f;
        Vector3 up = Vector3.up * (ascend - descend) * verticalSpeed;
        
        movement = (forward + right).normalized;
        movement.y = up.y;
        
        // Rotation
        if (smoothedInput.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(
                new Vector3(movement.x, 0, movement.z));
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                turnSpeed * Time.deltaTime);
        }
        
        // Banking effect
        float bankAngle = -horizontal * tiltAmount;
        transform.localRotation = Quaternion.Euler(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            bankAngle);
    }
    
    void FixedUpdate()
    {
        // Apply movement
        float targetSpeed = isBoosting ? boostSpeed : normalSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 
            acceleration * Time.fixedDeltaTime);
        
        Vector3 velocity = movement * currentSpeed;
        velocity.y -= gravity * Time.fixedDeltaTime;
        
        rb.velocity = velocity;
        
        // Clamp altitude
        if (transform.position.y < 5f)
        {
            Vector3 pos = transform.position;
            pos.y = 5f;
            transform.position = pos;
            
            if (rb.velocity.y < 0)
            {
                Vector3 vel = rb.velocity;
                vel.y = 0;
                rb.velocity = vel;
            }
        }
    }
    
    // Utility methods
    public float GetCurrentSpeed() => currentSpeed;
    public bool IsBoosting() => isBoosting;
    public float GetSpeedPercentage() => currentSpeed / boostSpeed;
}
```

### 5. Configure Camera Follow
Using Cinemachine:
1. Select CM_DragonFollowCam
2. Set Follow target: Dragon_Controller/CameraTarget
3. Configure 3rd Person Follow:
   - Camera Distance: 15
   - Camera Side: 0.5
   - Vertical Offset: 5
   - Damping: 0.5 on all axes
4. Configure Composer aim:
   - Tracked Object Offset: (0, 2, 0)
   - Lookahead Time: 0.2
   - Smoothing: 5

### 6. Add Flight Effects
Create basic flight visualization:
```csharp
public class DragonFlightEffects : MonoBehaviour
{
    public TrailRenderer[] wingTrails;
    public ParticleSystem boostParticles;
    public AudioSource flightSound;
    
    private DragonFlightController flightController;
    
    void Start()
    {
        flightController = GetComponent<DragonFlightController>();
    }
    
    void Update()
    {
        // Wing trails intensity based on speed
        float speedPercent = flightController.GetSpeedPercentage();
        foreach (var trail in wingTrails)
        {
            trail.time = Mathf.Lerp(0.1f, 1f, speedPercent);
        }
        
        // Boost effects
        if (flightController.IsBoosting())
        {
            if (!boostParticles.isPlaying)
                boostParticles.Play();
        }
        else
        {
            if (boostParticles.isPlaying)
                boostParticles.Stop();
        }
        
        // Audio pitch based on speed
        if (flightSound != null)
        {
            flightSound.pitch = Mathf.Lerp(0.8f, 1.3f, speedPercent);
        }
    }
}
```

### 7. Input Configuration
Edit > Project Settings > Input Manager:

Add/Modify axes:
- Horizontal: A/D keys + Left/Right arrows
- Vertical: W/S keys + Up/Down arrows
- Fire1: Left Mouse Button
- Fire2: Right Mouse Button
- Jump: Space (for ascend)
- Sprint: Left Shift (for boost)

### 8. Test Flight Controls
1. Enter Play mode
2. Test all movement axes
3. Verify smooth acceleration
4. Check camera follow
5. Test altitude limits
6. Verify boost works
7. Check banking visuals

### 9. Create Flight Test Scene
1. Duplicate Level01 scene
2. Rename to "FlightTest"
3. Add floating rings to fly through
4. Add speed indicator UI
5. Use for tuning controls

### 10. Fine-Tuning Checklist
- [ ] Acceleration feels natural
- [ ] Turning is responsive but not twitchy
- [ ] Camera maintains good view
- [ ] Boost provides clear speed increase
- [ ] Banking looks realistic
- [ ] Altitude limits work
- [ ] No jittery movement

## Expected Outcomes
- ✅ Dragon responds to all inputs smoothly
- ✅ Flight feels arcade-like but controlled
- ✅ Camera follows without nausea
- ✅ Boost system works correctly
- ✅ Visual effects enhance movement
- ✅ Can fly around entire island
- ✅ Performance remains 60+ FPS

## Control Scheme Summary
- **WASD**: Basic movement
- **Mouse**: Camera look (optional)
- **Space**: Ascend
- **Ctrl**: Descend
- **Shift**: Boost
- **Left Click**: Fire plasma orb (later)
- **Right Click**: Fire rapid shots (later)

## Common Issues & Solutions

### Issue: Jittery Movement
- Increase rigidbody mass
- Adjust input smoothing
- Check Time.fixedDeltaTime
- Use interpolation on rigidbody

### Issue: Camera Clips Through Terrain
- Adjust camera collision settings
- Increase minimum distance
- Add camera collision detection
- Use Cinemachine collider

### Issue: Unresponsive Controls
- Check input manager settings
- Verify script execution order
- Increase acceleration values
- Reduce drag values

### Issue: Dragon Flies Through Ground
- Add collision detection
- Implement altitude clamping
- Check collider setup
- Add ground detection ray

## Time Estimate: 3-4 hours

## Next Steps
Proceed to Task 07: Dragon Model Integration

---

## Completion Notes
*To be filled after task completion*

**Date Completed**: 
**Time Taken**: 
**Final Speed Values**: 
**Input Settings**: 
**Performance Impact**: 
**Player Feedback**: 