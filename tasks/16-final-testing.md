# Task 16: Final Testing and Deployment

## Status: Pending

## Priority: HIGH - Final step before release

## Description
Conduct comprehensive testing across browsers and devices, fix critical bugs, and deploy the game to production. This ensures a polished, professional demo ready for portfolio presentation.

## Prerequisites
- All previous tasks completed
- Performance optimization done
- WebGL build working
- Landing page ready

## Step-by-Step Instructions

### 1. Create Testing Framework
`Assets/_Project/Scripts/Testing/GameTestRunner.cs`:
```csharp
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameTestRunner : MonoBehaviour
{
    [Header("Test Configuration")]
    public bool runAutomatedTests = true;
    public bool generateReport = true;
    
    [System.Serializable]
    public class TestCase
    {
        public string name;
        public string description;
        public bool passed;
        public string errorMessage;
        public float executionTime;
    }
    
    private List<TestCase> testResults = new List<TestCase>();
    private float testStartTime;
    
    void Start()
    {
        if (runAutomatedTests)
        {
            StartCoroutine(RunAllTests());
        }
    }
    
    IEnumerator RunAllTests()
    {
        Debug.Log("=== Starting Automated Tests ===");
        testStartTime = Time.time;
        
        // Core Systems
        yield return StartCoroutine(TestFlightControls());
        yield return StartCoroutine(TestWeaponSystems());
        yield return StartCoroutine(TestEnemyAI());
        yield return StartCoroutine(TestBossAI());
        
        // Web3 Integration
        yield return StartCoroutine(TestWalletCreation());
        yield return StartCoroutine(TestTokenRewards());
        
        // Performance
        yield return StartCoroutine(TestMemoryLeaks());
        yield return StartCoroutine(TestLoadTimes());
        
        // UI/UX
        yield return StartCoroutine(TestUIResponsiveness());
        yield return StartCoroutine(TestMobileTouch());
        
        if (generateReport)
        {
            GenerateTestReport();
        }
    }
    
    IEnumerator TestFlightControls()
    {
        var test = new TestCase
        {
            name = "Flight Controls",
            description = "Test dragon flight responsiveness and controls"
        };
        
        float startTime = Time.time;
        
        try
        {
            // Get dragon controller
            var dragon = GameObject.FindGameObjectWithTag("Player");
            Assert.IsNotNull(dragon, "Dragon not found");
            
            var controller = dragon.GetComponent<DragonFlightController>();
            Assert.IsNotNull(controller, "Flight controller not found");
            
            // Test movement
            Vector3 startPos = dragon.transform.position;
            
            // Simulate input
            controller.SimulateInput(Vector2.up, true); // Forward + boost
            yield return new WaitForSeconds(1f);
            
            Vector3 endPos = dragon.transform.position;
            float distance = Vector3.Distance(startPos, endPos);
            
            Assert.IsTrue(distance > 10f, "Dragon didn't move enough");
            Assert.IsTrue(controller.GetCurrentSpeed() > 20f, "Speed too low");
            
            test.passed = true;
        }
        catch (System.Exception e)
        {
            test.passed = false;
            test.errorMessage = e.Message;
        }
        
        test.executionTime = Time.time - startTime;
        testResults.Add(test);
    }
    
    IEnumerator TestWeaponSystems()
    {
        var test = new TestCase
        {
            name = "Weapon Systems",
            description = "Test plasma orbs and fireball functionality"
        };
        
        float startTime = Time.time;
        
        try
        {
            var dragon = GameObject.FindGameObjectWithTag("Player");
            var weaponController = dragon.GetComponent<DragonWeaponController>();
            Assert.IsNotNull(weaponController, "Weapon controller not found");
            
            // Test plasma orb
            int projectilesBefore = FindObjectsOfType<PlasmaOrb>().Length;
            weaponController.TestFirePlasmaOrb();
            yield return new WaitForSeconds(0.1f);
            
            int projectilesAfter = FindObjectsOfType<PlasmaOrb>().Length;
            Assert.IsTrue(projectilesAfter > projectilesBefore, "Plasma orb not created");
            
            // Test rapid fire
            weaponController.TestStartRapidFire();
            yield return new WaitForSeconds(1f);
            weaponController.TestStopRapidFire();
            
            int fireballCount = FindObjectsOfType<Fireball>().Length;
            Assert.IsTrue(fireballCount >= 4, "Not enough fireballs created");
            
            test.passed = true;
        }
        catch (System.Exception e)
        {
            test.passed = false;
            test.errorMessage = e.Message;
        }
        
        test.executionTime = Time.time - startTime;
        testResults.Add(test);
    }
    
    IEnumerator TestEnemyAI()
    {
        var test = new TestCase
        {
            name = "Enemy AI",
            description = "Test enemy detection and combat behavior"
        };
        
        float startTime = Time.time;
        
        try
        {
            // Spawn test enemy
            GameObject enemyPrefab = Resources.Load<GameObject>("Prefabs/ArcherTower");
            GameObject enemy = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
            
            var archer = enemy.GetComponent<ArcherTower>();
            Assert.IsNotNull(archer, "Archer component not found");
            
            // Place player in range
            var player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = Vector3.forward * 20f;
            
            // Wait for detection
            yield return new WaitForSeconds(1f);
            
            // Check if enemy is targeting player
            Assert.IsTrue(archer.HasTarget(), "Enemy not targeting player");
            
            // Cleanup
            Destroy(enemy);
            
            test.passed = true;
        }
        catch (System.Exception e)
        {
            test.passed = false;
            test.errorMessage = e.Message;
        }
        
        test.executionTime = Time.time - startTime;
        testResults.Add(test);
    }
    
    IEnumerator TestBossAI()
    {
        var test = new TestCase
        {
            name = "Boss AI System",
            description = "Test AI decision making and API integration"
        };
        
        float startTime = Time.time;
        
        try
        {
            // Spawn boss
            GameObject bossPrefab = Resources.Load<GameObject>("Prefabs/EliteCommander");
            GameObject boss = Instantiate(bossPrefab, Vector3.zero, Quaternion.identity);
            
            var commander = boss.GetComponent<EliteCommander>();
            Assert.IsNotNull(commander, "Commander component not found");
            
            // Test AI decision system
            var aiSystem = commander.GetComponent<AIDecisionSystem>();
            Assert.IsNotNull(aiSystem, "AI system not found");
            
            // Create test context
            var context = new BossContext
            {
                bossHealth = 50f,
                bossMaxHealth = 100f,
                playerPosition = Vector3.forward * 30f,
                distanceToPlayer = 30f,
                availableActions = new[] { "ranged_barrage", "defensive_shield" }
            };
            
            // Get decision (uses fallback in testing)
            var decision = aiSystem.GetFallbackDecision(context);
            Assert.IsNotNull(decision, "No decision returned");
            Assert.IsNotEmpty(decision.action, "Empty action");
            
            // Cleanup
            Destroy(boss);
            
            test.passed = true;
        }
        catch (System.Exception e)
        {
            test.passed = false;
            test.errorMessage = e.Message;
        }
        
        test.executionTime = Time.time - startTime;
        testResults.Add(test);
    }
    
    IEnumerator TestWalletCreation()
    {
        var test = new TestCase
        {
            name = "Wallet Creation",
            description = "Test automatic wallet generation"
        };
        
        float startTime = Time.time;
        
        try
        {
            var solanaManager = SolanaManager.Instance;
            Assert.IsNotNull(solanaManager, "Solana manager not found");
            
            // Initialize if not already
            if (!solanaManager.IsInitialized)
            {
                yield return solanaManager.Initialize();
            }
            
            Assert.IsTrue(solanaManager.IsInitialized, "Wallet initialization failed");
            Assert.IsNotEmpty(solanaManager.PlayerWalletAddress, "Wallet address empty");
            
            test.passed = true;
        }
        catch (System.Exception e)
        {
            test.passed = false;
            test.errorMessage = e.Message;
        }
        
        test.executionTime = Time.time - startTime;
        testResults.Add(test);
    }
    
    IEnumerator TestTokenRewards()
    {
        var test = new TestCase
        {
            name = "Token Rewards",
            description = "Test token distribution system"
        };
        
        float startTime = Time.time;
        
        try
        {
            var rewardSystem = TokenRewardSystem.Instance;
            Assert.IsNotNull(rewardSystem, "Reward system not found");
            
            // Get initial balance
            int initialBalance = rewardSystem.GetLocalBalance();
            
            // Award test tokens
            rewardSystem.RewardTokens(5, "Test reward");
            
            // Wait for processing
            yield return new WaitForSeconds(0.5f);
            
            // Check balance updated
            int newBalance = rewardSystem.GetLocalBalance();
            Assert.AreEqual(initialBalance + 5, newBalance, "Balance not updated correctly");
            
            test.passed = true;
        }
        catch (System.Exception e)
        {
            test.passed = false;
            test.errorMessage = e.Message;
        }
        
        test.executionTime = Time.time - startTime;
        testResults.Add(test);
    }
    
    IEnumerator TestMemoryLeaks()
    {
        var test = new TestCase
        {
            name = "Memory Leaks",
            description = "Test for memory leaks during gameplay"
        };
        
        float startTime = Time.time;
        
        try
        {
            // Record initial memory
            System.GC.Collect();
            long initialMemory = System.GC.GetTotalMemory(false);
            
            // Spawn and destroy many objects
            for (int i = 0; i < 100; i++)
            {
                GameObject projectile = UniversalObjectPool.Instance.Spawn(
                    "PlasmaOrb", Vector3.zero, Quaternion.identity);
                
                yield return null;
                
                UniversalObjectPool.Instance.Return(projectile);
            }
            
            // Force collection
            System.GC.Collect();
            yield return new WaitForSeconds(1f);
            
            long finalMemory = System.GC.GetTotalMemory(false);
            long memoryGrowth = finalMemory - initialMemory;
            
            // Allow some growth but not excessive
            Assert.IsTrue(memoryGrowth < 10485760, // 10MB
                $"Excessive memory growth: {memoryGrowth / 1048576}MB");
            
            test.passed = true;
        }
        catch (System.Exception e)
        {
            test.passed = false;
            test.errorMessage = e.Message;
        }
        
        test.executionTime = Time.time - startTime;
        testResults.Add(test);
    }
    
    IEnumerator TestLoadTimes()
    {
        var test = new TestCase
        {
            name = "Load Times",
            description = "Test scene loading performance"
        };
        
        float startTime = Time.time;
        
        try
        {
            // Measure scene load time
            float loadStart = Time.realtimeSinceStartup;
            
            var loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
                "Level01_FortressIsland");
            
            while (!loadOperation.isDone)
            {
                yield return null;
            }
            
            float loadTime = Time.realtimeSinceStartup - loadStart;
            
            Assert.IsTrue(loadTime < 20f, $"Load time too long: {loadTime}s");
            
            test.passed = true;
        }
        catch (System.Exception e)
        {
            test.passed = false;
            test.errorMessage = e.Message;
        }
        
        test.executionTime = Time.time - startTime;
        testResults.Add(test);
    }
    
    IEnumerator TestUIResponsiveness()
    {
        var test = new TestCase
        {
            name = "UI Responsiveness",
            description = "Test UI interaction and feedback"
        };
        
        float startTime = Time.time;
        
        try
        {
            // Find UI elements
            var canvas = GameObject.Find("UICanvas");
            Assert.IsNotNull(canvas, "UI Canvas not found");
            
            var buttons = canvas.GetComponentsInChildren<UnityEngine.UI.Button>();
            Assert.IsTrue(buttons.Length > 0, "No buttons found");
            
            // Test button interaction
            foreach (var button in buttons)
            {
                Assert.IsTrue(button.interactable, $"Button {button.name} not interactable");
            }
            
            test.passed = true;
        }
        catch (System.Exception e)
        {
            test.passed = false;
            test.errorMessage = e.Message;
        }
        
        test.executionTime = Time.time - startTime;
        testResults.Add(test);
    }
    
    IEnumerator TestMobileTouch()
    {
        var test = new TestCase
        {
            name = "Mobile Touch Controls",
            description = "Test touch input handling"
        };
        
        float startTime = Time.time;
        
        try
        {
            // Simulate touch input
            var touchHandler = FindObjectOfType<MobileTouchHandler>();
            
            if (touchHandler != null)
            {
                // Simulate touch
                touchHandler.SimulateTouch(new Vector2(Screen.width / 2, Screen.height / 2));
                yield return new WaitForSeconds(0.1f);
                
                Assert.IsTrue(touchHandler.IsTouchActive(), "Touch not registered");
            }
            else
            {
                // Skip on non-mobile
                test.description += " (Skipped - not mobile)";
            }
            
            test.passed = true;
        }
        catch (System.Exception e)
        {
            test.passed = false;
            test.errorMessage = e.Message;
        }
        
        test.executionTime = Time.time - startTime;
        testResults.Add(test);
    }
    
    void GenerateTestReport()
    {
        int passedTests = 0;
        int failedTests = 0;
        
        StringBuilder report = new StringBuilder();
        report.AppendLine("=== Automated Test Report ===");
        report.AppendLine($"Date: {System.DateTime.Now}");
        report.AppendLine($"Total Tests: {testResults.Count}");
        report.AppendLine();
        
        foreach (var test in testResults)
        {
            if (test.passed)
            {
                passedTests++;
                report.AppendLine($"✓ {test.name} - PASSED ({test.executionTime:F2}s)");
            }
            else
            {
                failedTests++;
                report.AppendLine($"✗ {test.name} - FAILED");
                report.AppendLine($"  Error: {test.errorMessage}");
            }
        }
        
        report.AppendLine();
        report.AppendLine($"Summary: {passedTests} passed, {failedTests} failed");
        report.AppendLine($"Success Rate: {(passedTests * 100f / testResults.Count):F1}%");
        report.AppendLine($"Total Time: {Time.time - testStartTime:F2}s");
        
        Debug.Log(report.ToString());
        
        // Save to file
        string path = Application.dataPath + "/TestReport.txt";
        System.IO.File.WriteAllText(path, report.ToString());
    }
}

// Helper assertion class
public static class Assert
{
    public static void IsTrue(bool condition, string message)
    {
        if (!condition)
            throw new System.Exception($"Assertion failed: {message}");
    }
    
    public static void IsFalse(bool condition, string message)
    {
        IsTrue(!condition, message);
    }
    
    public static void IsNotNull(object obj, string message)
    {
        IsTrue(obj != null, message);
    }
    
    public static void IsNull(object obj, string message)
    {
        IsTrue(obj == null, message);
    }
    
    public static void AreEqual(object expected, object actual, string message)
    {
        IsTrue(expected.Equals(actual), 
            $"{message} - Expected: {expected}, Actual: {actual}");
    }
    
    public static void IsNotEmpty(string str, string message)
    {
        IsTrue(!string.IsNullOrEmpty(str), message);
    }
}
```

