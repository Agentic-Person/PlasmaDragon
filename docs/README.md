# 🐉 PlasmaDragon - Plasma Thief: Sky Rogue

A Unity 3D aerial combat portfolio demo featuring AI-powered adaptive bosses, Web3 token rewards, and seamless blockchain integration. Built to showcase modern game development skills combining traditional gaming with cutting-edge technology.

![Unity](https://img.shields.io/badge/Unity-2022.3%20LTS-black?logo=unity)
![Platform](https://img.shields.io/badge/Platform-WebGL-blue)
![Blockchain](https://img.shields.io/badge/Blockchain-Solana-green)
![AI](https://img.shields.io/badge/AI-Claude%20%2F%20GPT--4-purple)

## 🎮 Game Overview

**Plasma Thief: Sky Rogue** puts players in control of a dragon-riding rogue engaging in aerial combat against AI-driven military forces. The game demonstrates:

- **Sophisticated Flight Mechanics**: Full 360° dragon flight with intuitive arcade controls
- **Adaptive AI Enemies**: Bosses powered by Claude/GPT-4 that learn and adapt
- **Dual Weapon System**: Strategic combat with area-effect plasma orbs and rapid-fire projectiles
- **Web3 Integration**: Earn real Solana tokens without knowing anything about blockchain
- **Progressive Difficulty**: Enemies that remember and counter your tactics

## 🚀 Quick Start

### Prerequisites

- Unity 2022.3 LTS
- Cursor IDE with Claude Code integration
- Git
- Web browser with WebGL support

### Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/PlasmaDragon.git
cd PlasmaDragon
```

2. Install Unity MCP for Cursor integration:
```bash
git clone https://github.com/CoderGamester/mcp-unity.git
# Follow setup instructions in the MCP repository
```

3. **Unity Project Setup**:
   - Open Unity Hub
   - Click "Add" and select this project folder
   - Unity Version: 2022.3.x LTS (must be exact)
   - First time opening will import all packages

4. **Initial Configuration** (First time only):
   - File → Build Settings → Switch Platform to WebGL
   - Edit → Project Settings → Player:
     - Company Name: Your Company
     - Product Name: PlasmaDragon
     - WebGL → Publishing Settings:
       - Compression Format: Gzip
       - Decompression Fallback: ✓
   - Edit → Project Settings → Quality:
     - Delete all except Low/Medium/High
     - Set WebGL default to "Medium"

5. **Package Installation** (should auto-install from manifest.json):
   - Window → Package Manager
   - Verify installed:
     - Cinemachine 2.9.7
     - Universal Render Pipeline 14.0.8
     - Addressables 1.21.19
     - TextMeshPro 3.0.6

6. **MCP Unity Configuration**:
   - Follow instructions in `tasks/01-mcp-unity-setup.md`
   - Test with: "Create a terrain with grass"

## 🎯 Gameplay Features

### Core Mechanics

- **Dragon Flight**: Responsive controls with boost, brake, and hover modes
- **Combat System**: 
  - Plasma Orbs: 100 damage AOE attacks that devastate groups
  - Rapid Fireballs: 25 damage machine-gun style barrage
- **Enemy Types**:
  - Archer Towers: Fixed defenses with predictable patterns
  - Ballista Turrets: Heavy damage with predictive targeting
  - Military Patrols: Ground forces vulnerable to area attacks
  - AI Boss: Adaptive commander with multiple attack strategies

### Level Progression

1. **Level 1 - Fortress Island**: Learn the basics, defeat the Elite Commander
2. **Level 2 - Adapted Defenses**: Same layout, but enemies have learned your tactics
3. **Level 3 - Mountain Citadel** (Optional): Aerial boss battle with new challenges

### Token Rewards

- Level 1 completion: 5 PLASMA tokens
- Level 2 completion: 8 PLASMA tokens  
- Perfect run bonus: +3 PLASMA tokens
- No gas fees or wallet setup required!

## 🛠️ Technical Architecture

### Tech Stack

- **Game Engine**: Unity 2022.3 LTS (Built-in Render Pipeline)
- **Programming**: C# with async/await patterns
- **AI Integration**: Claude Sonnet 4 / GPT-4 API
- **Blockchain**: Solana (Devnet for demo)
- **Backend**: Supabase (free tier)
- **Automation**: n8n for token distribution
- **Hosting**: GitHub Pages / Netlify

### Project Structure

```
Assets/
├── _Project/
│   ├── Scripts/
│   │   ├── Player/          # Dragon flight and controls
│   │   ├── AI/              # Boss AI and enemy behaviors
│   │   ├── Combat/          # Weapon systems and damage
│   │   ├── Web3/            # Blockchain integration
│   │   ├── Core/            # Core game systems
│   │   ├── UI/              # User interface
│   │   └── Utilities/       # Helper scripts
│   ├── Prefabs/             # Game objects
│   ├── Materials/           # Shaders and materials
│   ├── Textures/            # Image assets
│   ├── Audio/               # Sound effects and music
│   ├── ScriptableObjects/   # Data containers
│   └── Resources/           # Runtime loaded assets
├── _ImportedAssets/         # Third-party assets
├── Plugins/                 # External libraries
│   ├── Solana/              # Solana Unity SDK
│   └── Supabase/            # Supabase Unity SDK
└── WebGLTemplates/          # Custom WebGL templates
    └── PlasmaDragon/        # Game-specific template
```

### Performance Targets

- **Frame Rate**: 60 FPS (desktop), 30 FPS (mobile)
- **Build Size**: <50MB compressed
- **Load Time**: <20 seconds
- **Memory Usage**: <256MB

## 🎨 Asset Credits

### Core Assets
- **Flight Controller**: 3rd Person Controller + Fly Mode (Asset ID: 28647) - FREE
- **Dragon Model**: [Dragon for Boss Monster: HP] or purchased asset
- **Environment**: Free Fantasy Castle assets
- **Effects**: Unity Particle Pack

### Optional Enhancements
- **3D AI Studio** ($9/month) - For custom props
- **Meshy AI** ($20/month) - Higher quality models

## 🤖 AI Integration

The game features two types of AI:

### Boss AI (API-Powered)
- Makes strategic decisions every 10-15 seconds
- Adapts tactics based on player behavior
- Remembers strategies between levels
- Cost: ~$0.01-0.02 per playthrough

### Military AI (Local)
- Heat map tracking of player routes
- Patrol adjustments between levels
- No API calls required
- Visible behavioral changes

## 💰 Web3 Features

### Seamless Onboarding
1. Email/password registration (no wallet needed)
2. Auto-generated custodial wallet
3. Instant token rewards on victory
4. Optional wallet export for advanced users

### Token Distribution
- Automated via n8n workflows
- No gas fees (subsidized)
- Real Solana blockchain tokens
- Viewable in-game and on-chain

## 🚧 Development Roadmap

### Phase 1: MVP (Current)
- [x] Core flight mechanics
- [x] Dual weapon system
- [x] Basic enemy AI
- [x] AI-powered boss
- [x] Web3 integration
- [x] 2 playable levels

### Phase 2: Enhanced Demo
- [ ] Level 3 with aerial boss
- [ ] NFT dragon skins
- [ ] Global leaderboard
- [ ] Achievement system

### Phase 3: Full Game
- [ ] 20+ levels
- [ ] Multiplayer battles
- [ ] Dragon customization
- [ ] User-generated content

## 📊 Development Metrics

### Current Status
- **Development Time**: 3-4 weeks
- **Lines of Code**: ~5,000
- **Asset Count**: 50-75
- **Total Cost**: $25-95

### Performance Metrics
- **Desktop FPS**: 60+ consistent
- **Mobile FPS**: 30+ stable
- **Load Time**: 15-18 seconds
- **Build Size**: 45MB compressed

## 🐛 Known Issues

- Mobile touch controls need optimization
- Occasional AI response delays (caching helps)
- WebGL audio compression artifacts
- Safari requires user interaction for audio

## 🤝 Contributing

This is a portfolio project, but feedback is welcome! Please open an issue for:
- Bug reports
- Performance problems
- Gameplay suggestions
- Technical questions

## 📜 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Unity Technologies for the amazing engine
- Anthropic/OpenAI for AI capabilities
- Solana Labs for blockchain infrastructure
- The Unity Asset Store community

## 📞 Contact

- **Developer**: [Your Name]
- **Email**: your.email@example.com
- **Portfolio**: https://yourportfolio.com
- **LinkedIn**: https://linkedin.com/in/yourprofile

---

**Play the Demo**: [https://plasmadragon.demo.com](#)

*Built with ❤️ for the Web3 gaming community*