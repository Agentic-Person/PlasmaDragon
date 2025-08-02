# Unity Troubleshooting Toolkit - Setup Guide

## 🚀 **Quick Setup for Any Project**

This toolkit provides instant fixes for the most common Unity issues that can cost hours of debugging time.

### **1. Copy the Toolkit**
```
📁 Your New Project/
├── Assets/
│   ├── Scripts/
│   │   └── Editor/
│   │       └── UnityTroubleshootingToolkit.cs  ← Copy this file
│   └── ...
```

### **2. Customize for Your Project (5 minutes)**

Open `UnityTroubleshootingToolkit.cs` and modify these lines:

```csharp
// Line ~15: Change project name
private const string PROJECT_NAME = "YourProjectName";

// Line ~16: Change main character/object name  
private const string MAIN_OBJECT_NAME = "YourMainCharacter";

// Lines ~18-25: Update asset paths for your project structure
private static readonly Dictionary<string, string> ASSET_PATHS = new Dictionary<string, string>
{
    ["MainTexture"] = "Assets/YourTexturePath/MainTexture.png",
    ["MaterialsFolder"] = "Assets/YourMaterials/",
    ["URPAssetPath"] = "Assets/YourURP-Asset.asset",
    ["RendererDataPath"] = "Assets/YourRenderer.asset"
};
```

### **3. Add Project-Specific Fixes**

In the `CustomProjectFixes()` function (line ~285), add your specific fixes:

```csharp
static void CustomProjectFixes()
{
    Debug.Log($"🎯 Running {PROJECT_NAME}-specific fixes...");
    
    // YOUR CUSTOM FIXES HERE:
    
    // Example: Position specific objects
    GameObject player = GameObject.Find("Player");
    if (player != null && player.transform.position.y < -100)
    {
        player.transform.position = Vector3.zero;
    }
    
    // Example: Fix specific materials
    FixPlayerMaterials();
    
    // Example: Configure specific components
    ConfigurePlayerController();
}
```

---

## 🛠️ **Available Tools**

After setup, you'll have these menu items:

### **Emergency Fixes:**
- **🚨 EMERGENCY - Fix Everything** - Runs all fixes at once
- **🔍 Diagnose All Issues** - Shows what's broken

### **Specific Fixes:**
- **Fix URP Setup** - Solves render pipeline issues
- **Fix Lighting and Camera** - Restores scene visibility  
- **Fix Scene Visibility** - Finds lost objects
- **Fix All Materials** - Converts materials to URP

### **Diagnostics:**
- **🔍 Diagnose Render Pipeline** - Check URP configuration
- **🔍 Diagnose Camera** - Check camera settings
- **🔍 Diagnose Lighting** - Check lighting setup
- **🔍 Diagnose Main Object** - Check your main character
- **🔍 Diagnose Materials** - Check material compatibility

---

## 🎯 **Common Customizations**

### **For Character Controllers:**
```csharp
private const string MAIN_OBJECT_NAME = "Player";
// or "Character", "Hero", "Dragon", etc.
```

### **For Vehicle Games:**
```csharp
private const string MAIN_OBJECT_NAME = "Car";
// or "Ship", "Plane", "Tank", etc.
```

### **For Architecture/Visualization:**
```csharp
private const string MAIN_OBJECT_NAME = "Building";
// or focus on camera positioning instead
```

### **Custom Asset Paths:**
```csharp
private static readonly Dictionary<string, string> ASSET_PATHS = new Dictionary<string, string>
{
    // For character games:
    ["MainTexture"] = "Assets/Characters/Textures/PlayerTexture.png",
    ["MaterialsFolder"] = "Assets/Characters/Materials/",
    
    // For environment games:
    ["MainTexture"] = "Assets/Environment/Textures/TerrainTexture.png", 
    ["MaterialsFolder"] = "Assets/Environment/Materials/",
    
    // Standard URP paths (usually don't change these):
    ["URPAssetPath"] = "Assets/Settings/URP-PipelineAsset.asset",
    ["RendererDataPath"] = "Assets/Settings/UniversalRenderer.asset"
};
```