### 2. Create Browser Compatibility Tester
`WebBuild/js/compatibility-test.js`:
```javascript
class CompatibilityTester {
    constructor() {
        this.results = {
            browser: this.detectBrowser(),
            features: {},
            performance: {},
            errors: []
        };
    }
    
    detectBrowser() {
        const userAgent = navigator.userAgent;
        let browserName = 'Unknown';
        let version = '';
        
        if (userAgent.indexOf('Chrome') > -1) {
            browserName = 'Chrome';
            version = userAgent.match(/Chrome\/(\d+)/)[1];
        } else if (userAgent.indexOf('Firefox') > -1) {
            browserName = 'Firefox';
            version = userAgent.match(/Firefox\/(\d+)/)[1];
        } else if (userAgent.indexOf('Safari') > -1) {
            browserName = 'Safari';
            version = userAgent.match(/Version\/(\d+)/)[1];
        } else if (userAgent.indexOf('Edge') > -1) {
            browserName = 'Edge';
            version = userAgent.match(/Edge\/(\d+)/)[1];
        }
        
        return {
            name: browserName,
            version: version,
            userAgent: userAgent,
            isMobile: /Mobi|Android/i.test(userAgent)
        };
    }
    
    async runAllTests() {
        console.log('Starting compatibility tests...');
        
        // Feature tests
        this.testWebGLSupport();
        this.testWebAssembly();
        this.testIndexedDB();
        this.testWebAudio();
        this.testGamepadAPI();
        
        // Performance tests
        await this.testRenderingPerformance();
        await this.testMemoryUsage();
        await this.testNetworkSpeed();
        
        // Generate report
        this.generateReport();
        
        return this.results;
    }
    
    testWebGLSupport() {
        try {
            const canvas = document.createElement('canvas');
            const gl = canvas.getContext('webgl2') || canvas.getContext('webgl');
            
            if (gl) {
                this.results.features.webgl = {
                    supported: true,
                    version: gl.getParameter(gl.VERSION),
                    vendor: gl.getParameter(gl.VENDOR),
                    renderer: gl.getParameter(gl.RENDERER),
                    maxTextureSize: gl.getParameter(gl.MAX_TEXTURE_SIZE),
                    maxVertexAttributes: gl.getParameter(gl.MAX_VERTEX_ATTRIBS)
                };
            } else {
                throw new Error('WebGL not supported');
            }
        } catch (e) {
            this.results.features.webgl = {
                supported: false,
                error: e.message
            };
            this.results.errors.push('WebGL not supported');
        }
    }
    
    testWebAssembly() {
        this.results.features.wasm = {
            supported: typeof WebAssembly !== 'undefined',
            instantiate: typeof WebAssembly.instantiate === 'function',
            memory: typeof WebAssembly.Memory === 'function'
        };
        
        if (!this.results.features.wasm.supported) {
            this.results.errors.push('WebAssembly not supported');
        }
    }
    
    testIndexedDB() {
        this.results.features.indexedDB = {
            supported: 'indexedDB' in window,
            persistent: navigator.storage && navigator.storage.persist
        };
    }
    
    testWebAudio() {
        this.results.features.webAudio = {
            supported: 'AudioContext' in window || 'webkitAudioContext' in window,
            sampleRate: null
        };
        
        if (this.results.features.webAudio.supported) {
            const AudioContext = window.AudioContext || window.webkitAudioContext;
            const ctx = new AudioContext();
            this.results.features.webAudio.sampleRate = ctx.sampleRate;
            ctx.close();
        }
    }
    
    testGamepadAPI() {
        this.results.features.gamepad = {
            supported: 'getGamepads' in navigator
        };
    }
    
    async testRenderingPerformance() {
        const canvas = document.createElement('canvas');
        canvas.width = 1920;
        canvas.height = 1080;
        const gl = canvas.getContext('webgl');
        
        if (!gl) return;
        
        // Simple render test
        const startTime = performance.now();
        const frames = 60;
        
        for (let i = 0; i < frames; i++) {
            gl.clearColor(Math.random(), Math.random(), Math.random(), 1.0);
            gl.clear(gl.COLOR_BUFFER_BIT);
            // Force render
            gl.finish();
        }
        
        const endTime = performance.now();
        const totalTime = endTime - startTime;
        const avgFrameTime = totalTime / frames;
        const fps = 1000 / avgFrameTime;
        
        this.results.performance.rendering = {
            averageFrameTime: avgFrameTime.toFixed(2) + 'ms',
            estimatedFPS: fps.toFixed(1),
            suitable: fps >= 30
        };
    }
    
    async testMemoryUsage() {
        if (performance.memory) {
            this.results.performance.memory = {
                available: true,
                jsHeapSizeLimit: (performance.memory.jsHeapSizeLimit / 1048576).toFixed(2) + 'MB',
                totalJSHeapSize: (performance.memory.totalJSHeapSize / 1048576).toFixed(2) + 'MB',
                usedJSHeapSize: (performance.memory.usedJSHeapSize / 1048576).toFixed(2) + 'MB'
            };
        } else {
            this.results.performance.memory = {
                available: false,
                note: 'Memory API not available in this browser'
            };
        }
    }
    
    async testNetworkSpeed() {
        const testUrl = 'Build/WebGL.data'; // Use actual build file
        const startTime = performance.now();
        
        try {
            const response = await fetch(testUrl, {
                method: 'HEAD',
                cache: 'no-cache'
            });
            
            const endTime = performance.now();
            const duration = endTime - startTime;
            
            this.results.performance.network = {
                latency: duration.toFixed(2) + 'ms',
                suitable: duration < 1000
            };
        } catch (e) {
            this.results.performance.network = {
                error: 'Could not test network speed'
            };
        }
    }
    
    generateReport() {
        const report = document.createElement('div');
        report.id = 'compatibility-report';
        report.style.cssText = `
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background: white;
            color: black;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
            max-width: 600px;
            max-height: 80vh;
            overflow-y: auto;
            z-index: 10000;
        `;
        
        let html = '<h2>Browser Compatibility Report</h2>';
        
        // Browser info
        html += '<h3>Browser Information</h3>';
        html += `<p>Browser: ${this.results.browser.name} ${this.results.browser.version}</p>`;
        html += `<p>Mobile: ${this.results.browser.isMobile ? 'Yes' : 'No'}</p>`;
        
        // Feature support
        html += '<h3>Feature Support</h3>';
        html += '<ul>';
        
        const features = [
            { name: 'WebGL', result: this.results.features.webgl?.supported },
            { name: 'WebAssembly', result: this.results.features.wasm?.supported },
            { name: 'IndexedDB', result: this.results.features.indexedDB?.supported },
            { name: 'Web Audio', result: this.results.features.webAudio?.supported },
            { name: 'Gamepad API', result: this.results.features.gamepad?.supported }
        ];
        
        features.forEach(feature => {
            const icon = feature.result ? '✅' : '❌';
            html += `<li>${icon} ${feature.name}</li>`;
        });
        
        html += '</ul>';
        
        // Performance
        html += '<h3>Performance</h3>';
        if (this.results.performance.rendering) {
            html += `<p>Estimated FPS: ${this.results.performance.rendering.estimatedFPS}</p>`;
            html += `<p>Frame Time: ${this.results.performance.rendering.averageFrameTime}</p>`;
        }
        
        // Compatibility status
        const compatible = this.results.errors.length === 0 && 
                         this.results.features.webgl?.supported && 
                         this.results.features.wasm?.supported;
        
        html += '<h3>Compatibility Status</h3>';
        if (compatible) {
            html += '<p style="color: green; font-weight: bold;">✅ Your browser is fully compatible!</p>';
        } else {
            html += '<p style="color: red; font-weight: bold;">❌ Compatibility issues detected:</p>';
            html += '<ul>';
            this.results.errors.forEach(error => {
                html += `<li>${error}</li>`;
            });
            html += '</ul>';
        }
        
        html += '<button onclick="document.getElementById(\'compatibility-report\').remove()">Close</button>';
        
        report.innerHTML = html;
        document.body.appendChild(report);
        
        // Also log to console
        console.log('Compatibility Test Results:', this.results);
    }
}

// Auto-run on page load
window.addEventListener('load', () => {
    const tester = new CompatibilityTester();
    tester.runAllTests();
});
```

