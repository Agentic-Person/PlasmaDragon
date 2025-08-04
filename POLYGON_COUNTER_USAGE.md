# 🔺 Polygon Counter Usage Guide

Perfect for monitoring file size and scene complexity!

## 🎯 **What It Does**
- **Real-time triangle count** displayed in Scene View
- **Color-coded performance warnings** (🟢🟡🔴)
- **Perfect for file size control** during development

## 🚀 **How to Use**

### Scene View Overlay (Recommended)
1. **Automatic**: Opens automatically in Scene View
2. **Toggle**: Click "Overlays" button in Scene View → Find "Polygon Counter"
3. **Always Visible**: Shows live count while you work

### Menu Access
- **Tools → Dragon Toolkit → Polygon Counter → Count Current Scene** - One-time detailed count
- **Tools → Dragon Toolkit → Polygon Counter → Toggle Scene Overlay** - Show/hide overlay

## 📊 **Performance Indicators**

| Triangles | Indicator | Meaning | Platforms |
|-----------|-----------|---------|-----------|
| < 25K | 🟢 Excellent | Perfect performance | All platforms |
| 25K-50K | 🟡 Good | Great for most platforms | Desktop, Mobile |
| 50K-100K | 🟠 Moderate | Consider optimization | Desktop |
| 100K-200K | 🔴 High | Optimization needed | Desktop only |
| > 200K | 🟣 Very High | Immediate optimization required | High-end only |

## 🌿 **Perfect Workflow with DragonToolkit**

1. **Check Current Count**: See triangle count in Scene View
2. **Use Vegetation Optimizer**: Remove grass/plants with sliders
3. **Watch Count Drop**: Real-time feedback as you optimize
4. **Target Your Platform**: 
   - **WebGL**: Keep under 100K triangles
   - **Mobile**: Keep under 50K triangles
   - **Desktop**: 50K-100K is ideal

## 🔧 **Controls**

### Scene View Overlay
- **Show/Hide Triangles**: Toggle triangle count display
- **Show/Hide Vertices**: Toggle vertex count display  
- **🔄 Refresh Count**: Force immediate update

### Auto-Update
- **Updates every 0.5 seconds** automatically
- **No performance impact** during normal work
- **Real-time feedback** when deleting objects

## 💡 **Pro Tips**

### For File Size Optimization
1. **Start with high triangle count** scene
2. **Use DragonToolkit Vegetation Optimizer** to remove % of vegetation
3. **Watch Polygon Counter** for real-time feedback
4. **Target platform thresholds** for optimal performance

### For Scene Prefab Creation
1. **Monitor count** before creating prefab
2. **Use Vegetation Optimizer** to reduce complexity
3. **Create optimized prefab** for master scene import
4. **Verify final count** meets requirements

## 🎯 **Requirements**
- **Unity 2022.3 LTS or newer**
- **UnityEditor.Overlays** (included in Unity 2022.2+)

---

## ✨ **Result**
Perfect real-time control over your scene complexity and file sizes! 🐉🌿🔺
