# Task 11: Web3 Integration

## Status: COMPLETED

## Priority: MEDIUM - Key differentiator

## Description
Integrate Solana blockchain for token rewards, implementing seamless Web2-to-Web3 onboarding where players earn real tokens without knowing they're using blockchain.

## Prerequisites
- Task 10 (AI Boss) completed
- Solana Unity SDK installed
- Basic understanding of blockchain concepts

## Step-by-Step Instructions

### 1. Configure Solana Unity SDK
`Assets/_Project/Scripts/Web3/SolanaManager.cs`:
```csharp
using UnityEngine;
using Solana.Unity.Wallet;
using Solana.Unity.SDK;
using System.Threading.Tasks;
using System;

public class SolanaManager : MonoBehaviour
{
    [Header("Network Configuration")]
    public bool useDevnet = true;
    private string rpcUrl => useDevnet ? 
        "https://api.devnet.solana.com" : 
        "https://api.mainnet-beta.solana.com";
    
    [Header("Token Configuration")]
    public string tokenMintAddress = ""; // Your token mint
    public string programId = ""; // Your program ID
    
    private static SolanaManager instance;
    public static SolanaManager Instance => instance;
    
    private Web3 web3;
    private Account playerAccount;
    
    public bool IsInitialized { get; private set; }
    public string PlayerWalletAddress => playerAccount?.PublicKey.ToString();
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public async Task Initialize()
    {
        try
        {
            // Initialize Web3
            web3 = new Web3(rpcUrl);
            
            // Check for existing wallet or create new
            await LoadOrCreateWallet();
            
            IsInitialized = true;
            Debug.Log($"Solana initialized. Wallet: {PlayerWalletAddress}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Solana initialization failed: {e.Message}");
            IsInitialized = false;
        }
    }
    
    async Task LoadOrCreateWallet()
    {
        // Check PlayerPrefs for existing wallet
        string encryptedMnemonic = PlayerPrefs.GetString("WalletMnemonic", "");
        
        if (string.IsNullOrEmpty(encryptedMnemonic))
        {
            // Create new wallet
            var mnemonic = new Mnemonic(WordList.English, WordCount.Twelve);
            playerAccount = new Account(mnemonic);
            
            // Encrypt and save (simple encryption for demo)
            string encrypted = SimpleEncrypt(mnemonic.ToString());
            PlayerPrefs.SetString("WalletMnemonic", encrypted);
            PlayerPrefs.Save();
            
            Debug.Log("Created new wallet");
        }
        else
        {
            // Load existing wallet
            string decrypted = SimpleDecrypt(encryptedMnemonic);
            var mnemonic = new Mnemonic(decrypted);
            playerAccount = new Account(mnemonic);
            
            Debug.Log("Loaded existing wallet");
        }
    }
    
    // Simple encryption for demo - use proper encryption in production
    string SimpleEncrypt(string text)
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text));
    }
    
    string SimpleDecrypt(string encrypted)
    {
        return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encrypted));
    }
    
    public async Task<string> GetWalletBalance()
    {
        if (!IsInitialized) return "0";
        
        try
        {
            var balance = await web3.GetBalance(playerAccount.PublicKey);
            return (balance / 1000000000.0).ToString("F4"); // Convert lamports to SOL
        }
        catch
        {
            return "0";
        }
    }
    
    public string GetWalletAddressShort()
    {
        if (string.IsNullOrEmpty(PlayerWalletAddress)) return "";
        
        // Show first 4 and last 4 characters
        return $"{PlayerWalletAddress.Substring(0, 4)}...{PlayerWalletAddress.Substring(PlayerWalletAddress.Length - 4)}";
    }
}
```

