# Dragon Rogue Game Design Document

## Game Overview

### Title
**Plasma Thief: Sky Rogue**

### Genre
Aerial Combat / Arcade / Web3 Gaming

### Platform
Unity WebGL (Browser-based)

### Target Audience
- **Primary**: Casual arcade players (Web2 gamers)
  - Enjoy action games
  - No blockchain knowledge
  - Surprised to discover they're earning real tokens
- **Secondary**: Web3 gaming enthusiasts
- **Tertiary**: Portfolio reviewers
- Demo: 2-3 levels (5-10 minutes gameplay)

### Core Concept
Players control a rogue riding a plasma dragon, engaging in aerial combat against AI-powered military forces and adaptive bosses. The game demonstrates cutting-edge AI integration, blockchain rewards, and seamless Web2-to-Web3 onboarding.

## Game Pillars

### 1. Accessible Flight Combat
- Intuitive controls inspired by arcade flight games
- No complex simulation mechanics
- Fun over realism

### 2. Adaptive AI Enemies
- Bosses that learn and adapt using AI APIs
- Each playthrough feels unique
- Visible progression of enemy intelligence

### 3. Seamless Web3 Integration
- Email login → Playing → Earning in 60 seconds
- No crypto knowledge required
- Real blockchain rewards

## Core Gameplay Loop

### 1. Approach Phase (30-45 seconds)
- Player flies toward fortress island
- Scouts enemy positions
- Identifies boss location
- Dodges defensive fire

### 2. Combat Phase (2-3 minutes)
- Engage AI-powered boss
- Destroy defensive turrets
- Manage dragon health/shields
- Use plasma orb attacks strategically

### 3. Collection Phase (15-30 seconds)
- Boss defeated, defenses down
- Dragon swoops to collect treasure
- Quick escape before reinforcements
- Token rewards distributed

## Player Character

### The Plasma Thief
- Mysterious sky rogue
- Permanently mounted on dragon
- No ground combat abilities
- Relies entirely on dragon's power

### Dragon Mount
- Plasma-powered creature
- Full 360° flight capability
- Regenerating shield system
- Plasma orb breath weapon

## Controls

### Flight Controls (Keyboard + Mouse)
- **W/S**: Throttle up/down
- **A/D**: Roll left/right
- **Mouse**: Pitch and yaw
- **Space**: Fire plasma orb
- **Shift**: Boost
- **Ctrl**: Brake/hover

## Combat System

### Dragon Weapons

**Primary: Plasma Orb**
- **Damage**: 100 (instant kill on regular enemies)
- **Area of Effect**: 10-foot radius explosion
- **Fire Rate**: 1 shot per 2 seconds
- **Visual**: Large blue/purple energy sphere
- **Sound**: Deep energy charge and release
- **Special**: Destroys groups of enemies

**Secondary: Rapid Fireballs**
- **Damage**: 25 per hit
- **Area of Effect**: Single target
- **Fire Rate**: 5 shots per second
- **Visual**: Smaller orange/red projectiles
- **Sound**: Quick "pew pew" fire sounds
- **Special**: Machine-gun style barrage

### Combat Controls
- **Left Click**: Fire plasma orb (hold to charge for bigger explosion)
- **Right Click**: Rapid fireball stream (hold for continuous fire)
- **Q**: Switch between weapons
- **E**: Special attack (if unlocked)

## Enemy Types

### Regular Enemies (Level 1)

**Archer Towers**
- Fixed positions
- Predictable firing patterns
- **Health**: 1 hit (plasma orb instantly destroys)
- Low threat individually

**Ballista Turrets**
- Slower, powerful shots
- Lead target prediction
- **Health**: 1-2 hits (1 plasma, 4 fireballs)
- Medium threat

**Military Patrols**
- Ground units that shoot upward
- Groups of 5-10 soldiers
- **Health**: 1 plasma orb destroys entire group
- **Area damage**: 10-foot radius kills all troops
- Low threat

### Adaptive Enemies (Level 2+)

**Smart Archers**
- Remember player flight patterns
- Adjust aim based on previous runs
- **Health**: Still 1 hit but harder to target
- Coordinate with other units
- Medium threat

**Tactical Patrols**
- Use heat map data from Level 1
- Spread out to avoid area damage
- Take cover when dragon approaches
- **Health**: Same but smarter positioning
- Medium-high threat

### Boss Enemies

**Level 1: Elite Commander**
- Large ground unit with multiple attacks
- Uses AI to select attack patterns
- Health: 100 HP
- Abilities:
  - Rapid fire crossbow
  - Shield generator
  - Summon reinforcements
  - Smoke screen

**Level 2: Veteran Commander**
- Same model, smarter AI
- Remembers player tactics from Level 1
- Adapts strategy mid-fight
- New patterns based on player behavior

**Level 3: Dragon Rider (Optional)**
- Aerial boss battle
- Full 3D combat
- Complex AI decision making
- Mirror match dynamics

## Level Design

### Level 1: Fortress Island
- **Setting**: Medieval castle on tropical island
- **Size**: 500m x 500m play area
- **Objectives**: 
  - Defeat Elite Commander
  - Destroy 5 defensive positions
  - Collect treasure chest
- **Token Reward**: 5 Plasma tokens

