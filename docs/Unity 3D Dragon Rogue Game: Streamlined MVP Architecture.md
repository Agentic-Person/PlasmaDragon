# Unity 3D Dragon Rogue Game: Enhanced MVP Architecture

## Project Overview

A cutting-edge portfolio demo showcasing Unity MCP integration, AI-powered adaptive gameplay, and seamless Web2-to-Web3 onboarding through Supabase authentication bridge. Target: 2-3 playable levels demonstrating blockchain gaming without friction.

## Enhanced Technology Stack

### Core Development
- **Unity 2022.3 LTS**: Primary game engine with WebGL target
- **Cursor IDE + Claude Code**: AI-assisted development with PRD-style prompts
- **Unity MCP Server**: Revolutionary natural language Unity control (3-5x productivity)

### Backend Infrastructure
- **Supabase**: 
  - PostgreSQL database for game state
  - Web2/Web3 authentication bridge
  - Real-time AI learning data storage
  - Built-in RLS for security
- **n8n**: Workflow automation for token distribution and events
- **Solana Blockchain**: Fast, low-cost token transactions

### AI Integration
- **Claude API**: Strategic boss decisions (every 10-15s)
- **Local ML**: Tactical reactions and pattern recognition
- **Caching Layer**: 70% API cost reduction

### Performance Tools
- **UnityAsync**: Allocation-free coroutines
- **Addressables**: Dynamic asset loading
- **Universal Render Pipeline**: Optimized rendering

## Priority 1: Unity MCP Integration (Revolutionary Workflow)

### Why MCP First
- **3-5x Productivity Boost**: Natural language commands directly control Unity
- **Cutting-Edge Technology**: Very few developers have this experience
- **Portfolio Differentiator**: Demonstrates mastery of bleeding-edge tools

### Day 1 Setup Protocol

**MCP Unity Configuration**
```bash
# Clone the official Unity MCP server
git clone https://github.com/CoderGamester/mcp-unity.git

# Install dependencies
cd mcp-unity
npm install

# Configure in Cursor IDE settings.json
{
  "mcpServers": {
    "unity-mcp": {
      "command": "node",
      "args": ["path/to/mcp-unity/Server/build/index.js"],
      "env": {
        "UNITY_PROJECT_PATH": "/path/to/PlasmaThief"
      }
    }
  }
}
```

### PRD-Style Prompt Strategy
Create `CLAUDE.md` in project root:
```markdown
# Plasma Thief Development Context

## Current Sprint
Phase 1: Core flight mechanics with WebGL optimization

## Performance Constraints
- Target: 60 FPS desktop, 30 FPS mobile
- Memory budget: 256MB
- Build size: <50MB compressed

## Code Standards
- Always use object pooling for projectiles
- Implement LOD for all models
- Cache AI responses aggressively
```

### Sub-Agent Workflow
1. **Explorer Agent**: Analyzes current project state
2. **Planner Agent**: Creates implementation strategy
3. **Implementer Agent**: Generates optimized code
4. **Validator Agent**: Checks performance and bugs

## Priority 2: Flight Controller Selection

### Primary Controller: 3rd Person Controller + Fly Mode (Asset ID: 28647)
**Status: FREE - Use This First**
- Perfect starting point for dragon flight
- Includes basic movement mechanics
- Can be enhanced with custom scripts
- Quick implementation path

### Alternative Options (If Needed)

**Option B: Arcade Flight Controller** ($25.45)
- More advanced arcade controls
- Use if free option insufficient

**Option C: Custom Enhancement**
- Build on top of free controller
- Add roll/pitch/yaw improvements
- Custom dragon-specific features

## Priority 3: Asset Strategy (Budget-Conscious)

### Dragon Models Strategy

**Option A: Affordable Asset Store Dragon** (~$20-40)
- Search for dragons with included flight animations
- Keywords: "dragon animated", "flying dragon", "low poly dragon"
- Ensure WebGL compatibility

**Option B: 3D AI Generation (Backup Method)**
- Use 3D AI Studio or Meshy AI
- Generate dragon model
- Apply Mixamo flying animations
- Note: More time-consuming, use only if needed

### Environment Assets (FREE)

1. **Castle Scene** - You mentioned finding this
2. **Terrain Tools Sample Asset Pack** - Unity official
3. **Medieval Village Pack** - Various free options