### 2. Create Token Reward System
`Assets/_Project/Scripts/Web3/TokenRewardSystem.cs`:
```csharp
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class TokenRewardSystem : MonoBehaviour
{
    [Header("Reward Configuration")]
    public int level1Reward = 5;
    public int level2Reward = 8;
    public int level3Reward = 10;
    public int perfectRunBonus = 3;
    
    [Header("UI References")]
    public GameObject rewardPopupPrefab;
    public Transform uiCanvas;
    
    private static TokenRewardSystem instance;
    public static TokenRewardSystem Instance => instance;
    
    // Track pending rewards
    private Queue<PendingReward> pendingRewards = new Queue<PendingReward>();
    
    [System.Serializable]
    public class PendingReward
    {
        public string playerId;
        public int amount;
        public string reason;
        public float timestamp;
    }
    
    void Awake()
    {
        instance = this;
    }
    
    public void RewardTokens(int amount, string reason)
    {
        // Queue reward
        var reward = new PendingReward
        {
            playerId = SolanaManager.Instance.PlayerWalletAddress,
            amount = amount,
            reason = reason,
            timestamp = Time.time
        };
        
        pendingRewards.Enqueue(reward);
        
        // Show UI immediately
        ShowRewardPopup(amount, reason);
        
        // Process reward
        StartCoroutine(ProcessReward(reward));
    }
    
    IEnumerator ProcessReward(PendingReward reward)
    {
        // In production, this would call your backend
        // For demo, we simulate the process
        
        yield return new WaitForSeconds(1f);
        
        // Save to backend via Supabase
        bool success = await SaveRewardToBackend(reward);
        
        if (success)
        {
            Debug.Log($"Reward processed: {reward.amount} tokens");
            UpdateLocalBalance(reward.amount);
        }
        else
        {
            Debug.LogError("Failed to process reward");
            // Could retry or save locally
        }
    }
    
    async Task<bool> SaveRewardToBackend(PendingReward reward)
    {
        // This would connect to your Supabase instance
        // For demo purposes, we'll simulate
        
        try
        {
            // Simulate API call
            await Task.Delay(500);
            
            // In production:
            // var supabase = Supabase.Client;
            // await supabase.From<TokenReward>().Insert(reward);
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    void ShowRewardPopup(int amount, string reason)
    {
        if (rewardPopupPrefab == null || uiCanvas == null) return;
        
        GameObject popup = Instantiate(rewardPopupPrefab, uiCanvas);
        RewardPopupUI popupUI = popup.GetComponent<RewardPopupUI>();
        
        if (popupUI != null)
        {
            popupUI.Show(amount, reason);
        }
    }
    
    void UpdateLocalBalance(int amount)
    {
        int currentBalance = PlayerPrefs.GetInt("TokenBalance", 0);
        currentBalance += amount;
        PlayerPrefs.SetInt("TokenBalance", currentBalance);
        PlayerPrefs.Save();
    }
    
    public int GetLocalBalance()
    {
        return PlayerPrefs.GetInt("TokenBalance", 0);
    }
}
```

### 3. Create Seamless Onboarding
`Assets/_Project/Scripts/Web3/SeamlessOnboarding.cs`:
```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class SeamlessOnboarding : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject emailLoginPanel;
    public InputField emailInput;
    public InputField passwordInput;
    public Button loginButton;
    public Text statusText;
    
    [Header("Wallet UI")]
    public GameObject walletPanel;
    public Text walletAddressText;
    public Text balanceText;
    public Button exportWalletButton;
    
    private bool isInitializing = false;
    
    void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        exportWalletButton.onClick.AddListener(OnExportWallet);
        
        // Check if already logged in
        if (PlayerPrefs.HasKey("UserEmail"))
        {
            AutoLogin();
        }
    }
    
    async void OnLoginClicked()
    {
        if (isInitializing) return;
        
        string email = emailInput.text;
        string password = passwordInput.text;
        
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowStatus("Please enter email and password");
            return;
        }
        
        isInitializing = true;
        loginButton.interactable = false;
        
        ShowStatus("Creating your account...");
        
        // Simulate account creation/login
        await Task.Delay(1000);
        
        // Initialize Solana in background
        ShowStatus("Setting up rewards wallet...");
        await SolanaManager.Instance.Initialize();
        
        if (SolanaManager.Instance.IsInitialized)
        {
            // Save credentials
            PlayerPrefs.SetString("UserEmail", email);
            PlayerPrefs.Save();
            
            ShowStatus("Success! Starting game...");
            
            // Show wallet info
            ShowWalletInfo();
            
            // Start game after delay
            await Task.Delay(2000);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level01_FortressIsland");
        }
        else
        {
            ShowStatus("Setup failed. Please try again.");
            loginButton.interactable = true;
        }
        
        isInitializing = false;
    }
    
    async void AutoLogin()
    {
        ShowStatus("Logging in...");
        emailLoginPanel.SetActive(false);
        
        await SolanaManager.Instance.Initialize();
        
        if (SolanaManager.Instance.IsInitialized)
        {
            ShowWalletInfo();
            await Task.Delay(1000);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level01_FortressIsland");
        }
        else
        {
            emailLoginPanel.SetActive(true);
            ShowStatus("Please log in");
        }
    }
    
    void ShowWalletInfo()
    {
        emailLoginPanel.SetActive(false);
        walletPanel.SetActive(true);
        
        walletAddressText.text = $"Wallet: {SolanaManager.Instance.GetWalletAddressShort()}";
        balanceText.text = $"Balance: {TokenRewardSystem.Instance.GetLocalBalance()} PLASMA";
    }
    
    void OnExportWallet()
    {
        // Show export options
        Debug.Log("Export wallet clicked - would show mnemonic phrase");
        // In production, show proper security warnings
    }
    
    void ShowStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }
}
```

