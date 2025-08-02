# Product Requirements Document: Plasma Thief Sky Rogue Unity Development

## Executive Summary
Build a WebGL-based dragon combat game demonstrating cutting-edge Unity MCP integration, AI-powered adaptive bosses, and seamless Web2-to-Web3 onboarding using Solana blockchain.

## Technology Stack
- **Development Environment**: Unity 2022.3 LTS + Cursor IDE + Claude Code
- **MCP Integration**: Unity MCP Server (bleeding-edge AI-assisted development)
- **Backend**: Supabase (database + Web2/Web3 authentication bridge)
- **Blockchain**: Solana (Unity SDK from Asset Store)
- **Workflow Automation**: n8n (token distribution, event handling)
- **AI Services**: Claude API (boss decisions) + Local ML (pattern recognition)
- **Deployment**: WebGL build on GitHub Pages/Netlify

## Critical Success Factors
1. Unity MCP integration working (3-5x productivity boost)
2. 60 FPS on desktop browsers, 30 FPS mobile
3. <50MB compressed build size
4. AI boss visibly adapts between playthroughs
5. Web2 users earn tokens without knowing blockchain

## Phase 1: Foundation Setup (Days 1-2)

### Task 1.1: Unity MCP Server Configuration
**Priority**: P0 - BLOCKER
**Acceptance Criteria**:
- Install Unity MCP from GitHub: `https://github.com/CoderGamester/mcp-unity`
- Configure Cursor IDE connection
- Place CLAUDE.md in project root with game-specific guidelines
- Test with command: "Create a castle environment with terrain"
- Document exact working configuration

**Expected Output**: Natural language commands execute in Unity Editor

### Task 1.2: Project Structure
**Priority**: P0
**Create folders**:
```
Assets/
├── _Project/
│   ├── Scripts/
│   │   ├── Flight/
│   │   ├── Combat/
│   │   ├── AI/
│   │   ├── Web3/
│   │   └── Core/
│   ├── Prefabs/
│   ├── ScriptableObjects/
│   └── Resources/
├── _ImportedAssets/
├── Plugins/
│   ├── Solana/
│   └── Supabase/
└── WebGLTemplates/
```

### Task 1.3: Essential Package Installation
**Priority**: P0
**Unity Packages**:
- Cinemachine (camera system)
- Universal Render Pipeline (performance)
- TextMeshPro (UI)
- Addressables (memory optimization)

**External SDKs**:
- Solana Unity SDK (Asset Store verified)
- Supabase Unity SDK
- UnityAsync (performance optimization)

## Phase 2: Core Gameplay (Days 3-5)

### Task 2.1: Environment Setup
**Priority**: P1
**Requirements**:
- Import "Medieval Castle Pack Lite" (FREE) from Asset Store
- Terrain size: 500x500 units
- Add fog for atmosphere
- Baked lighting only (WebGL optimization)

**Test**: Achieve stable 60 FPS with environment loaded

### Task 2.2: Dragon Integration
**Priority**: P1
**Asset Options** (in order of preference):
1. Purchase: "Dragons - Fantasy RPG" ($20-40) with animations
2. Free: "Dragon for Boss Monster: HP" 
3. Fallback: 3D AI Studio generation + Mixamo

**Dragon Prefab Structure**:
```
Dragon_Root
├── Model (with LODs)
├── Flight_Controller (3rd Person + Fly Mode asset)
├── Weapon_System
│   ├── Plasma_Orb_Spawn
│   └── Fireball_Spawn
└── Effects
```

### Task 2.3: Flight Controller Enhancement
**Priority**: P1
**Base**: "3rd Person Controller + Fly Mode" (FREE - Asset ID: 28647)
**Enhancements Required**:
```csharp
public class EnhancedDragonFlight : MonoBehaviour
{
    [Header("Flight Physics")]
    public float baseSpeed = 30f;
    public float boostSpeed = 60f;
    public float rotationSpeed = 45f;
    
    [Header("WebGL Optimizations")]
    public bool useMobileControls = false;
    public float inputSmoothing = 0.1f;
    
    // Implement 6DOF movement
    // Add boost/brake mechanics
    // Smooth camera follow
}
```

**Acceptance**: Smooth flight at 60 FPS, responsive controls, no jitter

## Phase 3: Combat Systems (Days 6-8)

### Task 3.1: Dual Weapon System
**Priority**: P1
**Plasma Orb (Primary)**:
- Damage: 100 (instant kill)
- AOE: 10-foot radius
- Cooldown: 2 seconds
- Visual: Large blue energy sphere
- Pooled objects: 10

**Rapid Fireballs (Secondary)**:
- Damage: 25 per hit
- Fire rate: 5/second
- No AOE
- Visual: Small orange projectiles
- Pooled objects: 50

### Task 3.2: Enemy Implementation
**Priority**: P1
**Enemy Types**:
1. Archer Tower: 1 HP, fixed position
2. Ballista: 2 HP, predictive targeting
3. Soldier Groups: 5-10 units, area vulnerable

**Performance Target**: 30 active enemies without FPS drop

## Phase 4: AI Integration (Days 9-11)