### 3. Create Deployment Checklist Script
`deployment-checklist.sh`:
```bash
#!/bin/bash

echo "=== Plasma Thief: Sky Rogue - Deployment Checklist ==="
echo "Date: $(date)"
echo ""

# Color codes
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Checklist items
declare -A checklist=(
    ["Unity Build Complete"]="Check if WebGL build exists"
    ["Build Size < 50MB"]="Verify compressed build size"
    ["Landing Page Ready"]="Check index.html exists"
    ["SSL Certificate"]="Verify HTTPS configuration"
    ["Environment Variables"]="Check API keys are set"
    ["Error Tracking"]="Verify error logging setup"
    ["Analytics"]="Confirm analytics integration"
    ["Browser Testing"]="Test on Chrome, Firefox, Safari, Edge"
    ["Mobile Testing"]="Test on iOS and Android"
    ["Performance Metrics"]="Verify 60 FPS on desktop"
)

echo "### Pre-Deployment Checklist ###"
echo ""

# Function to check item
check_item() {
    local item=$1
    local description=$2
    
    echo -n "[ ] $item"
    echo " - $description"
    read -p "    Completed? (y/n): " -n 1 -r
    echo
    
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        echo -e "    ${GREEN}✓ Checked${NC}"
        return 0
    else
        echo -e "    ${RED}✗ Not completed${NC}"
        return 1
    fi
    echo
}

# Run through checklist
completed=0
total=${#checklist[@]}

for item in "${!checklist[@]}"; do
    if check_item "$item" "${checklist[$item]}"; then
        ((completed++))
    fi
done

echo ""
echo "### Summary ###"
echo "Completed: $completed/$total"

if [ $completed -eq $total ]; then
    echo -e "${GREEN}All checks passed! Ready for deployment.${NC}"
else
    echo -e "${YELLOW}Warning: Some checks failed. Review before deploying.${NC}"
fi

# Build verification
echo ""
echo "### Build Verification ###"

# Check build directory
if [ -d "WebBuild/Build" ]; then
    echo -e "${GREEN}✓ Build directory found${NC}"
    
    # Check file sizes
    echo "Build files:"
    ls -lh WebBuild/Build/
    
    # Calculate total size
    total_size=$(du -sh WebBuild/Build | cut -f1)
    echo "Total build size: $total_size"
else
    echo -e "${RED}✗ Build directory not found${NC}"
fi

# Generate deployment report
echo ""
echo "### Generating Deployment Report ###"

report_file="deployment-report-$(date +%Y%m%d-%H%M%S).txt"
cat > "$report_file" << EOL
Plasma Thief: Sky Rogue - Deployment Report
Generated: $(date)

Checklist Completion: $completed/$total

Build Information:
- Build Size: $total_size
- Target Platform: WebGL
- Unity Version: 2022.3 LTS

Deployment Configuration:
- Hosting: GitHub Pages / Netlify
- CDN: Cloudflare (optional)
- SSL: Enabled
- Compression: Gzip

Performance Targets:
- Desktop FPS: 60
- Mobile FPS: 30
- Load Time: <20 seconds
- Memory Usage: <256MB

Browser Support:
- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

Notes:
$(read -p "Enter any deployment notes: " notes && echo "$notes")

EOL

echo "Report saved to: $report_file"

# Final confirmation
echo ""
read -p "Deploy to production? (y/n): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo -e "${GREEN}Initiating deployment...${NC}"
    # Add deployment commands here
else
    echo -e "${YELLOW}Deployment cancelled.${NC}"
fi
```

