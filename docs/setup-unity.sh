#!/bin/bash

# PlasmaDragon Unity Project Setup Script
# This script prepares the project structure for Unity initialization

echo "🐉 PlasmaDragon Unity Setup Script"
echo "================================="

# Check if we're in the right directory
if [ ! -f "README.md" ] || [ ! -d "Assets" ]; then
    echo "❌ Error: Must run from PlasmaDragon project root"
    exit 1
fi

echo "✅ Project directory verified"

# Create required directories if they don't exist
echo "📁 Creating Unity folder structure..."

# Create all required directories
directories=(
    "Assets/_Project/Scripts/Player/Dragon"
    "Assets/_Project/Scripts/Player/Input"
    "Assets/_Project/Scripts/AI/Boss"
    "Assets/_Project/Scripts/AI/Enemies"
    "Assets/_Project/Scripts/Combat/Weapons"
    "Assets/_Project/Scripts/Combat/Projectiles"
    "Assets/_Project/Scripts/Web3/Authentication"
    "Assets/_Project/Scripts/Web3/Blockchain"
    "Assets/_Project/Scripts/Core/Managers"
    "Assets/_Project/Scripts/Core/Systems"
    "Assets/_Project/Scripts/UI/Menus"
    "Assets/_Project/Scripts/UI/HUD"
    "Assets/_Project/Scripts/Utilities"
    "Assets/_Project/Prefabs/Player"
    "Assets/_Project/Prefabs/Enemies"
    "Assets/_Project/Prefabs/Environment"
    "Assets/_Project/Prefabs/Weapons"
    "Assets/_Project/Prefabs/Effects"
    "Assets/_Project/Materials/Characters"
    "Assets/_Project/Materials/Environment"
    "Assets/_Project/Materials/Effects"
    "Assets/_Project/Textures/Characters"
    "Assets/_Project/Textures/Environment"
    "Assets/_Project/Textures/UI"
    "Assets/_Project/Audio/SFX"
    "Assets/_Project/Audio/Music"
    "Assets/_Project/Audio/Ambience"
    "Assets/_Project/ScriptableObjects/Weapons"
    "Assets/_Project/ScriptableObjects/AI"
    "Assets/_Project/ScriptableObjects/GameData"
    "Assets/_Project/Resources/Prefabs"
    "Assets/_Project/Resources/Materials"
    "Assets/Plugins/Solana/Scripts"
    "Assets/Plugins/Supabase/Scripts"
    "Assets/WebGLTemplates/PlasmaDragon"
    "Assets/StreamingAssets/Data"
    "Assets/StreamingAssets/Config"
)

for dir in "${directories[@]}"; do
    mkdir -p "$dir"
    echo "  ✓ Created $dir"
done

# Create .keep files to ensure empty directories are tracked by git
echo "📄 Creating .keep files for git tracking..."
find Assets -type d -empty -exec touch {}/.keep \;

# Create initial configuration files
echo "⚙️ Creating configuration files..."

