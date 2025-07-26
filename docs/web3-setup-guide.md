# Plasma Dragon - Web3 Integration Setup Guide

## Overview

This guide provides step-by-step instructions for setting up the complete Web3 infrastructure for Plasma Dragon, including Supabase authentication, Solana blockchain integration, and PLASMA token rewards.

## Architecture Overview

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Unity Game    │    │    Supabase      │    │   Solana RPC    │
│                 │    │   (Database)     │    │   (Blockchain)  │
│ • Authentication│◄──►│ • User Profiles  │    │ • Wallet Mgmt   │
│ • Token Rewards │    │ • Game Progress  │    │ • Balance Check │
│ • Wallet UI     │    │ • Token Tracking │    │ • Transactions  │
└─────────────────┘    └──────────────────┘    └─────────────────┘
         │                        │
         └──────────────┬─────────┘
                        ▼
                ┌──────────────────┐
                │   n8n Webhook    │
                │ (Token Distribution)│
                └──────────────────┘
```

## Prerequisites

### Development Environment
- Unity 2022.3 LTS or newer
- Git (for package dependencies)
- Text editor for configuration files

### Service Accounts Required
1. **Supabase Account** (Free tier available)
2. **Solana Devnet Access** (Free)
3. **n8n Instance** (Optional, for token distribution)

## Step 1: Environment Configuration

### 1.1 Create Environment File

Copy `.env.example` to `.env` in your project root:

```bash
cp .env.example .env
```

### 1.2 Configure Environment Variables

Edit `.env` with your actual values:

```env
# Supabase Configuration
SUPABASE_URL=https://your-project-id.supabase.co
SUPABASE_ANON_KEY=your-public-anon-key-here
SUPABASE_SERVICE_ROLE_KEY=your-service-role-key-here

# Solana Configuration  
SOLANA_RPC_URL=https://api.devnet.solana.com
SOLANA_NETWORK=devnet

# Token Configuration
PLASMA_TOKEN_SYMBOL=PLASMA
LEVEL_1_REWARD=5
LEVEL_2_REWARD=8
LEVEL_3_REWARD=10
PERFECT_RUN_BONUS=3

# Backend Integration (Optional)
N8N_WEBHOOK_URL=https://your-n8n-instance.com/webhook/token-distribution
```

## Step 2: Supabase Setup

### 2.1 Create Supabase Project

1. Visit [supabase.com](https://supabase.com)
2. Click "Start your project"
3. Create a new organization (if needed)
4. Create a new project:
   - **Name**: `plasma-dragon-game`
   - **Database Password**: Generate strong password
   - **Region**: Choose nearest to your users

### 2.2 Get Supabase Credentials

1. Go to **Settings** → **API**
2. Copy the following values:
   - **Project URL**: `SUPABASE_URL`
   - **anon/public key**: `SUPABASE_ANON_KEY`
   - **service_role key**: `SUPABASE_SERVICE_ROLE_KEY`

### 2.3 Create Database Schema

Execute this SQL in the Supabase SQL Editor:

```sql
-- Create player profiles table
CREATE TABLE player_profiles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email TEXT UNIQUE NOT NULL,
    display_name TEXT NOT NULL,
    wallet_address TEXT,
    
    -- Token & Rewards
    token_balance INTEGER DEFAULT 0,
    total_tokens_earned INTEGER DEFAULT 0,
    pending_rewards INTEGER DEFAULT 0,
    
    -- Game Progress
    games_played INTEGER DEFAULT 0,
    levels_completed INTEGER DEFAULT 0,
    high_score INTEGER DEFAULT 0,
    perfect_runs INTEGER DEFAULT 0,
    bosses_defeated INTEGER DEFAULT 0,
    
    -- Player Preferences
    music_volume REAL DEFAULT 0.7,
    sfx_volume REAL DEFAULT 0.8,
    graphics_quality INTEGER DEFAULT 1,
    
    -- Analytics & Metadata
    last_login TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    total_playtime_minutes INTEGER DEFAULT 0,
    device_info TEXT,
    game_version TEXT,
    
    -- Timestamps
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Create token rewards tracking table
CREATE TABLE token_rewards (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    player_id UUID REFERENCES player_profiles(id),
    amount INTEGER NOT NULL,
    reason TEXT NOT NULL,
    status TEXT DEFAULT 'pending',
    transaction_hash TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    processed_at TIMESTAMP WITH TIME ZONE
);

-- Create indexes for performance
CREATE INDEX idx_player_profiles_email ON player_profiles(email);
CREATE INDEX idx_player_profiles_wallet ON player_profiles(wallet_address);
CREATE INDEX idx_token_rewards_player ON token_rewards(player_id);
CREATE INDEX idx_token_rewards_status ON token_rewards(status);

-- Enable Row Level Security
ALTER TABLE player_profiles ENABLE ROW LEVEL SECURITY;
ALTER TABLE token_rewards ENABLE ROW LEVEL SECURITY;

-- Create policies
CREATE POLICY "Users can read own profile" ON player_profiles
    FOR SELECT USING (auth.uid() = id);

CREATE POLICY "Users can update own profile" ON player_profiles
    FOR UPDATE USING (auth.uid() = id);

CREATE POLICY "Users can read own rewards" ON token_rewards
    FOR SELECT USING (player_id = auth.uid());