### 4. Create Error Tracking System
`WebBuild/js/error-tracker.js`:
```javascript
class ErrorTracker {
    constructor() {
        this.errors = [];
        this.setupErrorHandlers();
    }
    
    setupErrorHandlers() {
        // Global error handler
        window.addEventListener('error', (event) => {
            this.logError({
                type: 'javascript',
                message: event.message,
                source: event.filename,
                line: event.lineno,
                column: event.colno,
                stack: event.error?.stack,
                timestamp: new Date().toISOString()
            });
        });
        
        // Unhandled promise rejections
        window.addEventListener('unhandledrejection', (event) => {
            this.logError({
                type: 'promise',
                message: event.reason?.message || event.reason,
                stack: event.reason?.stack,
                timestamp: new Date().toISOString()
            });
        });
        
        // Unity specific errors
        if (window.UnityLoader) {
            const originalError = console.error;
            console.error = (...args) => {
                if (args[0]?.includes('Unity')) {
                    this.logError({
                        type: 'unity',
                        message: args.join(' '),
                        timestamp: new Date().toISOString()
                    });
                }
                originalError.apply(console, args);
            };
        }
    }
    
    logError(error) {
        // Add browser info
        error.browser = navigator.userAgent;
        error.url = window.location.href;
        error.resolution = `${window.innerWidth}x${window.innerHeight}`;
        
        // Store locally
        this.errors.push(error);
        
        // Send to backend (if configured)
        this.sendToBackend(error);
        
        // Log to console in development
        if (window.location.hostname === 'localhost') {
            console.error('Error tracked:', error);
        }
    }
    
    sendToBackend(error) {
        // In production, send to your error tracking service
        if (window.location.hostname !== 'localhost') {
            fetch('/api/errors', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(error)
            }).catch(() => {
                // Silently fail - don't create more errors
            });
        }
    }
    
    getErrorReport() {
        const report = {
            totalErrors: this.errors.length,
            errorsByType: {},
            mostCommon: {},
            timeline: []
        };
        
        // Group by type
        this.errors.forEach(error => {
            if (!report.errorsByType[error.type]) {
                report.errorsByType[error.type] = 0;
            }
            report.errorsByType[error.type]++;
        });
        
        // Find most common errors
        const messageCounts = {};
        this.errors.forEach(error => {
            const key = error.message?.substring(0, 50);
            if (key) {
                messageCounts[key] = (messageCounts[key] || 0) + 1;
            }
        });
        
        report.mostCommon = Object.entries(messageCounts)
            .sort((a, b) => b[1] - a[1])
            .slice(0, 5)
            .map(([message, count]) => ({ message, count }));
        
        return report;
    }
}

// Initialize error tracking
window.errorTracker = new ErrorTracker();
```

