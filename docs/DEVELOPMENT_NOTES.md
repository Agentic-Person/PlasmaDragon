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