### Task 4.1: Multi-Layer AI System
**Priority**: P0
**Architecture**:
```csharp
public class AIBossSystem : MonoBehaviour
{
    // Layer 1: Strategic AI (Claude API)
    private async Task<Strategy> GetStrategicDecision()
    {
        // Call every 10-15 seconds
        // Cache similar scenarios
        // Cost: ~$0.01 per session
    }
    
    // Layer 2: Tactical AI (Local)
    private void UpdateTacticalBehavior()
    {
        // Real-time reactions
        // Pattern recognition
        // No API calls
    }
    
    // Layer 3: Learning AI (Supabase)
    private void StorePlayerPatterns()
    {
        // Heat maps
        // Route preferences
        // Combat style analysis
    }
}
```

### Task 4.2: AI Features
**Priority**: P1
- Dynamic narration based on combat
- Procedural taunts using player data
- Environmental adaptation between levels
- Visual indicators of AI learning

## Phase 5: Web3 Integration (Days 12-14)

### Task 5.1: Supabase Authentication Bridge
**Priority**: P0
**Flow**:
1. Email/password registration (Web2)
2. Auto-generate Solana wallet (background)
3. Link wallet to Supabase user record
4. Progressive Web3 feature unlock

**Database Schema**:
```sql
-- Users table with Web2/Web3 bridge
CREATE TABLE users (
    id UUID PRIMARY KEY,
    email TEXT UNIQUE,
    wallet_address TEXT,
    wallet_encrypted TEXT,
    onboarding_stage TEXT,
    created_at TIMESTAMP
);

-- AI learning data
CREATE TABLE ai_patterns (
    user_id UUID REFERENCES users,
    level_number INT,
    heat_map JSONB,
    combat_style JSONB,
    boss_effectiveness FLOAT
);
```

### Task 5.2: Token Distribution
**Priority**: P1
**n8n Workflow**:
- Webhook: Level completion event
- Validate: Check anti-cheat metrics
- Process: Calculate token reward
- Distribute: Solana transaction
- Update: Supabase records

## Phase 6: Optimization & Polish (Days 15-18)

### Task 6.1: WebGL Optimization
**Priority**: P0
**Requirements**:
- Texture atlasing (reduce draw calls)
- LOD system (3 levels per model)
- Object pooling (all projectiles/effects)
- Compressed audio (Vorbis)
- Build stripping (remove unused code)

**Target Metrics**:
- Build size: <50MB compressed
- Load time: <20 seconds
- Memory: <256MB heap

### Task 6.2: Progressive Loading
**Priority**: P1
- Initial load: Menu + core systems
- Background load: Level assets
- On-demand: Audio/effects

## Testing Protocol

### Performance Testing (Every Build)
1. FPS counter always visible
2. Memory profiler checks
3. Build size monitoring
4. Load time measurement

### AI Testing
1. Boss makes different decisions each run
2. Level 2 enemies use Level 1 data
3. API costs stay under $0.02/session

### Web3 Testing
1. New user onboarding <60 seconds
2. Token rewards appear instantly
3. No blockchain knowledge required

## Risk Mitigation

### High Risk: MCP Setup Failure
**Mitigation**: 
- Allocate 2 full days for setup
- Have manual Unity workflow backup
- Document every working configuration

### Medium Risk: AI API Costs
**Mitigation**:
- Implement aggressive caching (70% reduction)
- Local fallback behaviors
- Rate limiting per session

### Medium Risk: WebGL Performance
**Mitigation**:
- Test builds every 2 days
- Progressive optimization
- Mobile quality settings

## Sub-Agent Workflow for Claude Code

### Agent 1: Explorer
"Analyze the current Unity project state and identify what needs to be implemented next based on the PRD phases."

### Agent 2: Planner
"Create a detailed implementation plan for [specific feature] including all Unity components, scripts, and integrations needed."

### Agent 3: Implementer
"Generate the complete code implementation for [specific feature] following Unity best practices and WebGL optimization guidelines."

### Agent 4: Validator
"Review the implementation for performance issues, potential bugs, and adherence to PRD requirements."

## Success Metrics

### Technical Metrics
- [ ] MCP commands execute successfully
- [ ] 60 FPS desktop, 30 FPS mobile
- [ ] <50MB build size
- [ ] <20 second load time
- [ ] <$0.02 per play session

### Gameplay Metrics
- [ ] Flight feels responsive and fun
- [ ] Combat is satisfying and powerful
- [ ] AI boss behavior varies each playthrough
- [ ] Players discover Web3 naturally

### Portfolio Metrics
- [ ] Demonstrates MCP integration
- [ ] Shows AI implementation skills
- [ ] Proves Web3 understanding
- [ ] Professional polish throughout

## CLAUDE.md File Template
```markdown
# Plasma Thief Sky Rogue - Development Guidelines

## Project Context
WebGL dragon combat game with AI bosses and Web3 integration.
Target: 60 FPS desktop, 30 FPS mobile, <50MB build.

## Code Standards
- Use object pooling for all projectiles
- Implement LOD for all 3D models
- Cache all AI responses
- Comment performance-critical sections

## Unity Specific
- Use Addressables for asset loading
- Baked lighting only
- Single terrain chunk
- Maximum 30 active enemies

## Current Phase
[Update this as you progress through PRD phases]

## Known Issues
[Document any blockers or workarounds]
```

---

*This PRD-style prompt maximizes Claude Code's effectiveness by providing clear structure, specific requirements, and measurable success criteria for each development phase.*