### 5. Create Performance Monitoring
`WebBuild/js/performance-monitor.js`:
```javascript
class PerformanceMonitor {
    constructor() {
        this.metrics = {
            fps: [],
            memory: [],
            loadTimes: {},
            unityMetrics: {}
        };
        
        this.startMonitoring();
    }
    
    startMonitoring() {
        // FPS monitoring
        let lastTime = performance.now();
        let frames = 0;
        
        const measureFPS = () => {
            frames++;
            const currentTime = performance.now();
            
            if (currentTime >= lastTime + 1000) {
                const fps = Math.round((frames * 1000) / (currentTime - lastTime));
                this.metrics.fps.push({
                    value: fps,
                    timestamp: Date.now()
                });
                
                // Keep only last 60 seconds
                if (this.metrics.fps.length > 60) {
                    this.metrics.fps.shift();
                }
                
                frames = 0;
                lastTime = currentTime;
            }
            
            requestAnimationFrame(measureFPS);
        };
        
        requestAnimationFrame(measureFPS);
        
        // Memory monitoring
        if (performance.memory) {
            setInterval(() => {
                this.metrics.memory.push({
                    used: performance.memory.usedJSHeapSize,
                    total: performance.memory.totalJSHeapSize,
                    limit: performance.memory.jsHeapSizeLimit,
                    timestamp: Date.now()
                });
                
                // Keep only last 60 samples
                if (this.metrics.memory.length > 60) {
                    this.metrics.memory.shift();
                }
            }, 1000);
        }
        
        // Load time monitoring
        window.addEventListener('load', () => {
            const loadTime = performance.timing.loadEventEnd - 
                           performance.timing.navigationStart;
            
            this.metrics.loadTimes.page = loadTime;
            
            // Unity specific load time
            if (window.unityInstance) {
                this.metrics.loadTimes.unity = 
                    performance.now() - performance.timing.navigationStart;
            }
        });
    }
    
    getAverageFPS() {
        if (this.metrics.fps.length === 0) return 0;
        
        const sum = this.metrics.fps.reduce((acc, sample) => acc + sample.value, 0);
        return Math.round(sum / this.metrics.fps.length);
    }
    
    getMinFPS() {
        if (this.metrics.fps.length === 0) return 0;
        
        return Math.min(...this.metrics.fps.map(s => s.value));
    }
    
    getCurrentMemoryMB() {
        if (this.metrics.memory.length === 0) return 0;
        
        const latest = this.metrics.memory[this.metrics.memory.length - 1];
        return Math.round(latest.used / 1048576);
    }
    
    generatePerformanceReport() {
        const report = {
            fps: {
                average: this.getAverageFPS(),
                min: this.getMinFPS(),
                current: this.metrics.fps[this.metrics.fps.length - 1]?.value || 0
            },
            memory: {
                current: this.getCurrentMemoryMB() + 'MB',
                peak: Math.round(Math.max(...this.metrics.memory.map(s => s.used)) / 1048576) + 'MB'
            },
            loadTimes: this.metrics.loadTimes,
            timestamp: new Date().toISOString()
        };
        
        // Check performance targets
        report.meetsTargets = {
            fps: report.fps.average >= 30,
            memory: this.getCurrentMemoryMB() < 256,
            loadTime: (this.metrics.loadTimes.page || 0) < 20000
        };
        
        return report;
    }
    
    showPerformanceOverlay() {
        const overlay = document.createElement('div');
        overlay.id = 'performance-overlay';
        overlay.style.cssText = `
            position: fixed;
            top: 10px;
            right: 10px;
            background: rgba(0, 0, 0, 0.8);
            color: white;
            padding: 10px;
            font-family: monospace;
            font-size: 12px;
            z-index: 10000;
            border-radius: 5px;
        `;
        
        const update = () => {
            const fps = this.metrics.fps[this.metrics.fps.length - 1]?.value || 0;
            const memory = this.getCurrentMemoryMB();
            
            overlay.innerHTML = `
                FPS: ${fps}<br>
                Memory: ${memory}MB<br>
                Load: ${(this.metrics.loadTimes.page / 1000).toFixed(1)}s
            `;
        };
        
        update();
        setInterval(update, 1000);
        
        document.body.appendChild(overlay);
    }
}

// Initialize performance monitoring
window.performanceMonitor = new PerformanceMonitor();

// Show overlay in development
if (window.location.hostname === 'localhost') {
    window.performanceMonitor.showPerformanceOverlay();
}
```

