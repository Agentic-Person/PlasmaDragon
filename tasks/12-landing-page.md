# Task 12: Landing Page Creation

## Status: Pending

## Priority: MEDIUM - First player impression

## Description
Create a simple, effective landing page that showcases the game, handles user registration, and embeds the Unity WebGL build. Focus on quick loading and seamless onboarding.

## Prerequisites
- Task 11 (Web3 Integration) completed
- WebGL build working
- Basic HTML/CSS/JS knowledge

## Step-by-Step Instructions

### 1. Create Landing Page Structure
Create `WebBuild/index.html`:
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Plasma Thief: Sky Rogue - Dragon Combat Game</title>
    <meta name="description" content="Battle AI-powered bosses while riding a dragon. Earn real crypto rewards in this aerial combat game!">
    
    <!-- Favicon -->
    <link rel="icon" type="image/png" href="assets/favicon.png">
    
    <!-- Styles -->
    <link rel="stylesheet" href="css/style.css">
    
    <!-- Unity WebGL Loader -->
    <script src="Build/UnityLoader.js"></script>
</head>
<body>
    <!-- Hero Section -->
    <section id="hero" class="hero-section">
        <div class="hero-background">
            <video autoplay muted loop playsinline>
                <source src="assets/hero-gameplay.mp4" type="video/mp4">
                <img src="assets/hero-fallback.jpg" alt="Dragon Combat">
            </video>
            <div class="hero-overlay"></div>
        </div>
        
        <div class="hero-content">
            <h1 class="game-title">Plasma Thief: Sky Rogue</h1>
            <p class="game-tagline">Ride Dragons. Battle AI Bosses. Earn Real Rewards.</p>
            
            <div class="cta-buttons">
                <button id="playNowBtn" class="btn btn-primary btn-large">
                    Play Now
                    <span class="btn-subtext">No download required</span>
                </button>
                <button id="watchTrailerBtn" class="btn btn-secondary">
                    <svg class="icon-play" viewBox="0 0 24 24">
                        <path d="M8 5v14l11-7z"/>
                    </svg>
                    Watch Trailer
                </button>
            </div>
            
            <div class="trust-indicators">
                <div class="indicator">
                    <svg class="icon" viewBox="0 0 24 24">
                        <path d="M12 2L2 7v10c0 5.55 3.84 10.74 9 12 5.16-1.26 9-6.45 9-12V7l-10-5z"/>
                    </svg>
                    <span>Blockchain Secured</span>
                </div>
                <div class="indicator">
                    <svg class="icon" viewBox="0 0 24 24">
                        <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z"/>
                    </svg>
                    <span>Free to Play</span>
                </div>
                <div class="indicator">
                    <svg class="icon" viewBox="0 0 24 24">
                        <path d="M11.8 10.9c-2.27-.59-3-1.2-3-2.15 0-1.09 1.01-1.85 2.7-1.85 1.78 0 2.44.85 2.5 2.1h2.21c-.07-1.72-1.12-3.3-3.21-3.81V3h-3v2.16c-1.94.42-3.5 1.68-3.5 3.61 0 2.31 1.91 3.46 4.7 4.13 2.5.6 3 1.48 3 2.41 0 .69-.49 1.79-2.7 1.79-2.06 0-2.87-.92-2.98-2.1h-2.2c.12 2.19 1.76 3.42 3.68 3.83V21h3v-2.15c1.95-.37 3.5-1.5 3.5-3.55 0-2.84-2.43-3.81-4.7-4.4z"/>
                    </svg>
                    <span>Earn Real Tokens</span>
                </div>
            </div>
        </div>
        
        <div class="scroll-indicator">
            <svg viewBox="0 0 24 24">
                <path d="M7.41 8.59L12 13.17l4.59-4.58L18 10l-6 6-6-6 1.41-1.41z"/>
            </svg>
        </div>
    </section>
    
    <!-- Game Embed Modal -->
    <div id="gameModal" class="modal">
        <div class="modal-content">
            <button class="modal-close" id="closeGameBtn">&times;</button>
            
            <!-- Login/Registration Form -->
            <div id="authForm" class="auth-container">
                <h2>Start Your Adventure</h2>
                <p class="auth-subtitle">Create an account to save progress and earn rewards</p>
                
                <form id="loginForm">
                    <div class="form-group">
                        <input type="email" id="emailInput" placeholder="Email address" required>
                    </div>
                    <div class="form-group">
                        <input type="password" id="passwordInput" placeholder="Password" required>
                    </div>
                    <button type="submit" class="btn btn-primary btn-block">
                        Start Playing
                    </button>
                </form>
                
                <div class="auth-divider">
                    <span>or</span>
                </div>
                
                <button id="guestPlayBtn" class="btn btn-secondary btn-block">
                    Play as Guest
                    <span class="btn-subtext">Progress won't be saved</span>
                </button>
                
                <p class="auth-note">
                    By playing, you agree to our <a href="#terms">Terms</a> and <a href="#privacy">Privacy Policy</a>
                </p>
            </div>
            
            <!-- Unity WebGL Container -->
            <div id="gameContainer" class="game-container" style="display: none;">
                <div id="unityContainer"></div>
                <div class="game-loading">
                    <div class="loading-bar">
                        <div class="loading-fill" id="loadingFill"></div>
                    </div>
                    <p class="loading-text" id="loadingText">Loading Dragon Assets...</p>
                </div>
            </div>
        </div>
    </div>
    
    <!-- Features Section -->
    <section id="features" class="features-section">
        <div class="container">
            <h2 class="section-title">Epic Dragon Combat</h2>
            
            <div class="features-grid">
                <div class="feature-card">
                    <div class="feature-icon">
                        <img src="assets/icon-dragon.svg" alt="Dragon Flight">
                    </div>
                    <h3>Soar Through the Skies</h3>
                    <p>Master responsive flight controls as you pilot your plasma dragon through stunning aerial battles.</p>
                </div>
                
                <div class="feature-card">
                    <div class="feature-icon">
                        <img src="assets/icon-ai.svg" alt="AI Bosses">
                    </div>
                    <h3>AI-Powered Bosses</h3>
                    <p>Face intelligent enemies that learn and adapt. Every encounter is unique and challenging.</p>
                </div>
                
                <div class="feature-card">
                    <div class="feature-icon">
                        <img src="assets/icon-rewards.svg" alt="Token Rewards">
                    </div>
                    <h3>Earn Real Rewards</h3>
                    <p>Collect PLASMA tokens for victories. Real blockchain rewards without the complexity.</p>
                </div>
            </div>
        </div>
    </section>
    
    <!-- How It Works -->
    <section id="howItWorks" class="how-section">
        <div class="container">
            <h2 class="section-title">Start Earning in 60 Seconds</h2>
            
            <div class="steps-timeline">
                <div class="step">
                    <div class="step-number">1</div>
                    <h3>Quick Sign Up</h3>
                    <p>Just email and password. No wallet setup needed.</p>
                </div>
                
                <div class="step">
                    <div class="step-number">2</div>
                    <h3>Battle & Win</h3>
                    <p>Defeat AI bosses and complete levels.</p>
                </div>
                
                <div class="step">
                    <div class="step-number">3</div>
                    <h3>Earn Tokens</h3>
                    <p>Automatically receive PLASMA tokens for victories.</p>
                </div>
                
                <div class="step">
                    <div class="step-number">4</div>
                    <h3>Real Value</h3>
                    <p>Trade, hold, or export your tokens anytime.</p>
                </div>
            </div>
        </div>
    </section>
    
    <!-- Footer -->
    <footer class="footer">
        <div class="container">
            <div class="footer-content">
                <div class="footer-brand">
                    <h3>Plasma Thief: Sky Rogue</h3>
                    <p>A Web3 gaming portfolio demo</p>
                </div>
                
                <div class="footer-links">
                    <a href="#terms">Terms</a>
                    <a href="#privacy">Privacy</a>
                    <a href="https://github.com/yourusername/plasma-thief" target="_blank">GitHub</a>
                    <a href="#contact">Contact</a>
                </div>
            </div>
            
            <div class="footer-bottom">
                <p>&copy; 2024 Your Name. Built with Unity & Solana.</p>
            </div>
        </div>
    </footer>
    
    <!-- Scripts -->
    <script src="js/main.js"></script>
