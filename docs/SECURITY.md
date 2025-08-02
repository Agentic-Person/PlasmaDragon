# PlasmaDragon Security Guidelines

⚠️ **CRITICAL**: This project contains sensitive blockchain and authentication data that must NEVER be committed to version control.

## 🚨 Protected Files (NEVER COMMIT!)

### Blockchain Private Data
- `Solana` - Contains token creation signatures and keypair references
- `.Solana` - Contains private key paths and transaction signatures  
- `temp-mint.json` - Temporary mint authority keys
- `devnet.json` / `mainnet.json` - Solana wallet keypairs
- Any `*-keypair.json` files
- `mnemonic.txt` or seed phrase files

### Environment & Configuration
- `.env` - Production API keys and secrets
- `.env.local` / `.env.production` - Environment-specific secrets
- `*-secrets.json` - Service account keys
- `*-config.private` - Private configuration files

### Application Data
- `memories.json` - May contain API keys in conversation history
- `*.mem.*` - Memory dumps that might contain sensitive data
- Any `*.wallet` or `*.key` files

## ✅ What IS Safe to Commit

### Template & Documentation Files
- `.env.example` - Template with placeholder values
- `.env.template` - Configuration template
- Unity scripts (`SolanaManager.cs`, etc.) - Code without secrets
- Documentation files (`*.md`)
- Unity meta files for scripts

### Configuration Templates
```env
# ✅ SAFE - .env.example
SUPABASE_URL=https://your-project-id.supabase.co
SOLANA_TOKEN_MINT_ADDRESS=your-token-mint-here

# ❌ NEVER COMMIT - .env
SUPABASE_URL=https://qwhtepzdsomhsqohzchv.supabase.co
SUPABASE_ANON_KEY=eyJhbGciOiJIUzI1NiIs...
```

## 🔒 Current Protection Status

### Git Ignore Patterns Applied
```gitignore
# Solana blockchain sensitive files (CRITICAL!)
Solana
.Solana
*.solana
solana-*
temp-mint.json
devnet.json
mainnet.json

# Environment and configuration secrets
.env
.env.local
.env.production
.env.development
.env.staging
*-secrets.json
*-keys.json

# Wallet and private key files
wallet.json
*.wallet
*.key
*.pem
*.seed
mnemonic.txt
keypair.json
*-keypair.json

# Memory dumps that might contain sensitive data
memories.json
*.mem.*
```

### Verification Status
- ✅ Sensitive files properly ignored by git
- ✅ Template files still trackable  
- ✅ Unity scripts and code remain version controlled
- ✅ Documentation and guides safely committed

## 🛡️ Security Best Practices

### For Developers
1. **Never bypass .gitignore** - Don't use `git add -f` on sensitive files
2. **Check before committing** - Run `git status` to verify no sensitive data
3. **Use templates** - Copy `.env.example` to `.env` and fill in real values
4. **Keep secrets local** - API keys and private keys stay on your machine only

### For Team Setup
1. **Clone repository** - Get code without sensitive data
2. **Copy configuration template**:
   ```bash
   cp .env.example .env
   # Edit .env with your actual API keys
   ```
3. **Generate your own wallets** - Don't share private keys between team members
4. **Use development/testnet** - Never use mainnet keys for development

### For Production Deployment
1. **Environment variables** - Use platform-specific secret management
2. **Separate wallets** - Development ≠ Production wallets
3. **Key rotation** - Regularly update API keys and tokens
4. **Access control** - Limit who has access to production secrets

## 🚨 If Sensitive Data Was Committed

### Immediate Actions
1. **DO NOT just delete the file** - Git history still contains it
2. **Remove from git history**:
   ```bash
   git rm --cached filename
   git commit -m "Remove sensitive file from tracking"
   ```
3. **Rotate all exposed secrets**:
   - Generate new API keys
   - Create new Solana wallets  
   - Update all services with new credentials
4. **Force push** to remove from remote history (if using git)

### Prevention
- Use `git commit --dry-run` to preview changes
- Set up pre-commit hooks to scan for sensitive patterns
- Regular security audits of repository contents

## 🔐 Specific Security Concerns

### PLASMA Token Security
- **Token Mint Authority**: `3UDziHJzxc7yLthFFdXYwRTPYvGD5i5UW7EtcTndwuA7` (public - safe)
- **Private Keys**: Never share or commit wallet private keys
- **Seed Phrases**: Each developer should have their own test wallet
- **Production Tokens**: Use separate mainnet wallet for production

### Supabase Security  
- **Public Keys**: Anon key is safe in client code
- **Service Role Key**: Must be kept secret and server-side only
- **Database Access**: RLS policies prevent unauthorized access

### API Key Management
- **Claude API**: Personal API keys, never shared
- **n8n Credentials**: Production webhooks use separate keys
- **Development vs Production**: Always use separate credentials

## 📋 Security Checklist

Before any commit:
- [ ] Run `git status` and verify no sensitive files staged
- [ ] Check that only code, documentation, and templates are committed
- [ ] Verify `.env` files are not in commit (only `.env.example`)
- [ ] Confirm no private keys, mnemonics, or API secrets in files
- [ ] Test that application works with template configurations

## 🆘 Emergency Contacts

If sensitive data is accidentally committed:
1. **Immediately rotate all affected credentials**
2. **Remove from git history using proper git commands**  
3. **Update team about security incident**
4. **Review security practices to prevent recurrence**

## 📚 Additional Resources

- [Git Secrets Prevention](https://git-secret.io/)
- [Solana Security Best Practices](https://docs.solana.com/developing/programming-model/security)
- [Environment Variable Security](https://blog.gitguardian.com/secrets-in-environment-variables/)

---

**Remember**: When in doubt, DON'T commit it. It's easier to add a file later than to remove sensitive data from git history.

**Last Updated**: 2025-01-21  
**Status**: ✅ All sensitive files protected