### 6. Create Final QA Checklist
`qa-checklist.md`:
```markdown
# Plasma Thief: Sky Rogue - Final QA Checklist

## Pre-Launch Testing

### 🎮 Gameplay Testing
- [ ] Dragon flight controls smooth and responsive
- [ ] Plasma orbs deal correct damage (100)
- [ ] Fireballs fire at correct rate (5/second)
- [ ] All enemies spawn correctly
- [ ] Boss AI makes decisions properly
- [ ] Token rewards distributed correctly
- [ ] Level transitions work smoothly

### 🌐 Browser Compatibility
- [ ] **Chrome 90+**: Tested and working
- [ ] **Firefox 88+**: Tested and working
- [ ] **Safari 14+**: Tested and working
- [ ] **Edge 90+**: Tested and working
- [ ] **Mobile Chrome**: Tested and working
- [ ] **Mobile Safari**: Tested and working

### 📱 Device Testing
- [ ] Desktop (1920x1080): 60 FPS achieved
- [ ] Desktop (1366x768): 60 FPS achieved
- [ ] iPad: 30+ FPS achieved
- [ ] iPhone: 30+ FPS achieved
- [ ] Android Tablet: 30+ FPS achieved
- [ ] Android Phone: 30+ FPS achieved

### 🔐 Web3 Integration
- [ ] Wallet creation works without errors
- [ ] Token rewards appear in UI
- [ ] No blockchain knowledge required
- [ ] Export wallet feature works
- [ ] Backend integration verified

### ⚡ Performance Metrics
- [ ] Build size < 50MB compressed
- [ ] Load time < 20 seconds on 10Mbps
- [ ] Memory usage < 256MB
- [ ] No memory leaks detected
- [ ] Consistent frame rate maintained

### 🐛 Bug Testing
- [ ] No console errors in production
- [ ] Error tracking working
- [ ] All UI elements clickable
- [ ] Audio plays correctly
- [ ] No visual glitches

### 🔒 Security Checks
- [ ] API keys not exposed
- [ ] HTTPS enabled
- [ ] Input validation working
- [ ] No sensitive data in logs

### 📊 Analytics & Monitoring
- [ ] Analytics tracking events
- [ ] Performance monitoring active
- [ ] Error reporting configured
- [ ] User metrics collecting

## Launch Day Checklist

### 📦 Deployment
- [ ] Production build created
- [ ] Files uploaded to hosting
- [ ] DNS configured correctly
- [ ] SSL certificate active
- [ ] CDN configured (if using)

### 🔍 Final Verification
- [ ] Live URL accessible
- [ ] Game loads properly
- [ ] All features working
- [ ] Social sharing configured
- [ ] SEO meta tags present

### 📢 Marketing Materials
- [ ] Screenshots captured
- [ ] Gameplay video recorded
- [ ] Press kit prepared
- [ ] Social media posts ready
- [ ] Portfolio updated

## Post-Launch Monitoring

### 🚨 First 24 Hours
- [ ] Monitor error logs
- [ ] Check performance metrics
- [ ] Review user feedback
- [ ] Fix critical issues
- [ ] Respond to questions

### 📈 First Week
- [ ] Analyze user behavior
- [ ] Gather feedback
- [ ] Plan improvements
- [ ] Update documentation
- [ ] Celebrate success! 🎉

## Sign-off

**QA Lead**: _________________ **Date**: _________
**Developer**: _______________ **Date**: _________
**Product Owner**: ___________ **Date**: _________
```

