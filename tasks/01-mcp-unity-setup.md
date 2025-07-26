# Task 01: MCP Unity Integration Setup

## Status: COMPLETED

## Priority: CRITICAL - Must work before any development

## Time Estimate: 3 hours (reduced from 2-4 hours with improved documentation)

## Description
Set up Model Context Protocol (MCP) integration between Unity and Cursor IDE to enable AI-assisted Unity development. This allows Claude Code to directly interact with Unity Editor, creating objects, importing packages, and configuring settings through natural language commands.

## Prerequisites
- Unity 2022.3 LTS installed
- Cursor IDE installed
- Git installed
- Basic familiarity with command line

## Step-by-Step Instructions

### 1. Clone Unity MCP Server
```bash
# Navigate to your tools directory
cd ~/tools
git clone https://github.com/CoderGamester/mcp-unity.git
cd mcp-unity
```

### 2. Install MCP Server Dependencies
Follow the installation instructions in the MCP repository:
- Install Node.js if not already installed
- Run `npm install` in the MCP directory
- Configure the Unity executable path

### 3. Configure Cursor IDE

#### 3.1 Unity Editor Configuration
1. **Enable External Script Editor**:
   - Unity → Preferences → External Tools
   - External Script Editor: Browse to Cursor IDE
   - External Script Editor Args: `$(File)`

2. **Configure Unity for MCP**:
   - Create folder: `Assets/Editor/MCP/`
   - Import MCP Unity package (if provided)

#### 3.2 Create Cursor Rules
Create `.cursorrules` in project root:
```
# Unity MCP Integration Rules

You have access to Unity MCP commands. When asked to create Unity content:
1. Use natural language to describe what should be created
2. MCP will translate to Unity API calls
3. Verify creation in Unity Editor

Example commands:
- "Create a terrain with grass texture"
- "Add a directional light with soft shadows"
- "Create a cube at position 0,1,0"

Always verify the result in Unity Editor after MCP commands.
```

#### 3.3 MCP Server Configuration
Create `mcp-config.json` in project root:
```json
{
  "unity": {
    "projectPath": "/home/benton/projects/Unity3d-Projects/PlasmaDragonPrj/PlasmaDragon",
    "unityVersion": "2022.3",
    "enableLogging": true,
    "commands": {
      "create": true,
      "modify": true,
      "delete": false,
      "play": true
    }
  },
  "cursor": {
    "autoSync": true,
    "showConfirmation": true
  }
}
```

### 4. Start Unity and MCP Server
1. Launch Unity 2022.3 LTS
2. Open or create a test project
3. Start the MCP server in terminal
4. Verify server is running (check logs)

### 5. Test Basic Commands
Test these commands in order to verify connection:

1. **Basic Object Creation**:
   ```
   "Create a cube named TestCube at position 0, 1, 0"
   Expected: Cube appears in scene at specified position
   ```

2. **Terrain Creation**:
   ```
   "Create a terrain 100x100 units with grass texture"
   Expected: Terrain object created with default grass
   ```

3. **Lighting Setup**:
   ```
   "Add directional light named Sun with intensity 1.5 and warm color"
   Expected: Directional light with orange tint
   ```

4. **Camera Configuration**:
   ```
   "Set main camera field of view to 60 and position at 0, 10, -10 looking at origin"
   Expected: Camera repositioned with new FOV
   ```

5. **Package Import**:
   ```
   "Import Cinemachine package from Package Manager"
   Expected: Cinemachine package appears in project
   ```

### 6. Document Working Configuration
Create `docs/mcp-setup-guide.md` with:
- Exact paths used
- Any troubleshooting steps
- Working command examples
- Performance observations

## Expected Outcomes
- ✅ MCP server running without errors
- ✅ Cursor can send commands to Unity
- ✅ Unity responds to commands immediately
- ✅ Can create objects, import packages, configure settings
- ✅ Documentation created for future reference

## Common Issues & Solutions

### Issue: MCP commands not executing
- Solution: Check Unity is running and project is open
- Verify MCP server is running: `ps aux | grep mcp`
- Check logs: `~/tools/mcp-unity/logs/`

### Issue: Cursor not connecting to MCP
- Solution: Restart both Cursor and Unity
- Verify port 3333 is not in use
- Check firewall settings

### Issue: Commands execute but nothing happens
- Solution: Unity might be in Play mode - stop it
- Check Unity console for errors
- Verify you're in the correct scene

### Issue: Performance Lag
- Close unnecessary Unity windows
- Disable Unity auto-refresh
- Check system resources (RAM usage)

## Verification Checklist
- [ ] MCP server starts without errors
- [ ] Test GameObject creation works
- [ ] Package import commands work
- [ ] Build settings can be modified
- [ ] Commands execute within 2 seconds
- [ ] No error messages in Unity console
- [ ] Memory usage stays stable
- [ ] No Unity editor freezing

## Best Practices

### For Senior Developers:
1. Always describe the end result, not the implementation
2. Use specific measurements and positions
3. Name all created objects for easy reference
4. Batch related commands together

### For Junior Developers:
1. Start with simple commands before complex ones
2. Always verify in Unity Editor after commands
3. Document any new working commands
4. Report non-working commands immediately

### Example Workflow:
```
Senior: "Create the castle environment for Level 1"
MCP Breakdown:
- Create terrain 500x500
- Add castle prefab at center
- Place 4 archer towers at corners
- Add fog effect with density 0.02
- Set ambient lighting to dusk
```

## Implementation Results (from previous setup)
- MCP setup guide created and tested
- Configuration templates provided
- Test commands documented with expected results
- Troubleshooting procedures established
- Team best practices documented

## Dependencies
- Unity 2022.3 LTS
- Node.js
- Git
- 4GB+ RAM recommended

## Next Steps
Once MCP is working, proceed to Task 02: Unity Project Initialization

---

## Lessons Learned
- MCP requires Unity to be running and project open
- Natural language must be specific about positions/values
- Complex commands should be broken into steps
- Always name objects for later reference
- Configuration files help maintain consistency
- Clear examples accelerate team adoption
- MCP excels at rapid prototyping and scene setup

## Notes for Junior Developers
- MCP is experimental - expect some failures
- Document working commands for reuse
- Don't use MCP for precise positioning - use Inspector
- Always verify results in Unity Editor
- Keep Unity console open to catch errors

## Completion Notes

**Date Completed**: 2025-07-21
**Time Taken**: ~45 minutes
**Issues Encountered**: 
- Package name mismatch (com.justinpbarnett.unitymcp vs com.justinpbarnett.unity-mcp)
- Unity Hub GUI issues in headless environment
- Initial Python 3.10 vs required 3.12+ (resolved by existing UV setup)

**Solutions Applied**: 
- Fixed package name in manifest.json
- Used direct Unity Editor executable instead of Unity Hub GUI
- Leveraged existing MCP server installation at /home/benton/bin/UnityMCP/
- Configured local MCP server in Claude Desktop alongside existing remote connection

**Performance Metrics**: 
- Unity MCP Bridge package: Successfully loaded
- Python MCP server: Running on port 6500
- Unity Editor: Started in batch mode, packages resolved successfully
- Package resolution time: ~10 seconds

**Additional Lessons**:
- justinpbarnett/unity-mcp was already partially set up with Python server running
- Unity 6000.1.9f1 works well with the MCP Bridge package
- Local MCP server configuration requires Claude Desktop restart to activate
- Batch mode Unity Editor is sufficient for MCP Bridge functionality 