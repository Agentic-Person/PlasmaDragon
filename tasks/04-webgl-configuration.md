# Task 04: WebGL Build Configuration

## Status: Pending

## Priority: HIGH - Critical for deployment

## Description
Configure Unity build settings for optimal WebGL performance. This ensures the game runs smoothly in web browsers while maintaining small download sizes and fast load times.

**Time Estimate**: 1.5 hours (streamlined for portfolio demo)
**Focus**: Get a working WebGL build quickly, optimize later

## Prerequisites
- Task 03 (Package Installation) completed
- WebGL module installed in Unity
- Basic understanding of build settings

## Step-by-Step Instructions

### 1. Switch to WebGL Platform
1. File > Build Settings
2. Select "WebGL" platform
3. Click "Switch Platform" (may take several minutes)
4. Wait for Unity to reimport assets

### 2. Player Settings Configuration (Essential Only)
Edit > Project Settings > Player:

#### WebGL Settings Tab
**Resolution and Presentation:**
- Default Canvas Width: 1920
- Default Canvas Height: 1080
- WebGL Template: Use custom PlasmaDragon template (from Task 02)
- Run In Background: ✓ Enabled

**Other Settings:**
- Color Space: Gamma (better WebGL performance)
- Auto Graphics API: ✓ Enabled
- Rendering Path: Forward
- Configuration:
  - Scripting Backend: IL2CPP
  - Api Compatibility Level: .NET Standard 2.1
  - Target Architecture: WebAssembly

**Publishing Settings:**
- Compression Format: Gzip
- Name Files As Hashes: ✓ Enabled
- Data Caching: ✓ Enabled
- Debug Symbols: ✗ Disabled
- Linker Target: Wasm
- Exception Support: None (smallest build)
- Memory Size: 256 MB (sufficient for demo)

### 3. Quality Settings Optimization
Edit > Project Settings > Quality:

1. Delete all quality levels except:
   - Low (for mobile)
   - Medium (default)
   - High (desktop)

2. Configure Medium settings:
   - Texture Quality: Half Res
   - Anisotropic Textures: Per Texture
   - Anti Aliasing: 2x Multi Sampling
   - Soft Particles: ✗ Disabled
   - Realtime Reflection Probes: ✗ Disabled
   - Shadows: Hard Shadows Only
   - Shadow Resolution: Medium
   - Shadow Distance: 50
   - Shadowmask Mode: Shadowmask
   - Pixel Light Count: 2
   - V Sync Count: Every V Blank

### 4. Graphics Settings
Edit > Project Settings > Graphics:

**Built-in Render Pipeline Settings:**
- Tier Settings: Use defaults
- Shader Stripping:
  - Lightmap Modes: Manual
  - Fog Modes: Manual
  - Instancing Variants: Strip Unused

**Shader Preloading:**
- Preloaded Shaders: Size 0 (add as needed)

### 5. Audio Settings for Web (Quick Setup)
Edit > Project Settings > Audio:

- Sample Rate: 44100 Hz (standard quality)
- DSP Buffer Size: Default
- Max Real Voices: 16

**Note**: We'll optimize audio compression per-file as needed

