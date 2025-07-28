# Unity Troubleshooting Toolkit Guide

## 🚨 **THE BIG LESSON LEARNED**

**When ALL materials appear pink/magenta in Unity:**
1. ✅ **Check URP Pipeline Asset first** (Graphics Settings)
2. ✅ **Check URP Renderer Data assignment**  
3. ⚠️ **Only THEN** worry about individual materials/textures

**This single issue can waste HOURS if not diagnosed correctly!**

## 🛠️ **Quick Start**

### **Access the Toolkit:**
- Unity Menu: `Tools → Unity Troubleshooting Toolkit`

### **Most Common Fixes:**
1. **🔧 FIX URP PIPELINE** - Solves pink/magenta materials
2. **🎮 FIX URP RENDERER** - Solves black screen in Play mode
3. **⬆️ Upgrade Materials** - Converts Standard → URP shaders

## 🎯 **Customization for New Projects**

Edit these constants in `UnityTroubleshootingToolkit.cs`:

```csharp
private const string PROJECT_NAME = "YourProjectName";
private const string MAIN_OBJECT_NAME = "YourMainObject"; 
private const string PARENT_OBJECT_NAME = "YourParentObject";
```

## 📊 **Diagnostic Sequence**

**When things go wrong:**
1. **🔍 Check URP Setup** - Diagnose URP configuration
2. **🎯 Check Project Status** - Verify objects exist
3. **Apply specific fixes** based on results

## 🚨 **Emergency Fixes**

### **Everything Pink/Magenta:**
```
Tools → Unity Troubleshooting Toolkit
→ "🔧 FIX URP PIPELINE - SOLVE PINK MATERIALS"
```

### **Black Screen in Play Mode:**
```
Tools → Unity Troubleshooting Toolkit  
→ "🎮 FIX URP RENDERER - SOLVE BLACK SCREEN"
```

### **Scene View Gray/Empty:**
```
Tools → Unity Troubleshooting Toolkit
→ "👁️ Fix Scene Visibility"
```

## 🎮 **Save This Toolkit**

**This toolkit was created after hours of debugging fundamental URP issues. Save it in every Unity project to avoid repeating the same troubleshooting process.**

**Remember: Check the render pipeline FIRST, not last!** 🎯 