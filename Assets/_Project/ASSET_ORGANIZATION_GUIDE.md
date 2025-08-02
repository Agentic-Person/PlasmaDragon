# PlasmaDragon Asset Organization Guide

## 📋 **Directory Structure Overview**

This guide explains how to properly organize assets when importing into PlasmaDragon.

## 🗂️ **Core Directory Structure**

```
Assets/_Project/
├── Animation/              # Animation controllers, clips, timelines
├── Audio/
│   ├── Music/             # Background music tracks
│   ├── SFX/               # Sound effects
│   └── Ambience/          # Ambient audio loops
├── Effects/               # Particle systems, VFX prefabs
├── Environment/
│   ├── Terrain/           # Terrain assets, heightmaps
│   ├── Props/             # Environment props, decorations
│   ├── Buildings/         # Structures, architecture
│   └── Nature/            # Trees, rocks, vegetation
├── Gameplay/
│   ├── Systems/           # Gameplay system prefabs
│   └── Mechanics/         # Game mechanic components
├── Materials/
│   ├── Dragons/           # Dragon-specific materials
│   ├── Environment/       # Environment materials
│   ├── UI/                # UI materials
│   └── Effects/           # Effect materials
├── Models/
│   ├── Dragons/           # Dragon models, animations, textures
│   ├── Environment/       # Environment 3D models
│   ├── Props/             # Prop models
│   └── Characters/        # Character models (NPCs, enemies)
├── Prefabs/
│   ├── Dragons/           # Dragon prefab variants
│   ├── Environment/       # Environment prefabs
│   ├── Gameplay/          # Gameplay system prefabs
│   ├── UI/                # UI prefabs
│   └── Combat/            # Combat-related prefabs
├── Scenes/
│   ├── Development/       # Test scenes, prototypes
│   └── Production/        # Final game scenes
├── ScriptableObjects/
│   ├── GameData/          # Game configuration data
│   └── Settings/          # Game settings
├── Scripts/
│   ├── Core/              # Core game systems
│   ├── Player/            # Player-related scripts
│   ├── AI/                # AI behavior scripts
│   ├── Combat/            # Combat system scripts
│   ├── UI/                # UI scripts
│   ├── Environment/       # Environment scripts
│   ├── Gameplay/          # Gameplay mechanic scripts
│   ├── Utilities/         # Helper utilities
│   └── Editor/            # Editor tools and utilities
├── Textures/
│   ├── Dragons/           # Dragon textures
│   ├── Environment/       # Environment textures
│   ├── UI/                # UI textures
│   └── Effects/           # Effect textures
└── UI/
    ├── Menus/             # Menu UI prefabs
    ├── HUD/               # HUD elements
    └── Components/        # Reusable UI components
```

## 🎯 **Asset Import Guidelines**

### When Importing New Assets:

1. **Identify Asset Type**: Determine what category the asset belongs to
2. **Place in Correct Directory**: Use the structure above
3. **Follow Naming Conventions**: Use clear, descriptive names
4. **Create Prefabs**: Convert scene objects to reusable prefabs
5. **Update Materials**: Ensure URP compatibility

### 🐉 **Dragon Asset Organization**
- **Models**: `Models/Dragons/` 
- **Textures**: `Textures/Dragons/`
- **Materials**: `Materials/Dragons/`
- **Prefabs**: `Prefabs/Dragons/`
- **Animations**: `Animation/` (dragon-specific)

### 🏰 **Environment Asset Organization**
- **Large Packs**: Create subdirectory (e.g., `Environment/TreasureIsland/`)
- **Individual Props**: Sort by type (`Props/Furniture/`, `Props/Decorations/`)
- **Buildings**: `Environment/Buildings/[PackName]/`
- **Nature**: `Environment/Nature/[Type]/`

### 🎮 **Gameplay Asset Organization**
- **Combat**: `Prefabs/Combat/` + `Scripts/Combat/`
- **UI**: `UI/[MenuType]/` + `Scripts/UI/`
- **Systems**: `Gameplay/Systems/` + `Scripts/Gameplay/`

## ✅ **Current Organized Assets**

### ✅ **Dragon System (WORKING)**
```
Models/Dragons/
├── Unka Toon.FBX           # Main dragon model
├── Textures/               # Dragon textures
├── Materials/              # Dragon materials (Toon variants)
└── Animations/             # Dragon animations

Materials/Dragons/
├── Dragon_Green_URP.mat    # Working URP material
├── SimpleGreenDragon.mat   # Alternative material
└── [Other dragon materials]

Scripts/Player/
└── BasicFlightController.cs # Working flight system

Scripts/AI/
├── AIBossController.cs     # 33KB - Claude-powered boss
├── SmartTowerSystem.cs     # 25KB - Adaptive towers
├── EnemyAI.cs             # 22KB - Multi-type enemies
├── DynamicDifficultyManager.cs # 19KB - Performance scaling
└── TowerDefenseSystem.cs   # 9KB - Basic auto-targeting
```

## 🚀 **Next Steps for Asset Import**

### 1. **Environment Assets**
When re-importing Treasure Island or other environment packs:
```
Environment/TreasureIsland/
├── Models/
├── Textures/
├── Materials/
└── Prefabs/
```

### 2. **Combat Assets**
```
Prefabs/Combat/
├── Enemies/
├── Weapons/
├── Towers/
└── Effects/
```

### 3. **UI Assets**
```
UI/Menus/
├── MainMenu/
├── GameHUD/
└── Settings/
```

## 🛠️ **Tools Available**

- **MaterialURPFixer.cs**: Converts materials to URP (Tools menu)
- **Clean project structure**: Ready for systematic asset import
- **Modular prefab system**: Environment can be imported as organized prefabs

## ⚡ **Best Practices**

1. **Import One Pack at a Time**: Don't overwhelm the structure
2. **Test Each Import**: Ensure assets work before adding more
3. **Create Prefabs Immediately**: Don't leave loose scene objects
4. **Maintain URP Compatibility**: Use the MaterialURPFixer for imports
5. **Document Asset Sources**: Keep track of where assets came from

---

**Ready for systematic asset import! 🎮** 