</body>
</html>
```

### 2. Create CSS Styles
Create `WebBuild/css/style.css`:
```css
/* Reset and Base Styles */
* {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
}

:root {
    --primary-color: #7B68EE;
    --secondary-color: #FF6B6B;
    --dark-bg: #0A0E27;
    --light-bg: #1A1F3A;
    --text-light: #E0E0E0;
    --text-dim: #A0A0A0;
    --accent-glow: 0 0 20px rgba(123, 104, 238, 0.5);
}

body {
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
    background-color: var(--dark-bg);
    color: var(--text-light);
    line-height: 1.6;
    overflow-x: hidden;
}

/* Hero Section */
.hero-section {
    position: relative;
    height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
    overflow: hidden;
}

.hero-background {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: -1;
}

.hero-background video,
.hero-background img {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.hero-overlay {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: linear-gradient(rgba(10, 14, 39, 0.7), rgba(10, 14, 39, 0.9));
}

.hero-content {
    text-align: center;
    z-index: 1;
    max-width: 800px;
    padding: 0 20px;
}

.game-title {
    font-size: 4rem;
    font-weight: 800;
    margin-bottom: 1rem;
    background: linear-gradient(45deg, var(--primary-color), var(--secondary-color));
    background-clip: text;
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    text-shadow: var(--accent-glow);
}

.game-tagline {
    font-size: 1.5rem;
    color: var(--text-dim);
    margin-bottom: 3rem;
}

/* Buttons */
.btn {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    padding: 1rem 2rem;
    border: none;
    border-radius: 50px;
    font-size: 1rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s ease;
    text-decoration: none;
    position: relative;
    overflow: hidden;
}

.btn-primary {
    background: linear-gradient(45deg, var(--primary-color), #9B59B6);
    color: white;
    box-shadow: 0 4px 15px rgba(123, 104, 238, 0.3);
}

.btn-primary:hover {
    transform: translateY(-2px);
    box-shadow: 0 6px 20px rgba(123, 104, 238, 0.4);
}

.btn-large {
    padding: 1.25rem 3rem;
    font-size: 1.25rem;
    flex-direction: column;
    gap: 0.25rem;
}

.btn-subtext {
    font-size: 0.875rem;
    opacity: 0.8;
    font-weight: 400;
}

.btn-secondary {
    background: transparent;
    color: var(--text-light);
    border: 2px solid rgba(255, 255, 255, 0.2);
    gap: 0.5rem;
}

.btn-secondary:hover {
    background: rgba(255, 255, 255, 0.1);
    border-color: rgba(255, 255, 255, 0.3);
}

.cta-buttons {
    display: flex;
    gap: 1rem;
    justify-content: center;
    margin-bottom: 3rem;
    flex-wrap: wrap;
}

/* Trust Indicators */
.trust-indicators {
    display: flex;
    gap: 2rem;
    justify-content: center;
    flex-wrap: wrap;
}

.indicator {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    color: var(--text-dim);
    font-size: 0.875rem;
}

.indicator .icon {
    width: 20px;
    height: 20px;
    fill: var(--primary-color);
}

/* Modal */
.modal {
    display: none;
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.9);
    z-index: 1000;
    align-items: center;
    justify-content: center;
}

.modal.active {
    display: flex;
}

.modal-content {
    background: var(--light-bg);
    border-radius: 20px;
    width: 90%;
    max-width: 1200px;
    height: 90vh;
    position: relative;
    overflow: hidden;
}

.modal-close {
    position: absolute;
    top: 20px;
    right: 20px;
    background: rgba(255, 255, 255, 0.1);
    border: none;
    color: white;
    font-size: 2rem;
    width: 40px;
    height: 40px;
    border-radius: 50%;
    cursor: pointer;
    z-index: 10;
    transition: all 0.3s ease;
}

.modal-close:hover {
    background: rgba(255, 255, 255, 0.2);
    transform: rotate(90deg);
}

/* Auth Form */
.auth-container {
    max-width: 400px;
    margin: 0 auto;
    padding: 3rem;
}

.auth-container h2 {
    font-size: 2rem;
    margin-bottom: 0.5rem;
    text-align: center;
}

.auth-subtitle {
    color: var(--text-dim);
    text-align: center;
    margin-bottom: 2rem;
}

.form-group {
    margin-bottom: 1rem;
}

.form-group input {
    width: 100%;
    padding: 1rem;
    background: rgba(255, 255, 255, 0.05);
    border: 1px solid rgba(255, 255, 255, 0.1);
    border-radius: 10px;
    color: white;
    font-size: 1rem;
    transition: all 0.3s ease;
}

.form-group input:focus {
    outline: none;
    border-color: var(--primary-color);
    background: rgba(255, 255, 255, 0.08);
}

.btn-block {
    width: 100%;
    margin-top: 1rem;
}

.auth-divider {
    text-align: center;
    margin: 2rem 0;
    position: relative;
}

.auth-divider span {
    background: var(--light-bg);
    padding: 0 1rem;
    color: var(--text-dim);
}

.auth-divider::before {
    content: '';
    position: absolute;
    top: 50%;
    left: 0;
    right: 0;
    height: 1px;
    background: rgba(255, 255, 255, 0.1);
    z-index: -1;
}

/* Game Container */
.game-container {
    width: 100%;
    height: 100%;
    position: relative;
}

#unityContainer {
    width: 100%;
    height: 100%;
}