### 7. Create Deployment Script
`deploy.sh`:
```bash
#!/bin/bash

echo "=== Plasma Thief: Sky Rogue - Deployment Script ==="

# Configuration
BUILD_DIR="WebBuild"
DEPLOY_BRANCH="gh-pages"
COMMIT_MESSAGE="Deploy build $(date +%Y%m%d-%H%M%S)"

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m'

# Check if build exists
if [ ! -d "$BUILD_DIR" ]; then
    echo -e "${RED}Error: Build directory not found!${NC}"
    exit 1
fi

# Run tests
echo "Running pre-deployment tests..."
npm test
if [ $? -ne 0 ]; then
    echo -e "${RED}Tests failed! Aborting deployment.${NC}"
    exit 1
fi

# Optimize build
echo "Optimizing build..."
cd $BUILD_DIR

# Compress files
find . -name "*.js" -exec gzip -9 -k {} \;
find . -name "*.wasm" -exec gzip -9 -k {} \;
find . -name "*.data" -exec gzip -9 -k {} \;

# Generate service worker
cat > sw.js << 'EOL'
const CACHE_NAME = 'plasma-thief-v1';
const urlsToCache = [
    '/',
    '/css/style.css',
    '/js/main.js',
    '/Build/WebGL.loader.js'
];

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => cache.addAll(urlsToCache))
    );
});

self.addEventListener('fetch', event => {
    event.respondWith(
        caches.match(event.request)
            .then(response => response || fetch(event.request))
    );
});
EOL

cd ..

# Deploy to GitHub Pages
echo "Deploying to GitHub Pages..."
git checkout -B $DEPLOY_BRANCH
git add -f $BUILD_DIR
git commit -m "$COMMIT_MESSAGE"
git push origin $DEPLOY_BRANCH --force

# Switch back to main branch
git checkout main

echo -e "${GREEN}Deployment complete!${NC}"
echo "URL: https://yourusername.github.io/plasma-thief/"

# Post-deployment tasks
echo ""
echo "Post-deployment tasks:"
echo "1. Check live site"
echo "2. Test all features"
echo "3. Monitor error logs"
echo "4. Share with testers"
```

