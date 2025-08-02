# PlasmaDragon Project Structure Reference

## 🎯 Purpose
**Template for organizing Unity projects with professional modular architecture**  
Use this as a reference to replicate the same structure in your environment project.

## 📁 **Complete Directory Structure**

### **🔧 Top-Level Assets Organization**
```
Assets/
├── _Project/                           ← 🎯 OUR MAIN CODE (underscore keeps it at top)
├── AddressableAssetsData/              ← Unity Addressables system
├── ImportedAssets/                     ← Third-party assets (organized)
├── Plugins/                            ← External libraries
├── StreamingAssets/                    ← Runtime content
├── DefaultVolumeProfile.asset          ← URP post-processing
└── UniversalRenderPipelineGlobalSettings.asset  ← URP configuration
```

### **🎮 _Project Folder Structure (CORE ORGANIZATION)**
```
Assets/_Project/
├── Scenes/                             ← 🎬 MODULAR SCENE ARCHITECTURE
│   ├── Masters/                        ← Integration & orchestration scenes
│   │   └── MasterGame_Orchestrator.unity
│   ├── Dragons/                        ← Dragon development workspace
│   │   └── DragonScene_WorkSpace.unity
│   ├── Environments/                   ← Environment building scenes
│   │   └── IslandBuildScene_001.unity
│   └── testFlight_001.unity            ← Testing & prototyping scene
├── Scripts/                            ← 💻 ORGANIZED CODE ARCHITECTURE
│   ├── Combat/                         ← 🧠 AI COMBAT SYSTEM (Task 18)
│   │   ├── AIBossController.cs         ← Claude-powered strategic boss
│   │   ├── SmartTowerSystem.cs         ← Adaptive AI towers
│   │   ├── EnemyAI.cs                  ← Multi-type enemy behaviors
│   │   ├── DynamicDifficultyManager.cs ← Performance-based scaling
│   │   └── TowerDefenseSystem.cs       ← Basic auto-targeting towers
│   ├── Core/                           ← Game systems & managers
│   ├── Player/                         ← Dragon-specific scripts
│   ├── AI/                             ← Future AI expansions
│   ├── UI/                             ← User interface scripts
│   ├── Web3/                           ← Blockchain integration
│   ├── Editor/                         ← Development tools
│   ├── BasicFlightController.cs        ← 🎮 Enhanced flight system
│   ├── CameraFollow.cs                 ← 📷 Smooth camera system
│   ├── SystemDiagnostic.cs             ← 🔧 Debugging utilities
│   ├── WorkflowGuide.cs                ← Development guidelines
│   └── PlasmaDragon.Runtime.asmdef     ← Assembly definition
├── Models/                             ← 🐲 3D ASSETS
│   └── Dragons/                        ← Dragon models & animations
├── Materials/                          ← 🎨 URP MATERIALS
├── Prefabs/                            ← 📦 REUSABLE GAME OBJECTS
│   ├── Combat/                         ← AI combat prefabs
│   ├── Dragons/                        ← Dragon prefabs
│   ├── Enemies/                        ← Enemy prefabs
│   ├── Environment/                    ← Environment prefabs
│   ├── Environments/                   ← Environment systems
│   ├── Gameplay/                       ← Gameplay mechanics
│   ├── Player/                         ← Player-related prefabs
│   ├── UI/                             ← Interface prefabs
│   └── Weapons/                        ← Weapon system prefabs
├── Audio/                              ← 🔊 SOUND ORGANIZATION
├── Textures/                           ← 🖼️ TEXTURE LIBRARY
├── ScriptableObjects/                  ← 📋 DATA CONTAINERS
├── URP-PipelineAsset.asset             ← URP pipeline configuration
└── UniversalRenderer.asset             ← URP renderer settings
```

## 🔧 **Key Organization Principles**