### Combat Effects
- Unity Particle Pack (FREE)
- Basic projectile effects
- Plasma orb can be created with emission materials

## Simplified Game Design

### Core Gameplay Loop (Dragon-Only)

1. **Aerial Patrol Phase**
   - Fly around island fortress
   - Identify boss location
   - Dodge defensive fire

2. **Boss Combat Phase**
   - AI-powered boss with unique tactics
   - Plasma orb combat
   - Dynamic difficulty based on player skill

3. **Treasure Collection**
   - Dragon swoops down to grab treasure
   - No dismounting needed
   - Quick grab mechanic with animation

### Flight Mechanics Features
- Full 360° movement (roll, pitch, yaw)
- Boost/brake system
- Hover mode for precision
- Banking turns
- Dive attacks

## Priority 4: Development Phases

### Week 1: Core Systems

**Days 1-2: Environment & Flight**
- Set up MCP integration
- Import island/castle assets
- Implement flight controller
- Basic camera follow system

**Days 3-4: Dragon Setup**
- Import free dragon model
- Configure animations (fly, attack, grab)
- Add plasma orb shooting
- Visual effects setup

**Days 5-7: Combat Basics**
- Enemy turrets/archers
- Basic projectile system
- Health/damage mechanics
- Simple UI elements

### Week 2: AI & Web3

**Days 8-10: AI Boss Integration**
- Simple API connection (Claude/GPT)
- Boss decision-making system
- Adaptive difficulty
- Response caching for cost control

**Days 11-14: Web3 Setup**
- Solana wallet integration
- Token reward system
- Simple landing page
- WebGL deployment

### Week 3: Polish & Additional Content

**Days 15-17: Level 2**
- Same environment, smarter enemies
- AI learns from Level 1
- Visual feedback for adaptation

**Days 18-21: Final Polish**
- Optimization for WebGL
- Bug fixes
- Landing page improvements
- Demo video recording

## AI Implementation (Simplified)

### Boss AI Only
- API calls every 10-15 seconds
- Simple decision tree:
  - Attack pattern selection
  - Movement strategy
  - Special ability timing
- Cost: ~$0.01 per boss fight

### Military AI (Optional)
- Simple heat map system
- Remember player flight paths
- Adjust patrol routes
- No API needed

## Web3 Architecture (Minimal)

### Landing Page
- Single page design
- Dragon hero image/video
- "Play Now" button
- Auto-wallet creation

### Token System
- 10 tokens to start
- 3-5 tokens per boss defeated
- No complex tokenomics
- Focus on demonstration

## Technical Considerations

### WebGL Optimization
- Single terrain chunk
- 20-30 enemies maximum
- LOD for distant objects
- Compressed textures
- Target: 50MB build

### Performance Goals
- 60fps on desktop
- 30fps on mobile browsers
- 20-second load time
- Smooth flight controls

## Development Tools

### Essential
- Unity 2022.3 LTS
- Cursor IDE with MCP
- Claude Code integration
- Git for version control

### Asset Creation
- **3D AI Studio** ($9/month) - For props
- **Meshy AI** ($20/month) - Higher quality
- Use for: Treasures, environmental details

### Optional
- Cinemachine for camera
- Post-processing stack
- Unity Analytics

## Success Metrics

1. **Smooth Dragon Flight** - Feels great to control
2. **AI Boss Variety** - Different each playthrough
3. **Web3 Simplicity** - 60-second onboarding
4. **Visual Polish** - Looks professional
5. **Performance** - Runs smoothly in browser

## Minimal Viable Features

### Level 1: Introduction
- Fly dragon around island fortress
- Locate and defeat AI-powered boss
- Dragon swoops to collect treasure
- Basic enemy turrets/archers
- Earn 5 tokens

### Level 2: AI Adaptation  
- Same layout, smarter enemies
- Boss remembers Level 1 tactics
- Guards patrol previous flight paths
- Increased defensive coordination
- Earn 8 tokens for success

### Level 3 (Optional): New Challenge
- Different fortress layout
- New boss type with unique AI
- Demonstrates AI variety
- Earn 10 tokens

## Simplified AI Boss System