### 6. Skip Build Automation (Save Time)
**Portfolio Demo Note**: For faster iteration, we'll handle optimizations manually as needed rather than creating automated build scripts. This saves development time.
```csharp
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class WebGLBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("=== WebGL Build Pre-processing ===");
        
        // Set optimal texture settings
        SetTextureImportSettings();
        
        // Configure audio compression
        SetAudioImportSettings();
        
        PlayerSettings.WebGL.memorySize = 256;
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
        
        Debug.Log("Pre-processing complete!");
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        Debug.Log("=== WebGL Build Post-processing ===");
        Debug.Log($"Build size: {report.summary.totalSize / 1048576}MB");
        Debug.Log($"Build time: {report.summary.totalTime.TotalSeconds}s");
    }

    void SetTextureImportSettings()
    {
        // Auto-configure texture compression
        string[] texturePaths = AssetDatabase.FindAssets("t:Texture2D");
        foreach (string guid in texturePaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (importer != null && path.Contains("_Project"))
            {
                importer.maxTextureSize = 1024;
                importer.textureCompression = TextureImporterCompression.Compressed;
                importer.SaveAndReimport();
            }
        }
    }

    void SetAudioImportSettings()
    {
        // Configure audio for web
        string[] audioPaths = AssetDatabase.FindAssets("t:AudioClip");
        foreach (string guid in audioPaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
            
            if (importer != null)
            {
                AudioImporterSampleSettings settings = importer.defaultSampleSettings;
                settings.loadType = AudioClipLoadType.CompressedInMemory;
                settings.compressionFormat = AudioCompressionFormat.Vorbis;
                settings.quality = 0.7f;
                importer.defaultSampleSettings = settings;
                importer.SaveAndReimport();
            }
        }
    }
}
```

### 7. Create Test Build
1. File > Build Settings
2. Click "Build"
3. Create folder "Builds/WebGL/"
4. Name: "TestBuild_v0.1"
5. Monitor build process
6. Check final size

### 8. Create Local Web Server Test
Create `test-server.py` in Builds folder:
```python
#!/usr/bin/env python3
import http.server
import socketserver
import os

PORT = 8000
os.chdir('WebGL/TestBuild_v0.1')

Handler = http.server.SimpleHTTPRequestHandler
Handler.extensions_map.update({
    '.wasm': 'application/wasm',
})

with socketserver.TCPServer(("", PORT), Handler) as httpd:
    print(f"Server running at http://localhost:{PORT}/")
    httpd.serve_forever()
```

Test build:
```bash
cd Builds
python3 test-server.py
# Open http://localhost:8000 in browser
```

### 9. Portfolio Demo Optimization (Essentials Only)
- [ ] Compress large textures (>1024px)
- [ ] Remove unused packages
- [ ] Strip unused shaders
- [ ] Test in Chrome/Firefox

## Expected Outcomes
- ✅ WebGL platform selected
- ✅ Build settings optimized
- ✅ First test build < 30MB
- ✅ Build runs in browser
- ✅ 60 FPS on desktop browsers
- ✅ No console errors in browser

## Performance Targets (Portfolio Demo)
- Build Size: < 50MB compressed (acceptable for portfolio)
- Load Time: < 20 seconds on broadband
- Frame Rate: 60 FPS desktop (priority)
- Mobile: Nice to have, not required

## Verification Checklist
- [ ] Build completes without errors
- [ ] Final size under 50MB
- [ ] Loads in Chrome/Firefox
- [ ] No WebGL errors in console
- [ ] Audio plays correctly
- [ ] Input responds properly
- [ ] Performance acceptable

## Common Issues & Solutions

### Issue: Build Too Large
- Check texture import settings
- Remove unused packages
- Use texture atlasing
- Compress audio more

### Issue: Build Fails
- Check console for errors
- Verify WebGL module installed
- Clear Library folder
- Reduce memory size

### Issue: Poor Performance
- Reduce quality settings
- Optimize shaders
- Reduce polygon count
- Profile in browser

### Issue: Audio Not Playing
- Check browser autoplay policies
- Add user interaction first
- Verify audio format support

## Browser Compatibility Notes
- Chrome: Best performance
- Firefox: Good support
- Safari: May need workarounds
- Edge: Chrome-equivalent
- Mobile: Reduced features

## Portfolio Strategy
- Get a working build first
- Optimize only if >50MB
- Focus on smooth gameplay over perfect compression
- Test on recruiters' likely browsers (Chrome/Firefox)

## Next Steps
Proceed to Task 05: Environment Import

---

## Completion Notes
*To be filled after task completion*

**Date Completed**: 
**Time Taken**: 
**Build Size Achieved**: 
**Load Time**: 
**Performance Metrics**: 
**Browser Compatibility**: 