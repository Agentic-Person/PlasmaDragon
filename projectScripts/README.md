# Project Scripts Library

Reusable Unity scripts that can be copied to any future project.

## 📁 Structure

```
projectScripts/
├── Editor/              # Unity Editor scripts
│   └── DragonToolkit.cs # Vegetation optimizer + debugging tools
└── Runtime/             # Future runtime scripts
```

## 🛠️ Available Scripts

### Editor Scripts

#### DragonToolkit.cs
**Purpose**: Comprehensive Unity Editor toolkit for project optimization and debugging

**Features**:
- 🌿 **Vegetation Optimizer** - Interactive sliders for reducing vegetation for prefab optimization
  - Grass, Plants, Trees, Palm Trees, Vines, Small Rocks
  - Percentage-based removal (0-50%)
  - Real-time object counts
- 🚨 **Emergency Fixes** - One-click fixes for common Unity issues
- 🔍 **Diagnostics** - Complete project health analysis
- 🔧 **Individual Fixes** - Targeted solutions for specific problems

**How to Use**:
1. Copy `DragonToolkit.cs` to `Assets/Scripts/Editor/` in your Unity project
2. Access via `Tools → Dragon Toolkit` in Unity Editor
3. Use Vegetation Optimizer sliders to reduce polygon count for prefabs

**Dependencies**: 
- UnityEngine
- UnityEditor  
- System.Collections.Generic
- System.Linq

**Tested With**: Unity 2022.3+ LTS

## 🚀 Usage Instructions

### Adding to New Project
```bash
# Copy Editor scripts
cp projectScripts/Editor/*.cs YourProject/Assets/Scripts/Editor/

# Copy Runtime scripts (when available)
cp projectScripts/Runtime/*.cs YourProject/Assets/Scripts/
```

### Requirements
- Unity 2022.3 LTS or newer
- Scripts must be placed in appropriate folders:
  - Editor scripts → `Assets/Scripts/Editor/`
  - Runtime scripts → `Assets/Scripts/`

## 📝 Notes

- All scripts are tested and working in PlasmaDragon project
- Update this README when adding new scripts
- Maintain Unity naming conventions
- Include dependency information for each script