```

### 2.4 Configure Authentication

1. Go to **Authentication** → **Settings**
2. **Site URL**: `http://localhost:3000` (for development)
3. **Redirect URLs**: Add your game's domain when deploying
4. **Email Templates**: Customize if needed

## Step 3: Solana Integration

### 3.1 Solana Devnet Setup

For development, we use Solana's Devnet (free testnet):

- **RPC URL**: `https://api.devnet.solana.com`
- **Network**: `devnet`
- **Faucet**: Available for free SOL tokens

### 3.2 Token Creation (Optional)

To create a custom PLASMA token on devnet:

```bash
# Install Solana CLI
sh -c "$(curl -sSfL https://release.solana.com/v1.16.0/install)"

# Set devnet cluster
solana config set --url https://api.devnet.solana.com

# Create wallet
solana-keygen new

# Get devnet SOL
solana airdrop 2

# Create token mint
spl-token create-token --decimals 9
# Copy the token mint address to SOLANA_TOKEN_MINT_ADDRESS
```

### 3.3 Unity SDK Configuration

The Solana Unity SDK is automatically installed via `manifest.json`. Configuration is handled by the `SolanaManager` class using environment variables.

## Step 4: Unity Integration

### 4.1 Verify Package Installation

Check that these packages are installed in **Package Manager**:
- `com.justinpbarnett.unity-mcp` (Unity MCP Bridge)
- `io.supabase.supabase-csharp` (Supabase)  
- `com.solana.web3.unity` (Solana Unity)

### 4.2 Create Manager GameObjects

Create these GameObjects in your main scene:

1. **EnvironmentConfig** (Empty GameObject)
2. **SupabaseManager** (Add SupabaseManager script)
3. **SolanaManager** (Add SolanaManager script)
4. **TokenRewardSystem** (Add TokenRewardSystem script)

### 4.3 Configure UI References

For scenes with UI, ensure the following are connected:
- **SeamlessOnboarding**: Login panels, input fields, buttons
- **TokenRewardSystem**: Reward popup prefab, UI canvas
- **RewardPopupUI**: Text components, animations

## Step 5: Testing & Validation

### 5.1 Use Integration Tester

1. Add `Web3IntegrationTester` script to a GameObject
2. Set **Test Email** and **Test Password**
3. Enable **Run Tests On Start**
4. Play the scene and check console output

### 5.2 Manual Testing Checklist

- [ ] Environment config loads successfully
- [ ] Supabase connection established
- [ ] User registration works
- [ ] User login works  
- [ ] Solana wallet created automatically
- [ ] Wallet address displayed correctly
- [ ] Token rewards appear in UI
- [ ] Balance persists between sessions

### 5.3 Common Issues & Solutions

**Issue**: Supabase connection fails
- **Solution**: Check URL and API keys in `.env`
- **Solution**: Verify RLS policies allow access

**Issue**: Solana wallet creation fails
- **Solution**: Check internet connection to devnet
- **Solution**: Try different RPC endpoint

**Issue**: Token rewards not appearing
- **Solution**: Check TokenRewardSystem is active
- **Solution**: Verify UI references are connected

## Step 6: Production Deployment

### 6.1 Security Considerations

**Environment Variables**:
- Use proper secret management (not `.env` files)
- Rotate API keys regularly
- Use service role keys only for backend operations

**Wallet Security**:
- Implement proper mnemonic encryption
- Consider hardware wallet integration
- Add transaction confirmation dialogs

### 6.2 Mainnet Migration

To switch to Solana mainnet:

1. Update environment variables:
   ```env
   SOLANA_RPC_URL=https://api.mainnet-beta.solana.com
   SOLANA_NETWORK=mainnet-beta
   ```

2. Create production token mint
3. Fund token distribution wallet
4. Update Supabase project settings
5. Test thoroughly on testnet first

### 6.3 Monitoring & Analytics

**Supabase Dashboard**:
- Monitor user registrations
- Track token distribution
- Analyze player behavior

**Custom Analytics**:
- Track wallet creation success rates
- Monitor transaction failures
- Log user onboarding completion

## Step 7: n8n Backend Integration (Optional)

### 7.1 n8n Workflow Setup

Create an n8n workflow for token distribution:

1. **Webhook Trigger**: Receive game rewards
2. **Data Validation**: Verify reward legitimacy
3. **Supabase Node**: Update user balance
4. **Solana Node**: Queue blockchain transaction
5. **Error Handling**: Retry failed operations

### 7.2 Webhook Configuration

Configure webhook URL in environment:
```env
N8N_WEBHOOK_URL=https://your-n8n.app.n8n.cloud/webhook/plasma-rewards
```

## Support & Resources

### Documentation Links
- [Supabase Docs](https://supabase.com/docs)
- [Solana Web3.js Docs](https://docs.solana.com/developing/clients/javascript-api)
- [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html)

### Community Resources
- [Supabase Discord](https://discord.supabase.com/)
- [Solana Developer Discord](https://discord.gg/solana)
- [Unity Forums](https://forum.unity.com/)

### Troubleshooting
For technical issues:
1. Check Unity Console for error messages
2. Verify all environment variables are set
3. Test each component individually
4. Use the Integration Tester for diagnostics

---

**Last Updated**: 2025-01-21  
**Version**: 1.0.0  
**Tested With**: Unity 2022.3 LTS, Supabase v2, Solana Web3 v1.16