```csharp
public class AIBossController : MonoBehaviour
{
    private float aiDecisionInterval = 10f; // Cost control
    private Dictionary<string, object> playerHistory;
    
    public async void GetBossAction()
    {
        var context = new {
            playerPosition = player.position,
            playerAltitude = player.position.y,
            playerVelocity = player.velocity,
            bossHealth = health,
            previousPlayerActions = playerHistory,
            currentLevel = GameManager.currentLevel
        };
        
        // Simple prompt for variety
        var prompt = $"Boss combat decision: {JsonConvert.SerializeObject(context)}";
        var response = await CallAIAPI(prompt);
        ExecuteBossAction(response.action);
    }
}
```

### AI Cost Control
- Cache similar situations (70% reduction)
- Batch decisions when possible
- Fallback to procedural behavior
- Estimated cost: $0.01-0.02 per playthrough

## Performance Targets (WebGL)

### Memory Budget
- **Unity Heap**: 256MB maximum
- **Texture Memory**: 128MB
- **Audio**: 32MB compressed

### Frame Rate Goals
- **Desktop**: 60fps consistent
- **Mobile**: 30fps minimum
- **Loading**: <20 seconds

### Build Size Targets
- **Uncompressed**: <150MB
- **Compressed**: <50MB
- **Initial Download**: <10MB

### Optimization Checklist
- ✓ Texture atlasing for UI
- ✓ LOD for castle/terrain
- ✓ Object pooling for projectiles
- ✓ Compressed audio (Vorbis)
- ✓ Single terrain mesh
- ✓ Baked lighting only

## Budget Breakdown

### One-Time Costs
- **Unity Assets**:
  - Flight Controller: $0-25 (free option available)
  - Dragon Model: $0 (free assets)
  - Environment: $0 (free assets)
  - Effects: $0-20 (optional)
- **Domain/Hosting**: $20/year
- **Total One-Time**: $20-65

### Monthly Costs
- **AI Tools**:
  - 3D AI Studio: $9 (optional)
  - OR Meshy AI: $20 (optional)
- **API Costs**:
  - Claude/GPT: ~$5-10 (demo usage)
- **Infrastructure**:
  - Supabase: $0 (free tier)
  - Hosting: $0 (GitHub Pages)
- **Total Monthly**: $5-30

### Cost Per Player (Demo)
- API calls: ~$0.01-0.02
- Infrastructure: $0 (free tier)
- Blockchain fees: ~$0.001 (Solana)

## Technical Implementation Details

### Flight Physics Configuration
```csharp
public class DragonFlightController : MonoBehaviour
{
    [Header("Flight Parameters")]
    public float maxSpeed = 50f;
    public float acceleration = 10f;
    public float pitchSpeed = 45f;
    public float rollSpeed = 60f;
    public float yawSpeed = 30f;
    
    [Header("Combat")]
    public float fireRate = 0.5f;
    public GameObject plasmaOrbPrefab;
    public Transform firePoint;
}
```

### Web Platform Integration
```javascript
// Simple Unity WebGL embed
const unityConfig = {
    dataUrl: "Build/game.data",
    frameworkUrl: "Build/game.framework.js",
    codeUrl: "Build/game.wasm",
    streamingAssetsUrl: "StreamingAssets",
    companyName: "YourName",
    productName: "PlasmaThief",
    productVersion: "1.0"
};
```

### Token Distribution
```csharp
public class TokenRewardSystem : MonoBehaviour
{
    private async Task RewardTokens(int amount)
    {
        // Queue reward for n8n processing
        var reward = new TokenReward {
            wallet = playerWallet,
            amount = amount,
            reason = "boss_defeated",
            timestamp = DateTime.UtcNow
        };
        
        await SupabaseClient.Insert(reward);
    }
}
```

## Next Steps

1. Set up MCP integration with Cursor/Claude Code
2. Download free flight controller code (SpaceshipController)
3. Import "Dragon for Boss Monster: HP" (free)
4. Create basic island scene with free castle asset
5. Test flight mechanics
6. Implement basic combat system
7. Add AI boss integration
8. Set up Web3 connection
9. Deploy to WebGL

This streamlined approach maintains all crucial technical details while focusing on achievable MVP features: sophisticated flight controls, AI-powered bosses, and simple Web3 integration - perfect for a portfolio demonstration.