### 4. Create Reward Popup UI
`Assets/_Project/Scripts/UI/RewardPopupUI.cs`:
```csharp
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Optional: for animations

public class RewardPopupUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text amountText;
    public Text reasonText;
    public Image tokenIcon;
    public CanvasGroup canvasGroup;
    
    [Header("Animation")]
    public float fadeInDuration = 0.5f;
    public float displayDuration = 3f;
    public float fadeOutDuration = 0.5f;
    public float moveUpDistance = 100f;
    
    public void Show(int amount, string reason)
    {
        amountText.text = $"+{amount} PLASMA";
        reasonText.text = reason;
        
        // Start animation sequence
        StartCoroutine(AnimatePopup());
    }
    
    IEnumerator AnimatePopup()
    {
        // Initial state
        canvasGroup.alpha = 0;
        transform.localPosition = Vector3.zero;
        
        // Fade in and move up
        float elapsed = 0;
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + Vector3.up * moveUpDistance;
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeInDuration;
            
            canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            
            yield return null;
        }
        
        // Display
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out
        elapsed = 0;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;
            
            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            
            yield return null;
        }
        
        // Destroy
        Destroy(gameObject);
    }
}
```

### 5. Create Victory Screen with Rewards
`Assets/_Project/Scripts/UI/VictoryScreen.cs`:
```csharp
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    [Header("UI References")]
    public GameObject victoryPanel;
    public Text levelCompleteText;
    public Text tokensEarnedText;
    public Text totalTokensText;
    public Button continueButton;
    public Button mainMenuButton;
    
    [Header("Reward Breakdown")]
    public Text baseRewardText;
    public Text bonusRewardText;
    public GameObject perfectRunBonus;
    
    public void ShowVictory(int level, bool perfectRun)
    {
        victoryPanel.SetActive(true);
        Time.timeScale = 0; // Pause game
        
        // Calculate rewards
        int baseReward = GetBaseReward(level);
        int bonus = perfectRun ? 3 : 0;
        int totalReward = baseReward + bonus;
        
        // Update UI
        levelCompleteText.text = $"Level {level} Complete!";
        baseRewardText.text = $"Base Reward: {baseReward} PLASMA";
        
        if (perfectRun)
        {
            perfectRunBonus.SetActive(true);
            bonusRewardText.text = $"Perfect Run: +{bonus} PLASMA";
        }
        else
        {
            perfectRunBonus.SetActive(false);
        }
        
        tokensEarnedText.text = $"Total Earned: {totalReward} PLASMA";
        
        // Award tokens
        TokenRewardSystem.Instance.RewardTokens(totalReward, $"Level {level} Victory");
        
        // Update total
        totalTokensText.text = $"Balance: {TokenRewardSystem.Instance.GetLocalBalance()} PLASMA";
        
        // Button listeners
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => OnContinue(level));
        
        mainMenuButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.AddListener(OnMainMenu);
    }
    
    int GetBaseReward(int level)
    {
        switch (level)
        {
            case 1: return 5;
            case 2: return 8;
            case 3: return 10;
            default: return 5;
        }
    }
    
    void OnContinue(int currentLevel)
    {
        Time.timeScale = 1;
        
        if (currentLevel < 3)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene($"Level{currentLevel + 1:D2}_FortressIsland");
        }
        else
        {
            OnMainMenu();
        }
    }
    
    void OnMainMenu()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
```