.game-loading {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    text-align: center;
    width: 300px;
}

.loading-bar {
    width: 100%;
    height: 4px;
    background: rgba(255, 255, 255, 0.1);
    border-radius: 2px;
    overflow: hidden;
    margin-bottom: 1rem;
}

.loading-fill {
    height: 100%;
    background: linear-gradient(90deg, var(--primary-color), var(--secondary-color));
    width: 0%;
    transition: width 0.3s ease;
}

/* Features Section */
.features-section {
    padding: 5rem 0;
    background: var(--light-bg);
}

.container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 20px;
}

.section-title {
    font-size: 3rem;
    text-align: center;
    margin-bottom: 3rem;
}

.features-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 2rem;
}

.feature-card {
    background: rgba(255, 255, 255, 0.05);
    border-radius: 15px;
    padding: 2rem;
    text-align: center;
    transition: all 0.3s ease;
}

.feature-card:hover {
    transform: translateY(-5px);
    background: rgba(255, 255, 255, 0.08);
}

.feature-icon {
    width: 80px;
    height: 80px;
    margin: 0 auto 1rem;
}

.feature-icon img {
    width: 100%;
    height: 100%;
    filter: drop-shadow(0 0 10px var(--primary-color));
}

/* Responsive Design */
@media (max-width: 768px) {
    .game-title {
        font-size: 2.5rem;
    }
    
    .game-tagline {
        font-size: 1.25rem;
    }
    
    .cta-buttons {
        flex-direction: column;
        align-items: center;
    }
    
    .btn-large {
        width: 100%;
        max-width: 300px;
    }
    
    .trust-indicators {
        flex-direction: column;
        align-items: center;
        gap: 1rem;
    }
    
    .modal-content {
        width: 100%;
        height: 100%;
        border-radius: 0;
    }
}

