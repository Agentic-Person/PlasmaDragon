# Task 05: Environment Setup

## Status: Pending

## Priority: MEDIUM - Sets the visual foundation

## Description
Import and configure the game environment including castle/island assets, terrain, lighting, and atmosphere. This creates the visual setting where all gameplay takes place.

## Prerequisites
- Task 04 (WebGL Configuration) completed
- Unity Asset Store account
- Level01_FortressIsland scene created

## Step-by-Step Instructions

### 1. Find Free Environment Assets
Search Unity Asset Store for these recommended free assets:
- "Free Fantasy Castle" by Rufat's ShaderLab
- "Low Poly Island" environment packs
- "Medieval Village Pack" (free versions)
- "Terrain Sample Asset Pack" by Unity

Alternative search terms:
- "free castle environment"
- "low poly island"
- "medieval fortress free"
- "fantasy environment lite"

### 2. Import Environment Asset
1. Add chosen asset to My Assets
2. Open Package Manager > My Assets
3. Download and Import
4. Import to: `Assets/_ImportedAssets/EnvironmentPack/`
5. Only import what's needed:
   - Models ✓
   - Textures ✓
   - Materials ✓
   - Demo Scenes ✗ (skip)

### 3. Set Up Island Terrain
In Level01_FortressIsland scene:

1. Create Terrain:
   - GameObject > 3D Object > Terrain
   - Position: (0, 0, 0)
   - Terrain Width: 500
   - Terrain Length: 500
   - Terrain Height: 50

2. Shape Terrain:
   - Raise/Lower tool for island shape
   - Create central plateau for castle
   - Add cliffs around edges
   - Leave landing areas

3. Paint Terrain:
   - Add 3-4 texture layers:
     - Grass (main)
     - Rock (cliffs)
     - Sand (beaches)
     - Dirt (paths)

### 4. Place Castle/Fortress
1. Find castle prefab in imported assets
2. Place on central plateau
3. Position: approximately (0, 20, 0)
4. Scale if needed (keep reasonable)
5. Add to `--- ENVIRONMENT ---` parent

Castle should include:
- Main keep/tower
- Walls
- Gates
- Defensive positions

### 5. Configure Lighting
Under `--- LIGHTING ---` parent:

**Directional Light (Sun):**
- Rotation: (45, -30, 0)
- Color: Warm sunset (#FFD4A3)
- Intensity: 1.2
- Shadows: Hard Shadows
- Shadow Strength: 0.8

**Ambient Lighting:**
- Window > Rendering > Lighting
- Environment Lighting:
  - Source: Gradient
  - Sky Color: #87CEEB
  - Equator Color: #FFE4B5
  - Ground Color: #8B7355

**Fog Settings:**
- Enable Fog: ✓
- Fog Mode: Linear
- Fog Color: #B0C4DE
- Start: 100
- End: 400

### 6. Add Environment Props
Place additional elements:
- Trees around island edges
- Rocks and boulders
- Small buildings/huts
- Docks or piers
- Watchtowers

Keep performance in mind:
- Use LODs where available
- Limit total object count
- Group static objects

### 7. Create Ocean/Water
Options for water:
1. Simple plane with water shader
2. Unity's water prefabs
3. Custom shader solution

Basic water setup:
```csharp
// Simple water movement script
public class SimpleWater : MonoBehaviour
{
    public float waveHeight = 0.5f;
    public float waveSpeed = 1f;
    private Vector3 startPos;
    
    void Start()
    {
        startPos = transform.position;
    }
    
    void Update()
    {
        transform.position = startPos + 
            Vector3.up * Mathf.Sin(Time.time * waveSpeed) * waveHeight;
    }
}
```

### 8. Optimize Environment
1. Make all static objects "Static"
2. Generate lighting:
   - Window > Rendering > Lighting
   - Generate Lighting (may take time)
3. Occlusion Culling:
   - Window > Rendering > Occlusion Culling
   - Bake (for performance)

### 9. Create Environment Prefab
1. Select all environment objects
2. Create prefab: `PRE_Level01_Environment`
3. Save in: `_Project/Prefabs/Environment/`
4. This allows easy duplication for Level 2

### 10. Performance Check
Create test script to verify FPS:
```csharp
public class PerformanceMonitor : MonoBehaviour
{
    private float deltaTime = 0.0f;
    
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
    
    void OnGUI()
    {
        float fps = 1.0f / deltaTime;
        string text = $"FPS: {fps:0.}";
        GUI.Label(new Rect(10, 10, 100, 20), text);
    }
}
```

## Expected Outcomes
- ✅ Island terrain created and textured
- ✅ Castle/fortress placed centrally
- ✅ Lighting creates sunset atmosphere
- ✅ Fog adds depth and mystery
- ✅ Water surrounds island
- ✅ 60+ FPS maintained
- ✅ Total environment < 50k triangles

## Visual Checklist
- [ ] Island feels like medieval fortress
- [ ] Castle is focal point
- [ ] Lighting is atmospheric
- [ ] Fog creates depth
- [ ] Scale feels appropriate
- [ ] Clear flight paths exist
- [ ] Defensive positions visible

## Common Issues & Solutions

### Issue: Performance Drop
- Reduce terrain resolution
- Use simpler shaders
- Decrease shadow distance
- Remove unnecessary details

### Issue: Lighting Too Dark
- Increase ambient intensity
- Add fill lights if needed
- Adjust fog density
- Check shadow settings

### Issue: Asset Import Errors
- Check Unity version compatibility
- Import dependencies first
- Use standard shader materials
- Convert textures if needed

### Issue: Scale Problems
- Measure against future dragon size
- Use Unity cube (1m) as reference
- Adjust terrain size if needed
- Keep playable area reasonable

## Artistic Guidelines
- Medieval fantasy aesthetic
- Warm, inviting colors
- Clear visual hierarchy
- Dragon-scale appropriate
- Good contrast for gameplay
- Readable from above

## Time Estimate: 2-3 hours

## Next Steps
Proceed to Task 06: Flight Controller Setup

---

## Completion Notes
*To be filled after task completion*

**Date Completed**: 
**Time Taken**: 
**Assets Used**: 
**Triangle Count**: 
**FPS Achieved**: 
**Visual Notes**: 