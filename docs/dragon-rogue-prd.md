# Dragon Rogue Product Requirements Document (PRD)

## 1. Executive Summary

### Product Name
Plasma Thief: Sky Rogue (Web3 Gaming Demo)

### Product Vision
Create a portfolio demonstration that showcases the seamless integration of traditional gaming, artificial intelligence, and blockchain technology through an engaging aerial combat experience.

### Success Criteria
- Functional 2-3 level demo deployable in 3-4 weeks
- Demonstrates AI-powered adaptive gameplay
- Proves Web2-to-Web3 onboarding capability
- Achieves 60 FPS on desktop browsers
- Generates interest from Web3 gaming companies

## 2. Product Objectives

### Primary Objectives
1. **Demonstrate Technical Competency**: Show ability to integrate Unity, AI APIs, and blockchain
2. **Showcase Game Design**: Create engaging gameplay with minimal scope
3. **Prove Web3 Understanding**: Implement frictionless onboarding and token rewards
4. **Build Portfolio Piece**: Complete professional demo for job applications

### Secondary Objectives
- Learn MCP integration with Unity
- Explore AI-powered game development
- Understand Web3 gaming economics
- Create reusable architecture

## 3. User Personas

### Primary: Casual Web2 Gamer
- **Age**: 18-35
- **Technical Level**: Mid
- **Goals**: Play fun arcade games
- **Pain Points**: Complicated game mechanics
- **Success**: "This is fun! Wait, I earned real money?"

### Secondary: Web3 Gaming Recruiter
- **Age**: 28-45
- **Technical Level**: High
- **Goals**: Find developers who understand gaming + blockchain
- **Pain Points**: Many applicants lack practical Web3 experience
- **Success**: "This developer can build what we need"

### Tertiary: Fellow Developers
- **Age**: 22-40
- **Technical Level**: Very High
- **Goals**: Learn from implementation
- **Pain Points**: Lack of Web3 gaming examples
- **Success**: "I can see how this was built"

## 4. Feature Requirements

### 4.1 Core Features (MVP)

#### F1: Flight Combat System
- **Priority**: P0 (Critical)
- **Description**: Fully functional dragon flight with dual weapon system
- **Acceptance Criteria**:
  - 360° flight movement using "3rd Person Controller + Fly Mode" asset
  - Plasma orb primary weapon (area damage, one-shots groups)
  - Rapid fireball secondary weapon (machine-gun style)
  - Responsive controls (<50ms input lag)
  - Visual feedback for both weapon types

#### F2: AI-Powered Boss
- **Priority**: P0 (Critical)
- **Description**: Boss that uses AI for decision-making
- **Acceptance Criteria**:
  - Integrates with Claude/GPT API
  - Makes decisions every 10-15 seconds
  - Visibly different behavior each playthrough
  - Costs <$0.02 per session

#### F3: Web3 Token Rewards
- **Priority**: P0 (Critical)
- **Description**: Blockchain rewards for completing levels
- **Acceptance Criteria**:
  - Auto-wallet generation
  - Instant token distribution
  - Visual confirmation of rewards
  - No user-facing gas fees

#### F4: Web Platform
- **Priority**: P0 (Critical)
- **Description**: Landing page with game embed
- **Acceptance Criteria**:
  - Loads in <5 seconds
  - Email/password registration
  - Embedded Unity WebGL game
  - Mobile responsive design

### 4.2 Secondary Features

#### F5: Adaptive Enemy AI
- **Priority**: P1 (High)
- **Description**: Enemies that learn between levels
- **Acceptance Criteria**:
  - Heat map tracking of player routes
  - Visible behavior changes in Level 2
  - No API calls needed

#### F6: Progressive Difficulty
- **Priority**: P1 (High)
- **Description**: Level 2 uses data from Level 1
- **Acceptance Criteria**:
  - Boss remembers player tactics
  - Enemies patrol learned routes
  - Clear adaptation visible

#### F7: Visual Polish
- **Priority**: P2 (Medium)
- **Description**: Professional appearance
- **Acceptance Criteria**:
  - Particle effects for combat
  - Smooth camera transitions
  - Consistent art style
  - Basic post-processing

### 4.3 Optional Features

#### F8: Level 3
- **Priority**: P3 (Low)
- **Description**: Additional level with flying boss
- **Acceptance Criteria**:
  - New environment
  - Aerial boss battle
  - 10 token reward

#### F9: Leaderboard
- **Priority**: P3 (Low)
- **Description**: Global high scores
- **Acceptance Criteria**:
  - Supabase integration
  - Anti-cheat measures
  - Daily/weekly/all-time views

## 5. Technical Requirements

### 5.1 Performance Requirements
- **Frame Rate**: 60 FPS (desktop), 30 FPS (mobile)
- **Load Time**: <20 seconds on 10 Mbps connection
- **Build Size**: <50MB compressed
- **Memory Usage**: <256MB RAM

### 5.2 Browser Support
- **Chrome**: v90+ (Primary)
- **Firefox**: v88+
- **Safari**: v14+
- **Edge**: v90+
- **Mobile**: Chrome/Safari on iOS/Android

### 5.3 Infrastructure Requirements
- **Game Engine**: Unity 2022.3 LTS with Universal Render Pipeline
- **Development Tools**: 
  - Cursor IDE with Claude Code integration
  - Unity MCP Server for natural language development
  - Git with Unity-specific .gitignore