### Level 2: Adapted Fortress
- **Setting**: Same island, smarter defenses
- **Changes**:
  - Enemies use Level 1 data
  - New patrol routes
  - Coordinated attacks
  - Boss has new strategies
- **Token Reward**: 8 Plasma tokens

### Level 3: Mountain Citadel (Optional)
- **Setting**: Cliff-side fortress
- **Unique Features**:
  - Vertical combat
  - Environmental hazards
  - Flying boss enemy
- **Token Reward**: 10 Plasma tokens

## Enhanced AI System Design

### Multi-Layer AI Architecture

**Layer 1: Strategic AI (Claude API)**
- **Purpose**: High-level boss strategy decisions
- **Frequency**: Every 10-15 seconds
- **Cost**: ~$0.01-0.02 per session with caching
- **Decisions**: Attack patterns, phase transitions, special abilities

**Layer 2: Tactical AI (Local Processing)**
- **Purpose**: Real-time combat reactions
- **Frequency**: Every frame
- **Cost**: No API calls
- **Behaviors**: Dodge timing, aim adjustment, movement patterns

**Layer 3: Learning AI (Pattern Recognition)**
- **Purpose**: Analyze player behavior across sessions
- **Storage**: Supabase database
- **Features**: Heat maps, route preferences, combat style analysis

### AI Implementation Example
```
Boss Context Sent to Claude:
{
  "player_altitude": 45.2,
  "player_speed": 28.5,
  "player_preferred_attack": "dive_bombing",
  "boss_health": 65,
  "phase": 2,
  "previous_strategies": ["defensive", "summon_adds"],
  "player_historical_patterns": {
    "favors_left_approach": 0.7,
    "uses_plasma_orb": 0.8,
    "average_engagement_distance": 35
  }
}

Expected Response:
{
  "strategy": "anti_air_focus",
  "specific_actions": [
    "deploy_sky_mines",
    "increase_vertical_attacks",
    "summon_flying_units"
  ],
  "taunt": "I see you prefer the skies... let's change that!"
}
```

### Dynamic Features

**AI-Generated Narration**
- Contextual battle commentary
- Reacts to player performance
- Creates unique story moments

**Procedural Taunts**
- Based on player actions
- References previous encounters
- Personality-driven responses

**Environmental Adaptation**
- AI modifies level hazards
- Places defenses based on player routes
- Creates dynamic challenges

## Progression System

### Token Economy
- **Entry Cost**: None (free demo)
- **Rewards**:
  - Level 1: 5 tokens
  - Level 2: 8 tokens
  - Level 3: 10 tokens
  - Perfect run bonus: +3 tokens

### Dragon Upgrades (Visual Only for Demo)
- Bronze scales (default)
- Silver scales (10 tokens)
- Gold scales (25 tokens)
- Plasma aura effect (50 tokens)

## Visual Style

### Art Direction
- Low-poly aesthetic
- Vibrant, saturated colors
- Fantasy medieval theme
- Optimized for WebGL

### Key Visual Elements
- Glowing plasma effects
- Dynamic sky system
- Particle-based combat
- Simple but effective UI

## Audio Design

### Music
- Epic orchestral combat theme
- Dynamic intensity based on combat
- Compressed for web delivery

### Sound Effects
- Plasma orb: Sci-fi energy sound
- Dragon wings: Powerful flapping
- Impact sounds: Satisfying feedback
- Boss voice: AI-generated taunts

## User Interface

### HUD Elements
- Health/Shield bars (top left)
- Token counter (top right)
- Crosshair (center)
- Boss health (bottom center)
- Mini-map (bottom right)

### Menus
- Main Menu: Play/Settings/Wallet
- Pause Menu: Resume/Restart/Quit
- Victory Screen: Tokens earned/Next level
- Wallet View: Token balance/Address

## Technical Specifications

### Performance Targets
- 60 FPS desktop browsers
- 30 FPS mobile browsers
- <50MB download size
- <20 second load time

### Optimization Strategies
- LOD system for environment
- Object pooling for projectiles
- Texture compression
- Minimal post-processing

## Web3 Integration

### Wallet System
- Auto-generated custodial wallet
- Hidden complexity
- Export option for advanced users
- Solana blockchain

### Token Distribution
- Instant rewards on victory
- Visual feedback
- Transaction history
- No gas fees (subsidized)

## Onboarding Flow

1. **Landing Page** (30 seconds)
   - See dragon gameplay video
   - Click "Play Now"
   - Enter email/password

2. **Tutorial** (2 minutes)
   - Learn flight controls
   - Practice shooting
   - Understand objectives
   - First token reward

3. **Level 1** (3-5 minutes)
   - Full gameplay experience
   - Adaptive boss fight
   - Earn real tokens

## Success Metrics

### Player Experience
- Controls feel responsive
- AI creates unique encounters
- Clear visual feedback
- Smooth performance

### Technical Achievement
- AI integration working
- Blockchain rewards functional
- Cross-browser compatibility
- Professional polish

### Portfolio Impact
- Demonstrates modern tech stack
- Shows game design skills
- Proves Web3 understanding
- Highlights AI implementation

## Future Expansion Ideas (Post-Demo)

- Multiplayer dragon battles
- NFT dragon customization
- Procedural level generation
- Community tournaments
- Mobile app version

---

*This GDD represents the complete vision for the Plasma Thief demo, balancing ambitious features with realistic scope for a solo developer portfolio piece.*