# Create a basic assembly definition for our scripts
cat > "Assets/_Project/Scripts/PlasmaDragon.Runtime.asmdef" << 'EOF'
{
    "name": "PlasmaDragon.Runtime",
    "rootNamespace": "PlasmaDragon",
    "references": [
        "Unity.TextMeshPro",
        "Unity.Cinemachine",
        "Unity.Addressables",
        "Unity.ResourceManager"
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
EOF

echo "  ✓ Created assembly definition"

# Create WebGL template files
cat > "Assets/WebGLTemplates/PlasmaDragon/index.html" << 'EOF'
<!DOCTYPE html>
<html lang="en-us">
<head>
    <meta charset="utf-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
    <title>PlasmaDragon - Plasma Thief: Sky Rogue</title>
    <style>
        body { margin: 0; padding: 0; overflow: hidden; background: #000; }
        #unity-container { width: 100vw; height: 100vh; }
        #unity-canvas { width: 100%; height: 100%; display: block; }
        #unity-loading-bar { position: absolute; left: 50%; top: 50%; transform: translate(-50%, -50%); }
        #unity-logo { width: 154px; height: 130px; background: url('unity-logo-dark.png') no-repeat center; }
        #unity-progress-bar-empty { width: 141px; height: 18px; margin-top: 10px; background: url('progress-bar-empty-dark.png') no-repeat center; }
        #unity-progress-bar-full { width: 0%; height: 18px; margin-top: 10px; background: url('progress-bar-full-dark.png') no-repeat center; }
        .unity-mobile-warning { display: none; }
    </style>
</head>
<body>
    <div id="unity-container">
        <canvas id="unity-canvas"></canvas>
        <div id="unity-loading-bar">
            <div id="unity-logo"></div>
            <div id="unity-progress-bar-empty">
                <div id="unity-progress-bar-full"></div>
            </div>
        </div>
    </div>
    <script src="Build/{{{ LOADER_FILENAME }}}"></script>
    <script>
        var buildUrl = "Build";
        var loaderUrl = buildUrl + "/{{{ LOADER_FILENAME }}}";
        var config = {
            dataUrl: buildUrl + "/{{{ DATA_FILENAME }}}",
            frameworkUrl: buildUrl + "/{{{ FRAMEWORK_FILENAME }}}",
            codeUrl: buildUrl + "/{{{ CODE_FILENAME }}}",
            streamingAssetsUrl: "StreamingAssets",
            companyName: "{{{ COMPANY_NAME }}}",
            productName: "{{{ PRODUCT_NAME }}}",
            productVersion: "{{{ PRODUCT_VERSION }}}",
        };

        var container = document.querySelector("#unity-container");
        var canvas = document.querySelector("#unity-canvas");
        var loadingBar = document.querySelector("#unity-loading-bar");
        var progressBarFull = document.querySelector("#unity-progress-bar-full");

        canvas.style.width = window.innerWidth + 'px';
        canvas.style.height = window.innerHeight + 'px';

        createUnityInstance(canvas, config, (progress) => {
            progressBarFull.style.width = 100 * progress + "%";
        }).then((unityInstance) => {
            loadingBar.style.display = "none";
        }).catch((message) => {
            alert(message);
        });
    </script>
</body>
</html>
EOF

echo "  ✓ Created WebGL template"

# Create initial ScriptableObject templates
cat > "Assets/_Project/ScriptableObjects/README.md" << 'EOF'
# ScriptableObject Data Files

This directory contains game configuration data:

- **Weapons/**: Weapon stats and configurations
- **AI/**: AI behavior parameters
- **GameData/**: Level settings, player stats, etc.

## Usage
Right-click in Unity → Create → PlasmaDragon → [Type]
EOF

echo "  ✓ Created ScriptableObject documentation"

# Create a sample .env.template for configuration
cat > ".env.template" << 'EOF'
# PlasmaDragon Configuration Template
# Copy to .env and fill in your values

# AI API Configuration
CLAUDE_API_KEY=your_claude_api_key_here
CLAUDE_API_ENDPOINT=https://api.anthropic.com/v1/messages

# Supabase Configuration
SUPABASE_URL=your_supabase_url_here
SUPABASE_ANON_KEY=your_supabase_anon_key_here

# Solana Configuration
SOLANA_RPC_URL=https://api.devnet.solana.com
SOLANA_NETWORK=devnet

# Game Configuration
ENABLE_DEBUG_MODE=true
TARGET_FPS_DESKTOP=60
TARGET_FPS_MOBILE=30
EOF

echo "  ✓ Created environment template"

# Create development notes
cat > "DEVELOPMENT_NOTES.md" << 'EOF'
# PlasmaDragon Development Notes

## Quick Commands

### Build WebGL
```
Unity -> File -> Build Settings -> Build
Output: Build/
```

### Run Local Server
```
python -m http.server 8000
Navigate to: http://localhost:8000/Build/
```

### Test MCP Commands
```
"Create terrain 100x100 with grass"
"Add directional light named Sun"
"Create player spawn point at 0,1,0"
```

## Common Issues & Solutions

### WebGL Build Too Large
- Enable compression in Player Settings
- Use texture compression
- Strip unused code
- Check Resources folder size

### AI API Rate Limiting
- Implement caching layer
- Batch requests
- Use local fallback
- Monitor usage in dashboard

### Performance Issues
- Profile with Unity Profiler
- Check draw calls (target < 100)
- Optimize particle systems
- Use object pooling

## Development Workflow
1. Create task documentation
2. Implement feature
3. Test in editor
4. Build WebGL test
5. Update task as completed
EOF

echo "  ✓ Created development notes"

echo ""
echo "🎉 Setup Complete!"
echo ""
echo "Next steps:"
echo "1. Open Unity Hub"
echo "2. Click 'Add' and select this folder"
echo "3. Open with Unity 2022.3 LTS"
echo "4. Switch platform to WebGL"
echo "5. Install required packages"
echo ""
echo "Happy developing! 🚀"