### 8. Create Monitoring Dashboard
`monitoring-dashboard.html`:
```html
<!DOCTYPE html>
<html>
<head>
    <title>Plasma Thief - Monitoring Dashboard</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background: #1a1a1a;
            color: #fff;
            margin: 0;
            padding: 20px;
        }
        
        .dashboard {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
        }
        
        .card {
            background: #2a2a2a;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.3);
        }
        
        .metric {
            font-size: 2em;
            font-weight: bold;
            margin: 10px 0;
        }
        
        .metric.good { color: #4CAF50; }
        .metric.warning { color: #FF9800; }
        .metric.bad { color: #F44336; }
        
        .chart {
            height: 200px;
            margin-top: 20px;
        }
        
        h1 { text-align: center; }
        h3 { margin-top: 0; }
    </style>
</head>
<body>
    <h1>Plasma Thief: Sky Rogue - Live Monitoring</h1>
    
    <div class="dashboard">
        <div class="card">
            <h3>Performance</h3>
            <div class="metric good" id="fps">60 FPS</div>
            <p>Average frame rate</p>
            <canvas id="fpsChart" class="chart"></canvas>
        </div>
        
        <div class="card">
            <h3>Active Players</h3>
            <div class="metric" id="players">0</div>
            <p>Currently playing</p>
            <canvas id="playersChart" class="chart"></canvas>
        </div>
        
        <div class="card">
            <h3>Errors</h3>
            <div class="metric good" id="errors">0</div>
            <p>Last 24 hours</p>
            <div id="errorList"></div>
        </div>
        
        <div class="card">
            <h3>Load Time</h3>
            <div class="metric warning" id="loadTime">15.2s</div>
            <p>Average load time</p>
        </div>
        
        <div class="card">
            <h3>Memory Usage</h3>
            <div class="metric good" id="memory">142 MB</div>
            <p>Average usage</p>
        </div>
        
        <div class="card">
            <h3>Token Distribution</h3>
            <div class="metric" id="tokens">0</div>
            <p>Tokens awarded today</p>
        </div>
    </div>
    
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script>
        // Mock real-time data updates
        function updateMetrics() {
            // Simulate data
            document.getElementById('fps').textContent = 
                Math.round(55 + Math.random() * 10) + ' FPS';
            
            document.getElementById('players').textContent = 
                Math.round(Math.random() * 50);
            
            document.getElementById('memory').textContent = 
                Math.round(130 + Math.random() * 30) + ' MB';
        }
        
        // Update every 5 seconds
        setInterval(updateMetrics, 5000);
        
        // Initialize charts
        const fpsCtx = document.getElementById('fpsChart').getContext('2d');
        new Chart(fpsCtx, {
            type: 'line',
            data: {
                labels: Array(20).fill(''),
                datasets: [{
                    data: Array(20).fill(60),
                    borderColor: '#4CAF50',
                    fill: false
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                legend: { display: false },
                scales: {
                    y: { min: 0, max: 100 }
                }
            }
        });
    </script>
</body>
</html>
```

## Expected Outcomes
- ✅ All automated tests pass
- ✅ Browser compatibility verified
- ✅ Performance targets met
- ✅ No critical bugs found
- ✅ Web3 integration working
- ✅ Deployment successful
- ✅ Monitoring active

## Testing Coverage
- Unit tests: Core systems
- Integration tests: System interactions
- Performance tests: FPS and memory
- Compatibility tests: Browser support
- User acceptance: Playability
- Security tests: API protection
- Load tests: Concurrent users

## Launch Criteria
1. All P0 bugs fixed
2. Performance targets met
3. Browser compatibility confirmed
4. Security audit passed
5. Documentation complete
6. Monitoring configured
7. Rollback plan ready

## Common Issues & Solutions

### Issue: Build Fails Tests
- Check error logs
- Run tests locally
- Fix failing tests
- Rebuild and retest

### Issue: Poor Performance on Mobile
- Reduce quality settings
- Optimize shaders
- Decrease particle count
- Test on real devices

### Issue: Browser Compatibility Issues
- Check polyfills
- Test specific features
- Add fallbacks
- Update documentation

## Time Estimate: 8-10 hours

## Final Steps
1. Complete all testing
2. Fix critical issues
3. Deploy to production
4. Monitor launch
5. Gather feedback
6. Plan updates

---

## Completion Notes
*To be filled after task completion*

**Date Completed**: 
**Time Taken**: 
**Tests Passed**: 
**Bugs Fixed**: 
**Performance Score**: 
**Launch Status**: 