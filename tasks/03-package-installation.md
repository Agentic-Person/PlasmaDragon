# Task 03: Essential Package Installation

## Status: Pending

## Priority: HIGH - Required for core functionality

## Description
Install and configure essential Unity packages and external SDKs needed for the game. This includes camera systems, visual effects, UI, and blockchain integration tools.

**Time Estimate**: 1.5 hours (reduced from 1-2 hours with manifest.json prepared)

## Prerequisites
- Task 02 (Project Initialization) completed
- Internet connection for package downloads
- Unity project open

## Step-by-Step Instructions

### 1. Unity Package Manager Packages

**Note**: Most packages should auto-install from manifest.json created in Task 02. Verify installation:

Open Window > Package Manager and verify these are installed:

#### Cinemachine (Camera System)
1. Should be version 2.9.7 (from manifest.json)
2. If missing, install manually
3. Import samples for reference

#### Universal Render Pipeline (URP)
1. Should be version 14.0.8 (from manifest.json)
2. Provides better WebGL performance
3. Includes built-in post-processing

#### TextMeshPro
1. Should be version 3.0.6 (from manifest.json)
2. Import TMP Essential Resources when prompted
3. Skip TMP Examples (save space)

#### Unity Particle Pack (Free)
1. Open Unity Asset Store in browser
2. Search "Unity Particle Pack"
3. Add to My Assets (free)
4. Open Package Manager > My Assets
5. Download and Import
6. Import into `_ImportedAssets/UnityParticlePack/`

### 2. External Package Installation

#### Solana Unity SDK
**Option 1 - Asset Store (Recommended)**:
1. Search "Solana SDK" in Unity Asset Store
2. Choose verified/official SDK
3. Import to `Assets/Plugins/Solana/`

**Option 2 - Git Clone**:
```bash
cd Assets/Plugins/Solana
git clone https://github.com/magicblock-labs/Solana.Unity-SDK.git
```

#### UnityAsync (Performance Helper)
1. Download from: https://github.com/muckSponge/UnityAsync
2. Import .unitypackage file
3. Place in `Assets/Plugins/UnityAsync/`
4. Verify no compilation errors

### 3. Configure Cinemachine Virtual Camera
1. In Level01_FortressIsland scene
2. Create > Cinemachine > Virtual Camera
3. Name it "CM_DragonFollowCam"
4. Settings:
   - Body: 3rd Person Follow
   - Aim: Composer
   - Damping: X=0.5, Y=0.5, Z=0.5
5. Save scene

### 4. Set Up URP Post Processing
1. Create empty GameObject "GlobalVolume"
2. Add Component > Volume (URP)
3. Check "Is Global"
4. Create Volume Profile:
   - Right-click in `_Project/Settings/`
   - Create > Volume Profile
   - Name: "VP_MainProfile"
5. Add basic effects for portfolio polish:
   - Bloom (subtle glow on plasma)
   - Vignette (cinematic feel)
   - Color Grading (warm tones)

### 5. Verify Package Integration
Create test script `Assets/_Project/Scripts/Utilities/PackageTest.cs`:
```csharp
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using Solana.Unity.SDK;
using UnityAsync;

public class PackageTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== Package Verification ===");
        Debug.Log($"Cinemachine: {CinemachineCore.kVersionString}");
        Debug.Log("TextMeshPro: Loaded");
        Debug.Log("PostProcessing: Active");
        Debug.Log("Solana SDK: Integrated");
        Debug.Log("UnityAsync: Available");
        Debug.Log("=== All packages OK ===");
    }
}
```

### 6. Create Package Configuration
Create `Assets/_Project/Settings/PackageConfig.asset`:
- Store package versions
- Configuration references
- SDK credentials (later)

### 7. Update .gitignore
Add to .gitignore:
```
# Package Manager
/[Pp]ackages/
!/[Pp]ackages/manifest.json
!/[Pp]ackages/packages-lock.json

# Imported Assets
_ImportedAssets/*/
!_ImportedAssets/.gitkeep
```

## Expected Outcomes
- ✅ All packages installed without errors
- ✅ No compilation errors in console
- ✅ Cinemachine camera ready for use
- ✅ Post processing configured
- ✅ Solana SDK integrated
- ✅ Package test script runs successfully

## Verification Checklist
- [ ] Cinemachine virtual camera in scene
- [ ] Post process volume with profile
- [ ] TextMeshPro resources imported
- [ ] Unity Particle Pack available
- [ ] Solana SDK compiles
- [ ] UnityAsync working
- [ ] Test script outputs all OK
- [ ] No console errors

## Common Issues & Solutions

### Issue: Package Not Found
- Check Unity version compatibility
- Try specific version number
- Clear Package Manager cache

### Issue: Compilation Errors
- Check for missing dependencies
- Verify .NET compatibility level
- Update Visual Studio packages

### Issue: Git Package Fails
- Use HTTPS instead of SSH
- Check network proxy settings
- Download and import manually

### Issue: Post Processing Not Working
- Ensure URP/HDRP not enabled
- Check Graphics settings
- Verify camera has PP Layer

## Package Versions Reference
```json
{
  "dependencies": {
    "com.unity.cinemachine": "2.8.9",
    "com.unity.postprocessing": "3.2.2",
    "com.unity.textmeshpro": "3.0.6",
    "com.unity.particles": "1.0.0"
  }
}
```

## Time Estimate: 1.5 hours

## Portfolio Demo Focus
- Prioritize packages that showcase technical skills
- Skip optional content to reduce build size
- Focus on WebGL-compatible features

## Next Steps
Proceed to Task 04: WebGL Build Configuration

---

## Completion Notes
*To be filled after task completion*

**Date Completed**: 
**Time Taken**: 
**Package Versions Installed**: 
**Issues Encountered**: 
**Total Project Size**: 
**Import Warnings**: 