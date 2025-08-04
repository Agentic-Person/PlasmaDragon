# PlasmaDragon Project Setup Guide

Complete instructions for setting up this Unity project on any computer.

## 🎯 **Quick Setup (Recommended)**

### Option 1: Clone from GitHub (Easiest)
```bash
# Clone the repository
git clone https://github.com/Agentic-Person/PlasmaDragon.git
cd PlasmaDragon

# Switch to the latest branch with all features
git checkout clean-vegetation-optimizer
```

### Option 2: Manual File Transfer
Copy these essential folders/files only (Library/ will regenerate):
```
PlasmaDragon/
├── Assets/                    # 21MB - All game content ✅ ESSENTIAL
├── ProjectSettings/           # 200KB - Unity configuration ✅ ESSENTIAL  
├── Packages/                  # 20KB - Package dependencies ✅ ESSENTIAL
├── projectScripts/            # Reusable scripts library ✅ ESSENTIAL
├── docs/                      # Documentation ✅ ESSENTIAL
├── tasks/                     # Development tasks ✅ ESSENTIAL
├── TODO.md                    # Project status ✅ ESSENTIAL
├── README.md                  # Project overview ✅ ESSENTIAL
└── .gitignore                 # Git configuration ✅ ESSENTIAL

❌ SKIP THESE (Unity will regenerate):
├── Library/                   # 1.6GB - Unity cache (regenerates)
├── Logs/                      # Unity logs (regenerates)  
├── UserSettings/              # User preferences (regenerates)
└── Temp/                      # Temporary files (regenerates)
```

**Transfer Size**: ~400MB instead of 2GB!

## 🛠️ **Requirements**

### Unity Installation
- **Unity Version**: 6000.0.53f1 (Unity 6 LTS)
- **Download**: [Unity Hub](https://unity.com/download) → Install Unity 6 LTS
- **Required Modules**: 
  - WebGL Build Support
  - Linux Build Support (if on Linux)

### System Requirements
- **RAM**: 8GB minimum, 16GB recommended
- **Storage**: 2GB for project + 5GB for Unity
- **OS**: Windows 10+, macOS 10.15+, or Ubuntu 18.04+

## 📦 **Package Dependencies (Auto-Install)**

Unity will automatically install these when you open the project:
```json
{
  "com.unity.render-pipelines.universal": "14.0.11",  // URP rendering
  "com.unity.cinemachine": "2.9.7",                    // Camera system
  "com.unity.textmeshpro": "3.0.6",                   // Text rendering
  "com.unity.addressables": "1.21.19"                 // Asset management
}
```

## 🚀 **First-Time Setup Steps**

### 1. Open Project
1. **Launch Unity Hub**
2. **Click "Add"** → Navigate to PlasmaDragon folder
3. **Select the folder** (not a .unity file)
4. **Click "Open"** - Unity will import packages (takes 5-10 minutes first time)

### 2. Verify Setup
1. **Check Console** - Should show no critical errors
2. **Open Scene**: `Assets/_Project/Scenes/Production/MasterScene.unity`
3. **Test DragonToolkit**: `Tools → Dragon Toolkit` should open

### 3. Verify DragonToolkit
1. **Open**: Tools → Dragon Toolkit 
2. **Check Vegetation Optimizer**: Should show counts for Grass, Plants, Trees, etc.
3. **Test Sliders**: Try removing 10% of grass (should work immediately)

## 🌿 **DragonToolkit Features Available**
- ✅ Vegetation Optimizer (6 interactive sliders)
- ✅ Emergency Unity fixes  
- ✅ Project diagnostics
- ✅ URP and lighting tools

## 🆘 **Troubleshooting**

### Common Issues
| Problem | Solution |
|---------|----------|
| **"Project won't open"** | Check Unity version (needs Unity 6 LTS) |
| **"Package errors"** | Wait for Unity to finish importing (10+ minutes) |
| **"DragonToolkit missing"** | Check `Assets/Scripts/Editor/DragonToolkit.cs` exists |
| **"Water not rendering"** | Verify Quality Settings set to "PC" not "Mobile" |
| **"Scene looks broken"** | Open `Assets/_Project/Scenes/Production/MasterScene.unity` |

### Emergency Reset
If Unity gets confused:
1. **Close Unity**
2. **Delete Library/ folder**
3. **Reopen project** (Unity rebuilds Library automatically)

---

## 🎉 **You're Ready!**

Once Unity finishes importing:
1. **Open**: `Tools → Dragon Toolkit`  
2. **Test**: Vegetation Optimizer sliders
3. **Check**: `TODO.md` for next development tasks
4. **Start**: Working on dragon model completion (Task 07)

**Total setup time**: 15-20 minutes (mostly Unity importing packages)  
**File transfer**: Only ~400MB (if you skip Library folder)  

Happy developing! 🐉🌿🛠️