/* Animations */
@keyframes fadeIn {
    from {
        opacity: 0;
        transform: translateY(20px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.hero-content > * {
    animation: fadeIn 0.8s ease-out forwards;
    opacity: 0;
}

.hero-content > *:nth-child(1) { animation-delay: 0.2s; }
.hero-content > *:nth-child(2) { animation-delay: 0.4s; }
.hero-content > *:nth-child(3) { animation-delay: 0.6s; }
.hero-content > *:nth-child(4) { animation-delay: 0.8s; }

/* Scroll Indicator */
.scroll-indicator {
    position: absolute;
    bottom: 2rem;
    left: 50%;
    transform: translateX(-50%);
    animation: bounce 2s infinite;
}

.scroll-indicator svg {
    width: 30px;
    height: 30px;
    fill: var(--text-dim);
}

@keyframes bounce {
    0%, 20%, 50%, 80%, 100% {
        transform: translateX(-50%) translateY(0);
    }
    40% {
        transform: translateX(-50%) translateY(-10px);
    }
    60% {
        transform: translateX(-50%) translateY(-5px);
    }
}
```

### 3. Create JavaScript Logic
Create `WebBuild/js/main.js`:
```javascript
// Main JavaScript for Landing Page
class PlasmaThiefLanding {
    constructor() {
        this.modal = document.getElementById('gameModal');
        this.authForm = document.getElementById('authForm');
        this.gameContainer = document.getElementById('gameContainer');
        this.unityInstance = null;
        this.isGameLoaded = false;
        
        this.init();
    }
    
    init() {
        // Button listeners
        document.getElementById('playNowBtn').addEventListener('click', () => this.openGame());
        document.getElementById('closeGameBtn').addEventListener('click', () => this.closeGame());
        document.getElementById('watchTrailerBtn').addEventListener('click', () => this.playTrailer());
        document.getElementById('guestPlayBtn').addEventListener('click', () => this.playAsGuest());
        
        // Form submission
        document.getElementById('loginForm').addEventListener('submit', (e) => this.handleLogin(e));
        
        // Smooth scrolling
        this.initSmoothScroll();
        
        // Check for returning user
        this.checkReturningUser();
    }
    
    openGame() {
        this.modal.classList.add('active');
        document.body.style.overflow = 'hidden';
        
        // Analytics
        this.trackEvent('game_open');
    }
    
    closeGame() {
        this.modal.classList.remove('active');
        document.body.style.overflow = 'auto';
        
        // Pause Unity if loaded
        if (this.unityInstance) {
            this.unityInstance.SendMessage('GameManager', 'PauseGame');
        }
    }
    
    async handleLogin(e) {
        e.preventDefault();
        
        const email = document.getElementById('emailInput').value;
        const password = document.getElementById('passwordInput').value;
        
        // Show loading state
        const submitBtn = e.target.querySelector('button[type="submit"]');
        const originalText = submitBtn.textContent;
        submitBtn.textContent = 'Loading...';
        submitBtn.disabled = true;
        
        try {
            // Simulate authentication (in production, call your backend)
            await this.authenticateUser(email, password);
            
            // Save session
            localStorage.setItem('userEmail', email);
            localStorage.setItem('sessionToken', this.generateSessionToken());
            
            // Load game
            await this.loadUnityGame(false);
            
        } catch (error) {
            console.error('Login error:', error);
            alert('Login failed. Please try again.');
            
        } finally {
            submitBtn.textContent = originalText;
            submitBtn.disabled = false;
        }
    }
    
    async playAsGuest() {
        // Track guest play
        this.trackEvent('guest_play');
        
        // Load game without authentication
        await this.loadUnityGame(true);
    }
    
    async loadUnityGame(isGuest) {
        // Hide auth form
        this.authForm.style.display = 'none';
        this.gameContainer.style.display = 'block';
        
        // Update loading text
        const loadingText = document.getElementById('loadingText');
        const loadingFill = document.getElementById('loadingFill');
        
        if (!this.isGameLoaded) {
            // Unity WebGL configuration
            const buildUrl = 'Build';
            const config = {
                dataUrl: buildUrl + '/WebGL.data',
                frameworkUrl: buildUrl + '/WebGL.framework.js',
                codeUrl: buildUrl + '/WebGL.wasm',
                streamingAssetsUrl: 'StreamingAssets',
                companyName: 'YourStudio',
                productName: 'PlasmaThief',
                productVersion: '1.0.0',
            };
            
            // Create Unity instance
            createUnityInstance(document.querySelector('#unityContainer'), config, (progress) => {
                // Update loading bar
                const percent = Math.round(progress * 100);
                loadingFill.style.width = percent + '%';
                
                // Update loading text
                if (progress < 0.3) {
                    loadingText.textContent = 'Loading Dragon Assets...';
                } else if (progress < 0.6) {
                    loadingText.textContent = 'Preparing Fortress...';
                } else if (progress < 0.9) {
                    loadingText.textContent = 'Summoning AI Boss...';
                } else {
                    loadingText.textContent = 'Almost Ready...';
                }
            }).then((unityInstance) => {
                this.unityInstance = unityInstance;
                this.isGameLoaded = true;
                
                // Hide loading
                document.querySelector('.game-loading').style.display = 'none';
                
                // Send user data to Unity
                if (!isGuest) {
                    const email = localStorage.getItem('userEmail');
                    unityInstance.SendMessage('AuthManager', 'SetUserEmail', email);
                }
                
                // Track successful load
                this.trackEvent('game_loaded');
                
            }).catch((error) => {
                console.error('Unity loading error:', error);
                loadingText.textContent = 'Loading failed. Please refresh and try again.';
            });
        } else {
            // Game already loaded, just show it
            document.querySelector('.game-loading').style.display = 'none';
            
            // Resume game
            if (this.unityInstance) {
                this.unityInstance.SendMessage('GameManager', 'ResumeGame');
            }
        }
    }
    
    playTrailer() {
        // Open trailer in modal or new tab
        window.open('https://youtube.com/watch?v=your-trailer-id', '_blank');
        this.trackEvent('trailer_played');
    }
    
    checkReturningUser() {
        const userEmail = localStorage.getItem('userEmail');
        if (userEmail) {
            // Update UI for returning user
            document.getElementById('emailInput').value = userEmail;
        }
    }
    
    initSmoothScroll() {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                e.preventDefault();
                const target = document.querySelector(this.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    }
    
    async authenticateUser(email, password) {
        // In production, this would call your backend API
        // For demo, we simulate authentication
        
        return new Promise((resolve, reject) => {
            setTimeout(() => {
                if (email && password.length >= 6) {
                    resolve({ success: true, token: this.generateSessionToken() });
                } else {
                    reject(new Error('Invalid credentials'));
                }
            }, 1000);
        });
    }
    
    generateSessionToken() {
        return 'demo_' + Math.random().toString(36).substr(2, 9);
    }
    
    trackEvent(eventName, eventData = {}) {
        // Analytics tracking
        if (typeof gtag !== 'undefined') {
            gtag('event', eventName, eventData);
        }
        
        console.log('Event tracked:', eventName, eventData);
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    new PlasmaThiefLanding();
});

// Handle Unity-to-JavaScript communication
window.UnityToJavaScript = {
    OnTokensEarned: function(amount) {
        console.log('Tokens earned:', amount);
        // Could show notification or update UI
    },
    
    OnLevelComplete: function(level) {
        console.log('Level completed:', level);
        // Track completion
    },
    
    RequestWalletExport: function() {
        // Handle wallet export request from Unity
        console.log('Wallet export requested');
    }
};
```

### 4. Create Mobile Responsive Adjustments
Add to `style.css`:
```css
/* Mobile Optimizations */
@media (max-width: 480px) {
    .hero-content {
        padding: 0 15px;
    }
    
    .game-title {
        font-size: 2rem;
    }
    
    .auth-container {
        padding: 2rem 1rem;
    }
    
    .features-grid {
        grid-template-columns: 1fr;
        gap: 1rem;
    }
    
    .modal-content {
        height: 100vh;
        height: -webkit-fill-available;
    }
    
    #unityContainer {
        height: 100vh;
        height: -webkit-fill-available;
    }
}

/* PWA Support */
@media (display-mode: standalone) {
    .modal-close {
        top: env(safe-area-inset-top, 20px);
        right: env(safe-area-inset-right, 20px);
    }
}

/* Loading Animation */
@keyframes pulse {
    0% { opacity: 0.6; }
    50% { opacity: 1; }
    100% { opacity: 0.6; }
}

.loading-text {
    animation: pulse 2s ease-in-out infinite;
}
```

### 5. Create Service Worker for Offline Support
Create `WebBuild/sw.js`:
```javascript
const CACHE_NAME = 'plasma-thief-v1';
const urlsToCache = [
    '/',
    '/css/style.css',
    '/js/main.js',
    '/Build/WebGL.loader.js',
    '/assets/hero-fallback.jpg'
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
```

### 6. SEO and Meta Tags
Add to `index.html` head:
```html
<!-- SEO Meta Tags -->
<meta property="og:title" content="Plasma Thief: Sky Rogue - Dragon Combat Game">
<meta property="og:description" content="Battle AI-powered bosses while riding a dragon. Earn real crypto rewards!">
<meta property="og:image" content="https://yourdomain.com/assets/og-image.jpg">
<meta property="og:url" content="https://yourdomain.com">
<meta property="og:type" content="website">

<!-- Twitter Card -->
<meta name="twitter:card" content="summary_large_image">
<meta name="twitter:title" content="Plasma Thief: Sky Rogue">
<meta name="twitter:description" content="Epic dragon combat with blockchain rewards">
<meta name="twitter:image" content="https://yourdomain.com/assets/twitter-card.jpg">

<!-- PWA Manifest -->
<link rel="manifest" href="manifest.json">
<meta name="theme-color" content="#7B68EE">
```

### 7. Create Manifest for PWA
Create `WebBuild/manifest.json`:
```json
{
    "name": "Plasma Thief: Sky Rogue",
    "short_name": "PlasmaThief",
    "description": "Dragon combat game with blockchain rewards",
    "start_url": "/",
    "display": "standalone",
    "background_color": "#0A0E27",
    "theme_color": "#7B68EE",
    "icons": [
        {
            "src": "assets/icon-192.png",
            "sizes": "192x192",
            "type": "image/png"
        },
        {
            "src": "assets/icon-512.png",
            "sizes": "512x512",
            "type": "image/png"
        }
    ]
}
```

### 8. Deployment Configuration
Create `WebBuild/.htaccess` for Apache:
```apache
# Enable compression
<IfModule mod_deflate.c>
    AddOutputFilterByType DEFLATE text/html text/css text/javascript application/javascript application/json
</IfModule>

# Set proper MIME types
AddType application/wasm .wasm
AddType application/octet-stream .data

# Enable caching
<IfModule mod_expires.c>
    ExpiresActive On
    ExpiresByType image/jpg "access plus 1 year"
    ExpiresByType image/png "access plus 1 year"
    ExpiresByType text/css "access plus 1 month"
    ExpiresByType application/javascript "access plus 1 month"
</IfModule>

# Security headers
Header set X-Content-Type-Options "nosniff"
Header set X-Frame-Options "SAMEORIGIN"
Header set X-XSS-Protection "1; mode=block"
```

### 9. GitHub Pages Deployment
Create `WebBuild/404.html`:
```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>Redirecting...</title>
    <script>
        // GitHub Pages SPA hack
        const path = window.location.pathname;
        window.location.replace('/?redirect=' + path);
    </script>
</head>
<body>
    Redirecting...
</body>
</html>
```

### 10. Performance Optimization Checklist
- [ ] Compress all images (use WebP where supported)
- [ ] Minify CSS and JavaScript
- [ ] Enable gzip compression
- [ ] Implement lazy loading for images
- [ ] Use CDN for static assets
- [ ] Optimize Unity build size
- [ ] Test on slow 3G connection

## Expected Outcomes
- ✅ Landing page loads in <3 seconds
- ✅ Clear value proposition visible immediately
- ✅ One-click game launch
- ✅ Seamless authentication flow
- ✅ Unity game loads smoothly
- ✅ Mobile responsive design
- ✅ SEO optimized for discovery

## Asset Requirements
- Hero gameplay video (30 seconds, <5MB)
- Fallback hero image (1920x1080)
- Game screenshots (3-5)
- Icon set (dragon, AI, rewards)
- Favicon and app icons
- Open Graph image (1200x630)

## Common Issues & Solutions

### Issue: Unity Build Too Large
- Enable compression in build settings
- Use texture compression
- Strip unused code
- Implement asset bundles

### Issue: Slow Loading Time
- Optimize images
- Use loading placeholders
- Implement progressive loading
- Cache static assets

### Issue: Mobile Performance
- Reduce quality settings for mobile
- Implement touch controls
- Test on real devices
- Use viewport meta tag

## Time Estimate: 4-6 hours

## Next Steps
Proceed to Task 13: Level 2 Adaptive AI

---

## Completion Notes
*To be filled after task completion*

**Date Completed**: 
**Time Taken**: 
**Page Load Time**: 
**Unity Load Time**: 
**Mobile Performance**: 
**User Feedback**: 