### 6. Backend Integration Setup
`Assets/_Project/Scripts/Web3/BackendConnector.cs`:
```csharp
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BackendConnector : MonoBehaviour
{
    [Header("Backend Configuration")]
    public string supabaseUrl = "https://your-project.supabase.co";
    public string supabaseAnonKey = ""; // Public anon key
    public string n8nWebhookUrl = ""; // For token distribution
    
    public IEnumerator SendRewardToBackend(string wallet, int amount, string reason)
    {
        var data = new
        {
            wallet_address = wallet,
            amount = amount,
            reason = reason,
            timestamp = System.DateTime.UtcNow.ToString("o"),
            game_version = Application.version
        };
        
        string json = JsonUtility.ToJson(data);
        
        using (UnityWebRequest request = UnityWebRequest.Post(n8nWebhookUrl, json, "application/json"))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Reward sent to backend");
            }
            else
            {
                Debug.LogError($"Backend error: {request.error}");
                // Queue for retry
            }
        }
    }
}
```

### 7. Create Wallet Export Feature
`Assets/_Project/Scripts/Web3/WalletExporter.cs`:
```csharp
using UnityEngine;
using UnityEngine.UI;

public class WalletExporter : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject exportPanel;
    public Text mnemonicText;
    public Button copyButton;
    public Button closeButton;
    public GameObject warningText;
    
    public void ShowExportPanel()
    {
        exportPanel.SetActive(true);
        
        // Get mnemonic (in production, require password)
        string encryptedMnemonic = PlayerPrefs.GetString("WalletMnemonic", "");
        string mnemonic = SimpleDecrypt(encryptedMnemonic);
        
        mnemonicText.text = mnemonic;
        
        copyButton.onClick.RemoveAllListeners();
        copyButton.onClick.AddListener(() => 
        {
            GUIUtility.systemCopyBuffer = mnemonic;
            Debug.Log("Mnemonic copied to clipboard");
        });
        
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => exportPanel.SetActive(false));
    }
    
    string SimpleDecrypt(string encrypted)
    {
        return System.Text.Encoding.UTF8.GetString(
            System.Convert.FromBase64String(encrypted));
    }
}
```

## Expected Outcomes
- ✅ Players login with email/password only
- ✅ Wallet created automatically in background
- ✅ Tokens awarded instantly on victory
- ✅ Balance visible in game UI
- ✅ Optional wallet export for advanced users
- ✅ No blockchain knowledge required
- ✅ Smooth onboarding under 60 seconds

## Configuration Steps
1. Create Solana token on Devnet
2. Set up Supabase project (free tier)
3. Configure n8n automation
4. Add environment variables
5. Test wallet creation
6. Verify token distribution

## Security Considerations
- Never store private keys in plain text
- Use proper encryption for mnemonics
- Implement rate limiting
- Add transaction verification
- Monitor for unusual activity

## Common Issues & Solutions

### Issue: Wallet Creation Fails
- Check network connectivity
- Verify RPC endpoint
- Add retry logic
- Provide offline mode

### Issue: Token Distribution Delayed
- Check backend status
- Verify webhook configuration
- Add queue system
- Show pending status

### Issue: Players Confused by Blockchain
- Hide technical details
- Use familiar terms
- Provide simple explanations
- Focus on rewards

## Time Estimate: 4-5 hours

## Next Steps
Proceed to Task 12: Landing Page Creation

---

## Completion Notes
*To be filled after task completion*

**Date Completed**: 
**Time Taken**: 
**Integration Method**: 
**Wallet Creation Time**: 
**Token Distribution Time**: 
**User Feedback**: 