# PlasmaDragon - Development TODO List (FRESH START)

## 🚀 Project Reset - January 26, 2025

### 🎯 **Why We Started Over**
The previous attempt became cluttered with:
- Mixed file organization (imported assets scattered everywhere)
- Unity 6000.x compatibility issues 
- No clear separation between custom code and third-party assets
- Platform still set to Standalone instead of WebGL

### ✅ **What We've Accomplished (Fresh Start)**
- [x] Backed up previous work to BACKUP_PRE_FRESH_START/
- [x] Created proper Unity project structure with _Project folder
- [x] Set up comprehensive .gitignore for Unity
- [x] Created Packages/manifest.json with essential packages
- [x] Added Unity MCP Bridge package to manifest
- [x] Created assembly definition (PlasmaDragon.Runtime.asmdef)
- [x] Initialized Git repository
- [x] Created initial core scripts (GameManager, InputManager)
- [x] Opened project in Unity 6.0 LTS (latest stable)
- [x] Switched to WebGL platform
- [x] Created testFlight_001 scene with working flying cube
- [x] Pushed to GitHub: https://github.com/Agentic-Person/PlasmaDragon

### 📋 **Immediate Next Steps (Dragon First Approach)**
1. [x] ~~Open Unity 6.0 LTS~~ ✅ Done
2. [x] ~~Switch to WebGL platform~~ ✅ Done
3. [x] ~~Basic flying cube working~~ ✅ Done
4. [ ] **PRIORITY: Import/Purchase Dragon Model**
5. [ ] Replace flying cube with dragon model
6. [ ] Test flight controls with dragon (ensure smooth movement)
7. [ ] THEN import environment assets (castle/island)
8. [ ] Test Unity MCP connection with commands

### 🏗️ **Project Structure (Best Practices)**
```
PlasmaDragon/
├── Assets/
│   ├── _Project/              ← OUR CODE (underscore keeps at top)
│   │   ├── Scripts/
│   │   ├── Prefabs/
│   │   ├── Materials/
│   │   ├── Models/
│   │   ├── Textures/
│   │   ├── Audio/
│   │   ├── Scenes/
│   │   └── ScriptableObjects/
│   ├── ImportedAssets/        ← THIRD-PARTY ASSETS
│   ├── Plugins/               ← EXTERNAL LIBRARIES
│   └── StreamingAssets/       ← RUNTIME LOADED CONTENT
```

## 📚 **Lessons Learned**
1. **Always create folder structure BEFORE importing any assets**
2. **Switch to WebGL platform immediately after project creation**
3. **Unity 6.0 LTS is stable** - no need to downgrade to 2022.3
4. **Keep custom code in _Project folder, imported assets separate**
5. **Use assembly definitions for better compilation times**
6. **Test WebGL builds weekly to catch issues early**
7. **Dragon model first** - better for visual impact and scale reference

## 🎮 **Development Phases (Updated)**

### Phase 1: Foundation (Week 1) - IN PROGRESS
- [x] Project structure setup
- [x] Git initialization
- [x] Unity project creation in 6.0 LTS
- [x] WebGL platform configuration
- [ ] Unity MCP integration test
- [x] Basic scene setup (testFlight_001 with flying cube)
- [ ] Import and organize purchased assets

### Phase 2: Core Gameplay (Week 2)
- [ ] Dragon flight controller integration
- [ ] Basic combat system (dual weapons)
- [ ] Enemy placement and AI
- [ ] Level 1 environment setup

### Phase 3: AI Boss System (Week 3)
- [ ] Claude API integration
- [ ] Boss behavior state machine
- [ ] Caching system for API responses
- [ ] Adaptive difficulty system

### Phase 4: Web3 Integration (Week 4)
- [ ] Supabase authentication
- [ ] Solana wallet generation
- [ ] Token reward system
- [ ] n8n automation setup

### Phase 5: Polish & Optimization (Week 5)
- [ ] Visual effects enhancement
- [ ] WebGL-specific optimizations
- [ ] UI/UX improvements
- [ ] Audio integration

### Phase 6: Testing & Deployment (Week 6)
- [ ] Comprehensive testing
- [ ] Performance profiling
- [ ] Landing page creation
- [ ] Final deployment

## 🎯 **Success Metrics**
- [ ] Clean, professional code structure maintained
- [ ] Unity MCP commands working smoothly
- [ ] Stable 60 FPS on desktop WebGL
- [ ] AI Boss provides unique encounters each playthrough
- [ ] Web3 onboarding < 60 seconds
- [ ] Build size < 50MB
- [ ] Load time < 20 seconds

## 🛠️ **Technical Stack**
- **Unity Version**: 6.0 LTS (latest stable)
- **Render Pipeline**: URP (for WebGL performance)
- **Platform**: WebGL
- **MCP Integration**: justinpbarnett/unity-mcp
- **AI**: Claude API for boss behavior
- **Web3**: Solana Unity SDK
- **Backend**: Supabase + n8n

## 📝 **Daily Development Checklist**
- [ ] Update TODO.md with progress
- [ ] Test in WebGL build (weekly)
- [ ] Commit changes with clear messages
- [ ] Document any issues or learnings
- [ ] Keep file structure organized

## 🚨 **Critical Reminders**
1. **DO NOT** import assets directly into Assets/ root
2. **DO NOT** forget to switch to WebGL platform (already done!)
3. **DO** use Unity 6.0 LTS (current stable version)
4. **DO NOT** mix custom code with imported assets
5. **ALWAYS** test MCP commands after Unity restarts

## 📅 **Timeline Status**
- **Start Date**: January 26, 2025
- **Target Completion**: 4-6 weeks
- **Current Week**: 1
- **Hours Invested**: ~2 hours (fresh start)

---

**Remember**: This is a portfolio piece. Quality over speed, but maintain momentum!