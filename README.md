# PlasmaDragon

A Unity 3D WebGL dragon combat game showcasing Unity MCP integration, AI-powered adaptive bosses, and Web3 integration via Solana blockchain.

## Project Structure

```
PlasmaDragon/
├── Assets/
│   ├── _Project/           # Our custom content
│   │   ├── Scripts/        # C# game scripts
│   │   ├── Prefabs/        # Reusable game objects
│   │   ├── Materials/      # Shaders and materials
│   │   ├── Models/         # 3D models
│   │   ├── Textures/       # Image assets
│   │   ├── Audio/          # Sound and music
│   │   ├── Scenes/         # Unity scenes
│   │   └── ScriptableObjects/
│   ├── ImportedAssets/     # Third-party assets
│   ├── Plugins/            # External libraries
│   └── StreamingAssets/    # Runtime loaded content
├── Packages/               # Unity package dependencies
├── ProjectSettings/        # Unity project configuration
└── UserSettings/          # User-specific settings
```

## Key Features

- **Unity MCP Integration**: AI-assisted development with Claude
- **Dragon Flight Combat**: Smooth 3D flight mechanics with dual weapon system
- **AI Boss System**: Claude API-powered adaptive boss behavior
- **Web3 Integration**: Seamless Solana wallet and token rewards
- **WebGL Optimized**: Targets 60 FPS with <50MB build size

## Development Setup

1. **Unity Version**: 2022.3 LTS
2. **Render Pipeline**: Universal Render Pipeline (URP)
3. **Platform**: WebGL
4. **API Compatibility**: .NET Standard 2.1

## Getting Started

1. Clone this repository
2. Open in Unity 2022.3 LTS
3. Switch platform to WebGL (File > Build Settings)
4. Install required packages (auto-installed via manifest.json)
5. Open scene: Assets/_Project/Scenes/MainMenu.unity

## Unity MCP Setup

The project includes Unity MCP Bridge for AI-assisted development. Make sure Claude Desktop is configured with the Unity MCP server pointing to this project directory.

## License

This is a portfolio project. All rights reserved.