### **📂 Folder Naming Convention**
- **_Project/**: Underscore prefix keeps our code at the top of Assets
- **PascalCase**: For all folder names (Scripts, Models, Materials)
- **Descriptive Names**: Clear purpose for each folder
- **Hierarchical Structure**: Logical grouping by function/type

### **🎬 Scene Architecture (Modular)**
- **Masters/**: Main integration scenes (orchestrate everything)
- **Dragons/**: Isolated dragon development and testing
- **Environments/**: Environment design and building
- **Root Level**: General testing scenes (testFlight_001.unity)

### **💻 Script Organization (Component-Based)**
- **Combat/**: All AI combat systems (Task 18 implementation)
- **Core/**: Essential game systems and managers
- **Player/**: Dragon-specific functionality
- **AI/**: Future AI system expansions
- **UI/**: User interface and HUD systems
- **Web3/**: Blockchain and token integration
- **Editor/**: Development tools and utilities

### **📦 Prefab Organization (Asset Types)**
- **Combat/**: AI combat encounter prefabs
- **Dragons/**: Dragon variants and configurations
- **Enemies/**: Enemy types and formations
- **Environment/**: Static environment pieces
- **Environments/**: Complete environment systems
- **Gameplay/**: Interactive gameplay mechanics
- **Player/**: Player-related systems
- **UI/**: Interface components
- **Weapons/**: Weapon systems and projectiles

## 🎯 **Replication Instructions for Your Environment Project**

### **Step 1: Create Main Structure**
```bash
# In your environment project Assets folder:
mkdir _Project
cd _Project
mkdir Scenes Scripts Models Materials Prefabs Audio Textures ScriptableObjects
```

### **Step 2: Create Scene Organization**
```bash
cd Scenes
mkdir Masters Dragons Environments
# Move your environment scene to Environments/
```

### **Step 3: Create Script Organization**
```bash
cd ../Scripts
mkdir Combat Core Player AI UI Web3 Editor
```

### **Step 4: Create Prefab Organization**
```bash
cd ../Prefabs
mkdir Combat Dragons Enemies Environment Environments Gameplay Player UI Weapons
```

### **Step 5: Transfer Assets to Proper Locations**
- **Environment Scene**: Move to `_Project/Scenes/Environments/`
- **3D Models**: Organize in `_Project/Models/` by type
- **Materials**: Move to `_Project/Materials/`
- **Combat Assets**: Prepare for `_Project/Prefabs/Combat/`

## 🧠 **AI Combat System Integration Points**

### **Combat Assets Organization**
When transferring your combat assets, organize them like this:
```
_Project/Models/
├── Buildings/
│   ├── Towers/          ← For TowerDefenseSystem + SmartTowerSystem
│   ├── Fortifications/
│   └── Structures/
├── Units/
│   ├── Soldiers/        ← For EnemyAI (Soldier type)
│   ├── Archers/         ← For EnemyAI (Archer type)
│   └── Guards/          ← For EnemyAI (Guard type)
└── Weapons/
    ├── TowerWeapons/
    └── UnitWeapons/
```

### **Scene Hierarchy for AI Integration**
```
YourEnvironmentScene.unity:
├── 🏝️ ENVIRONMENT
│   └── (your static environment)
├── ⚔️ COMBAT_TARGETS
│   ├── 🏰 Tower Defense System
│   ├── 🤖 Smart Tower Network
│   ├── 👥 Enemy Soldiers
│   ├── 🏹 Archer Units
│   └── 🧠 AI_BOSS_ARENA
├── 📊 GAME_SYSTEMS
│   └── DifficultyManager
└── 🎮 TESTING
    └── Dragon_Test_Spawn
```

## 🔧 **Critical URP Setup Files**
Make sure to include these URP configuration files:
- `URP-PipelineAsset.asset` (Pipeline configuration)
- `UniversalRenderer.asset` (Renderer settings)
- `DefaultVolumeProfile.asset` (Post-processing)
- `UniversalRenderPipelineGlobalSettings.asset` (Global URP settings)

## 📋 **Assembly Definition Setup**
Include `PlasmaDragon.Runtime.asmdef` in Scripts folder:
```json
{
    "name": "PlasmaDragon.Runtime",
    "rootNamespace": "PlasmaDragon",
    "references": [
        "Unity.InputSystem",
        "Unity.TextMeshPro",
        "Unity.Addressables"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
```

## ✅ **Verification Checklist**

### **Structure Complete When:**
- [ ] `_Project/` folder created at Assets root
- [ ] Modular scene folders created (Masters/Dragons/Environments)
- [ ] Script folders organized by function (Combat/Core/Player/etc.)
- [ ] Prefab folders organized by asset type
- [ ] URP configuration files present
- [ ] Assembly definition configured
- [ ] Assets moved to appropriate folders

### **Ready for AI Integration When:**
- [ ] Combat assets organized in proper folders
- [ ] Scene hierarchy matches AI system requirements
- [ ] 3D models ready for script attachment
- [ ] Environment scene in _Project/Scenes/Environments/
- [ ] Combat prefab folders prepared

## 🎯 **Benefits of This Structure**

✅ **Professional Organization**: Industry-standard folder hierarchy  
✅ **Modular Development**: Isolated systems for team development  
✅ **AI System Ready**: Optimized for Task 18 AI combat integration  
✅ **Scalable Architecture**: Easy to add new systems and features  
✅ **Clear Dependencies**: Well-defined component relationships  
✅ **Performance Optimized**: Assembly definitions for faster compilation  

---

**📝 Usage**: Reference this document when organizing your environment project to match the PlasmaDragon structure. This ensures seamless integration when bringing assets between projects and maintains professional development standards. 