- **Backend Stack**:
  - Supabase (PostgreSQL + Auth + Realtime)
  - n8n (workflow automation)
  - Cloudflare (CDN and DDoS protection)
- **Blockchain**: 
  - Solana (Mainnet-beta for production)
  - Solana Unity SDK (Asset Store verified)
- **AI Services**:
  - Claude API for strategic decisions
  - Local ML for tactical reactions
  - Redis for response caching

### 5.4 Integration Requirements
- **Unity MCP**: Natural language Unity control
- **Supabase SDK**: Authentication and database
- **Solana Web3.js**: Wrapped in Unity-compatible layer
- **Analytics**: Unity Analytics + Supabase events
- **Monitoring**: Sentry for error tracking

## 6. User Experience Requirements

### 6.1 Onboarding Flow
1. **Landing Page** (30s)
   - Hero video of gameplay
   - Single "Play Now" CTA
   - Trust indicators

2. **Registration** (30s)
   - Email and password only
   - No wallet setup needed
   - Instant access

3. **Tutorial** (2m)
   - Interactive flight training
   - Combat practice
   - Objective explanation

4. **Gameplay** (5-10m)
   - Immediate action
   - Clear objectives
   - Visible progress

### 6.2 Accessibility
- **Controls**: Remappable keys
- **Visual**: High contrast UI option
- **Audio**: Subtitles for boss taunts
- **Performance**: Low quality mode

## 7. Content Requirements

### 7.1 3D Assets
- **Dragon Model**: 1 (free from Asset Store)
- **Enemy Models**: 3-4 types
- **Environment**: Island with castle
- **Props**: 10-15 items
- **Effects**: 5-10 particle systems

### 7.2 Audio Assets
- **Music**: 2-3 tracks (combat, victory, menu)
- **SFX**: 15-20 effects
- **Voice**: AI-generated boss taunts

### 7.3 UI Assets
- **HUD Elements**: Health, shields, tokens
- **Menus**: Main, pause, victory
- **Icons**: 10-15 custom icons
- **Fonts**: 2 (headers, body)

## 8. Development Milestones

### Milestone 1: Core Systems (Week 1)
- [ ] MCP integration complete
- [ ] Basic flight working
- [ ] Island environment set up
- [ ] Dragon model imported

### Milestone 2: Combat (Week 2)
- [ ] Projectile system
- [ ] Enemy AI basics
- [ ] Boss implementation
- [ ] Health/damage system

### Milestone 3: Web3 (Week 3)
- [ ] Wallet integration
- [ ] Token rewards
- [ ] Landing page
- [ ] WebGL build

### Milestone 4: Polish (Week 4)
- [ ] AI adaptation working
- [ ] Visual effects
- [ ] Bug fixes
- [ ] Performance optimization

## 9. Risks and Mitigation

### Technical Risks
1. **WebGL Performance**
   - Risk: Poor performance on low-end devices
   - Mitigation: Aggressive LOD, quality settings

2. **AI API Costs**
   - Risk: Expensive API calls
   - Mitigation: Caching, rate limiting

3. **Web3 Complexity**
   - Risk: Wallet issues
   - Mitigation: Custodial wallet, simple flow

### Design Risks
1. **Scope Creep**
   - Risk: Adding too many features
   - Mitigation: Strict MVP focus

2. **Difficulty Balance**
   - Risk: Too hard/easy
   - Mitigation: Playtesting, adjustable AI

## 10. Success Metrics

### Quantitative Metrics
- Load time <20 seconds
- 60 FPS on target hardware
- <$0.02 per play session (AI costs)
- 5-minute average session
- 90% tutorial completion

### Qualitative Metrics
- "The AI boss felt different each time"
- "I didn't realize I was using blockchain"
- "The flight controls feel great"
- "I want to play more levels"

## 11. Development Best Practices

### Unity MCP Workflow
1. **PRD-Style Prompts**: Always provide context in CLAUDE.md
2. **Sub-Agent Pattern**: Use multiple specialized agents
3. **Incremental Testing**: Test after each MCP command
4. **Documentation**: Record all working configurations

### Performance Optimization
1. **Asset Pipeline**:
   - Texture max 1024x1024
   - Audio compressed to Vorbis
   - Models under 10k polygons
   - Use texture atlasing

2. **Code Optimization**:
   - Object pooling mandatory
   - Async/await over coroutines
   - Cache all AI responses
   - Profile every build

### WebGL Specific
1. **Memory Management**:
   - Addressables for dynamic loading
   - Aggressive garbage collection
   - Unload unused assets
   - Monitor heap size

2. **Build Settings**:
   - Strip unused code
   - Compress textures
   - Disable exceptions
   - Use IL2CPP

### AI Cost Control
1. **Caching Strategy**:
   - Redis for API responses
   - 70% cache hit target
   - TTL: 5 minutes
   - Similar context grouping

2. **Fallback Chain**:
   - Check cache first
   - Use local AI if available
   - Default behaviors last
   - Never block gameplay

## 12. Budget Summary

### Development Costs
- **One-Time**: $20-65 (assets + domain)
- **Monthly**: $5-30 (AI API + tools)
- **Total (1 month)**: $25-95

### Time Investment
- **Development**: 160 hours (4 weeks)
- **Testing**: 20 hours
- **Polish**: 20 hours
- **Total**: 200 hours

---

*This PRD defines the complete product requirements for the Plasma Thief demo, ensuring all stakeholders understand the scope, objectives, and success criteria.*