---

## 🔧 **Project-Specific Fix Examples**

### **Example 1: Platformer Game**
```csharp
static void CustomProjectFixes()
{
    // Reset player if fallen off map
    GameObject player = GameObject.Find("Player");
    if (player != null && player.transform.position.y < -50f)
    {
        player.transform.position = new Vector3(0, 1, 0);
        Debug.Log("✅ Player reset to spawn point");
    }
    
    // Fix platform materials
    FixPlatformMaterials();
}

static void FixPlatformMaterials()
{
    GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
    Material platformMaterial = CreatePlatformMaterial();
    
    foreach (GameObject platform in platforms)
    {
        MeshRenderer renderer = platform.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = platformMaterial;
        }
    }
}
```

### **Example 2: Racing Game**
```csharp
static void CustomProjectFixes()
{
    // Reset car if flipped
    GameObject car = GameObject.Find("Car");
    if (car != null)
    {
        Vector3 rotation = car.transform.eulerAngles;
        if (Mathf.Abs(rotation.z) > 90f || Mathf.Abs(rotation.x) > 90f)
        {
            car.transform.rotation = Quaternion.identity;
            car.transform.position += Vector3.up * 2f; // Lift slightly
            Debug.Log("✅ Car unflipped");
        }
    }
    
    // Position track cameras
    PositionTrackCameras();
}
```

### **Example 3: Architectural Visualization**
```csharp
static void CustomProjectFixes()
{
    // Position cameras at key viewpoints
    Camera[] cameras = FindObjectsOfType<Camera>();
    Vector3[] viewpoints = {
        new Vector3(10, 5, -10),  // Exterior view
        new Vector3(0, 2, 0),     // Interior view
        new Vector3(20, 15, -20)  // Overview
    };
    
    for (int i = 0; i < cameras.Length && i < viewpoints.Length; i++)
    {
        cameras[i].transform.position = viewpoints[i];
        cameras[i].transform.LookAt(Vector3.zero);
    }
    
    // Fix architectural materials
    FixBuildingMaterials();
}
```

---

## 📝 **Setup Checklist**

- [ ] Copy `UnityTroubleshootingToolkit.cs` to `Assets/Scripts/Editor/`
- [ ] Change `PROJECT_NAME` to your project name
- [ ] Update `MAIN_OBJECT_NAME` to your main object
- [ ] Customize `ASSET_PATHS` for your folder structure
- [ ] Add project-specific fixes in `CustomProjectFixes()`
- [ ] Test the toolkit with **Tools → YourProject → 🚨 EMERGENCY - Fix Everything**

---

## 🎉 **Benefits**

✅ **Saves Hours** - Instant fixes for common Unity issues  
✅ **Project Agnostic** - Works with any Unity project type  
✅ **Customizable** - Easy to adapt for specific needs  
✅ **Comprehensive** - Covers URP, lighting, camera, materials, objects  
✅ **Diagnostic** - Shows exactly what's broken  
✅ **Professional** - Clean, organized, well-documented code  

---

## 🆘 **Emergency Usage**

**When something breaks in Unity:**
1. **Tools → YourProject → 🔍 Diagnose All Issues** (see what's wrong)
2. **Tools → YourProject → 🚨 EMERGENCY - Fix Everything** (fix it all)
3. **Check Console** for detailed fix reports
4. **Test your scene** - should be working again!

**Most common fixes:**
- All materials pink/magenta → **Fix URP Setup**  
- Scene view grey → **Fix Lighting and Camera**
- Object disappeared → **Fix Scene Visibility**
- Play mode black → **Fix URP Setup** + **Fix Lighting and Camera**

---

*Save this toolkit in your Unity templates folder for instant access in new projects!* 🚀 