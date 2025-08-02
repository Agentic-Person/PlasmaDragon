using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

namespace PlasmaDragon.Combat
{
    [System.Serializable]
    public class DifficultyLevel
    {
        [Header("📊 Difficulty Configuration")]
        public string levelName = "Normal";
        public int difficultyRating = 5; // 1-10 scale
        
        [Header("📦 Combat Prefabs")]
        public GameObject[] enemyEncounterPrefabs;
        public GameObject[] towerEncounterPrefabs;
        public GameObject[] bossEncounterPrefabs;
        
        [Header("⚙️ Difficulty Modifiers")]
        public float enemyHealthMultiplier = 1f;
        public float enemyDamageMultiplier = 1f;
        public float enemySpeedMultiplier = 1f;
        public float towerAccuracyBonus = 0f; // 0-1 scale
        public float aiDecisionSpeedBonus = 0f; // Faster AI decisions
        
        [Header("🎯 Spawn Configuration")]
        public int maxEnemies = 10;
        public int maxTowers = 3;
        public float spawnDelay = 2f;
        public bool enableSmartTowers = false;
    }
    
    [System.Serializable]
    public class PlayerPerformanceData
    {
        public float survivalTime = 0f;
        public int enemiesDefeated = 0;
        public int towersDestroyed = 0;
        public int bossesDefeated = 0;
        public float damageReceived = 0f;
        public float accuracyRating = 0f; // Player's hit accuracy
        public float evasionRating = 0f;  // How well player avoids damage
        public DateTime sessionStart;
        public List<float> levelCompletionTimes = new List<float>();
        
        public float GetOverallPerformance()
        {
            // Calculate overall performance score (0-1 scale)
            float score = 0f;
            
            // Survival factor
            score += Mathf.Clamp01(survivalTime / 300f) * 0.3f; // 5 minutes = 1.0
            
            // Combat effectiveness
            float combatScore = (enemiesDefeated * 0.1f + towersDestroyed * 0.3f + bossesDefeated * 1f);
            score += Mathf.Clamp01(combatScore / 10f) * 0.4f;
            
            // Skill ratings
            score += accuracyRating * 0.15f;
            score += evasionRating * 0.15f;
            
            return Mathf.Clamp01(score);
        }
    }
    
    public class DynamicDifficultyManager : MonoBehaviour
    {
        [Header("📊 DYNAMIC DIFFICULTY SYSTEM")]
        public DifficultyLevel[] difficultyLevels;
        public int startingDifficultyIndex = 1; // Start at "Normal"
        
        [Header("⚙️ Adaptation Settings")]
        public bool enableDynamicDifficulty = true;
        public float adaptationInterval = 30f; // Check every 30 seconds
        public float performanceWindowSize = 60f; // Consider last 60 seconds
        
        [Header("🎯 Performance Thresholds")]
        [Range(0f, 1f)] public float difficultyIncreaseThreshold = 0.75f;
        [Range(0f, 1f)] public float difficultyDecreaseThreshold = 0.35f;
        public int maxDifficultyChangesPerLevel = 2;
        
        [Header("📦 Spawn Containers")]
        public Transform enemySpawnContainer;
        public Transform towerSpawnContainer;
        public Transform bossSpawnContainer;
        
        [Header("🔍 Debug Info")]
        public bool showDebugInfo = true;
        public bool logDifficultyChanges = true;
        
        // Current state
        public int currentDifficultyIndex { get; private set; }
        public DifficultyLevel currentDifficulty => difficultyLevels[currentDifficultyIndex];
        public PlayerPerformanceData playerPerformance { get; private set; } = new PlayerPerformanceData();
        
        // Tracking variables
        private float lastAdaptationTime;
        private int difficultyChangesThisLevel = 0;
        private List<GameObject> activeEnemies = new List<GameObject>();
        private List<GameObject> activeTowers = new List<GameObject>();
        private List<GameObject> activeBosses = new List<GameObject>();
        
        // Events
        public System.Action<int, DifficultyLevel> OnDifficultyChanged;
        public System.Action<PlayerPerformanceData> OnPerformanceUpdated;
        
        void Start()
        {
            InitializeDifficultyManager();
        }
        
        void InitializeDifficultyManager()
        {
            // Validate difficulty levels
            if (difficultyLevels == null || difficultyLevels.Length == 0)
            {
                Debug.LogError("No difficulty levels configured!");
                return;
            }
            
            // Set starting difficulty
            currentDifficultyIndex = Mathf.Clamp(startingDifficultyIndex, 0, difficultyLevels.Length - 1);
            
            // Initialize player performance tracking
            playerPerformance.sessionStart = System.DateTime.UtcNow;
            
            // Find spawn containers if not assigned
            if (enemySpawnContainer == null)
                enemySpawnContainer = GameObject.Find("👥 Enemy Soldiers")?.transform;
            if (towerSpawnContainer == null)
                towerSpawnContainer = GameObject.Find("🏰 Tower Defense System")?.transform;
            if (bossSpawnContainer == null)
                bossSpawnContainer = GameObject.Find("🎯 AI_BOSS_ARENA")?.transform;
            
            Debug.Log($"📊 Dynamic Difficulty Manager initialized - Starting Level: {currentDifficulty.levelName}");
        }
        
        void Update()
        {
            if (!enableDynamicDifficulty) return;
            
            // Update player performance
            UpdatePlayerPerformance();
            
            // Check for difficulty adaptation
            if (Time.time - lastAdaptationTime >= adaptationInterval)
            {
                EvaluateDifficultyAdjustment();
                lastAdaptationTime = Time.time;
            }
            
            // Clean up destroyed objects
            CleanupDestroyedObjects();
        }
        
        void UpdatePlayerPerformance()
        {
            // Update survival time
            playerPerformance.survivalTime += Time.deltaTime;
            
            // TODO: These would be updated by other systems
            // playerPerformance.damageReceived updated by player health system
            // playerPerformance.accuracyRating updated by weapon system
            // playerPerformance.evasionRating updated by damage system
            
            // Trigger performance update event
            OnPerformanceUpdated?.Invoke(playerPerformance);
        }
        
        void EvaluateDifficultyAdjustment()
        {
            if (difficultyChangesThisLevel >= maxDifficultyChangesPerLevel) return;
            
            float currentPerformance = playerPerformance.GetOverallPerformance();
            
            if (showDebugInfo)
                Debug.Log($"📊 Player Performance: {currentPerformance:F2} (Threshold: ↑{difficultyIncreaseThreshold:F2} ↓{difficultyDecreaseThreshold:F2})");
            
            // Check if we should increase difficulty
            if (currentPerformance >= difficultyIncreaseThreshold && 
                currentDifficultyIndex < difficultyLevels.Length - 1)
            {
                IncreaseDifficulty();
            }
            // Check if we should decrease difficulty
            else if (currentPerformance <= difficultyDecreaseThreshold && 
                     currentDifficultyIndex > 0)
            {
                DecreaseDifficulty();
            }
        }
        
        public void IncreaseDifficulty()
        {
            int newIndex = Mathf.Min(currentDifficultyIndex + 1, difficultyLevels.Length - 1);
            ChangeDifficulty(newIndex);
        }
        
        public void DecreaseDifficulty()
        {
            int newIndex = Mathf.Max(currentDifficultyIndex - 1, 0);
            ChangeDifficulty(newIndex);
        }
        
        void ChangeDifficulty(int newDifficultyIndex)
        {
            if (newDifficultyIndex == currentDifficultyIndex) return;
            
            int oldIndex = currentDifficultyIndex;
            currentDifficultyIndex = newDifficultyIndex;
            difficultyChangesThisLevel++;
            
            if (logDifficultyChanges)
            {
                string direction = newDifficultyIndex > oldIndex ? "INCREASED" : "DECREASED";
                Debug.Log($"📊 Difficulty {direction}: {difficultyLevels[oldIndex].levelName} → {currentDifficulty.levelName}");
            }
            
            // Apply new difficulty
            StartCoroutine(TransitionToDifficulty());
            
            // Trigger difficulty change event
            OnDifficultyChanged?.Invoke(currentDifficultyIndex, currentDifficulty);
        }
        
        IEnumerator TransitionToDifficulty()
        {
            // Smooth transition to new difficulty
            yield return new WaitForSeconds(1f);
            
            // Despawn old encounters
            DespawnCurrentEncounters();
            
            yield return new WaitForSeconds(0.5f);
            
            // Spawn new encounters based on difficulty
            SpawnDifficultyEncounters();
            
            // Apply difficulty modifiers to existing entities
            ApplyDifficultyModifiers();
        }
        
        void DespawnCurrentEncounters()
        {
            // Despawn enemies gradually
            foreach (GameObject enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    // Add despawn effect here
                    Destroy(enemy);
                }
            }
            activeEnemies.Clear();
            
            // Keep towers but modify them
            // Keep bosses but modify them
        }
        
        void SpawnDifficultyEncounters()
        {
            var difficulty = currentDifficulty;
            
            // Spawn enemy encounters
            if (difficulty.enemyEncounterPrefabs != null && difficulty.enemyEncounterPrefabs.Length > 0)
            {
                StartCoroutine(SpawnEnemiesGradually(difficulty));
            }
            
            // Spawn additional towers if needed
            if (difficulty.towerEncounterPrefabs != null && difficulty.towerEncounterPrefabs.Length > 0)
            {
                SpawnTowerEncounters(difficulty);
            }
            
            // Spawn boss encounters
            if (difficulty.bossEncounterPrefabs != null && difficulty.bossEncounterPrefabs.Length > 0)
            {
                SpawnBossEncounters(difficulty);
            }
        }
        
        IEnumerator SpawnEnemiesGradually(DifficultyLevel difficulty)
        {
            int enemiesToSpawn = Mathf.Min(difficulty.maxEnemies, difficulty.enemyEncounterPrefabs.Length);
            
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                if (difficulty.enemyEncounterPrefabs[i] != null)
                {
                    Vector3 spawnPos = GetRandomSpawnPosition(enemySpawnContainer);
                    GameObject enemy = Instantiate(difficulty.enemyEncounterPrefabs[i], spawnPos, Quaternion.identity, enemySpawnContainer);
                    activeEnemies.Add(enemy);
                    
                    // Apply difficulty modifiers
                    ApplyEnemyModifiers(enemy, difficulty);
                }
                
                yield return new WaitForSeconds(difficulty.spawnDelay);
            }
        }
        
        void SpawnTowerEncounters(DifficultyLevel difficulty)
        {
            int towersToSpawn = Mathf.Min(difficulty.maxTowers, difficulty.towerEncounterPrefabs.Length);
            
            for (int i = 0; i < towersToSpawn; i++)
            {
                if (difficulty.towerEncounterPrefabs[i] != null)
                {
                    Vector3 spawnPos = GetRandomSpawnPosition(towerSpawnContainer);
                    GameObject tower = Instantiate(difficulty.towerEncounterPrefabs[i], spawnPos, Quaternion.identity, towerSpawnContainer);
                    activeTowers.Add(tower);
                    
                    // Apply difficulty modifiers
                    ApplyTowerModifiers(tower, difficulty);
                }
            }
        }
        
        void SpawnBossEncounters(DifficultyLevel difficulty)
        {
            foreach (GameObject bossPrefab in difficulty.bossEncounterPrefabs)
            {
                if (bossPrefab != null)
                {
                    Vector3 spawnPos = GetRandomSpawnPosition(bossSpawnContainer);
                    GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity, bossSpawnContainer);
                    activeBosses.Add(boss);
                    
                    // Apply difficulty modifiers
                    ApplyBossModifiers(boss, difficulty);
                }
            }
        }
        
        Vector3 GetRandomSpawnPosition(Transform container)
        {
            if (container == null) return Vector3.zero;
            
            // Generate random position around container
            Vector3 basePos = container.position;
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * 20f;
            randomOffset.y = Mathf.Abs(randomOffset.y); // Keep above ground
            
            return basePos + randomOffset;
        }
        
        void ApplyDifficultyModifiers()
        {
            var difficulty = currentDifficulty;
            
            // Apply modifiers to all active entities
            foreach (GameObject enemy in activeEnemies)
            {
                if (enemy != null) ApplyEnemyModifiers(enemy, difficulty);
            }
            
            foreach (GameObject tower in activeTowers)
            {
                if (tower != null) ApplyTowerModifiers(tower, difficulty);
            }
            
            foreach (GameObject boss in activeBosses)
            {
                if (boss != null) ApplyBossModifiers(boss, difficulty);
            }
        }
        
        void ApplyEnemyModifiers(GameObject enemy, DifficultyLevel difficulty)
        {
            var enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                // Apply health modifier
                float newMaxHealth = enemyAI.stats.maxHealth * difficulty.enemyHealthMultiplier;
                enemyAI.stats.maxHealth = newMaxHealth;
                
                // Apply damage modifier
                enemyAI.stats.attackDamage *= difficulty.enemyDamageMultiplier;
                
                // Apply speed modifier
                enemyAI.stats.moveSpeed *= difficulty.enemySpeedMultiplier;
                enemyAI.stats.runSpeed *= difficulty.enemySpeedMultiplier;
            }
        }
        
        void ApplyTowerModifiers(GameObject tower, DifficultyLevel difficulty)
        {
            // Apply to regular towers
            var towerSystem = tower.GetComponent<TowerDefenseSystem>();
            if (towerSystem != null)
            {
                towerSystem.stats.fireRate *= (1f + difficulty.towerAccuracyBonus);
            }
            
            // Apply to smart towers
            var smartTower = tower.GetComponent<SmartTowerSystem>();
            if (smartTower != null)
            {
                smartTower.stats.predictionAccuracy += difficulty.towerAccuracyBonus;
                smartTower.stats.adaptationInterval = Mathf.Max(1f, smartTower.stats.adaptationInterval - difficulty.aiDecisionSpeedBonus);
            }
        }
        
        void ApplyBossModifiers(GameObject boss, DifficultyLevel difficulty)
        {
            var bossAI = boss.GetComponent<AIBossController>();
            if (bossAI != null)
            {
                // Apply health modifier
                bossAI.stats.maxHealth *= difficulty.enemyHealthMultiplier;
                
                // Apply damage modifier
                bossAI.stats.primaryAttackDamage *= difficulty.enemyDamageMultiplier;
                bossAI.stats.specialAttackDamage *= difficulty.enemyDamageMultiplier;
                
                // Apply AI speed modifier
                bossAI.stats.aiDecisionInterval = Mathf.Max(3f, bossAI.stats.aiDecisionInterval - difficulty.aiDecisionSpeedBonus);
            }
        }
        
        void CleanupDestroyedObjects()
        {
            activeEnemies.RemoveAll(enemy => enemy == null);
            activeTowers.RemoveAll(tower => tower == null);
            activeBosses.RemoveAll(boss => boss == null);
        }
        
        // Public methods for other systems to report events
        public void ReportEnemyDefeated()
        {
            playerPerformance.enemiesDefeated++;
        }
        
        public void ReportTowerDestroyed()
        {
            playerPerformance.towersDestroyed++;
        }
        
        public void ReportBossDefeated()
        {
            playerPerformance.bossesDefeated++;
        }
        
        public void ReportDamageReceived(float damage)
        {
            playerPerformance.damageReceived += damage;
        }
        
        public void ReportAccuracyRating(float accuracy)
        {
            playerPerformance.accuracyRating = accuracy;
        }
        
        public void ReportEvasionRating(float evasion)
        {
            playerPerformance.evasionRating = evasion;
        }
        
        public void StartNewLevel()
        {
            // Reset per-level tracking
            difficultyChangesThisLevel = 0;
            playerPerformance.levelCompletionTimes.Add(playerPerformance.survivalTime);
            playerPerformance.survivalTime = 0f;
        }
        
        void OnDrawGizmosSelected()
        {
            if (!showDebugInfo) return;
            
            // Draw spawn areas
            if (enemySpawnContainer != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(enemySpawnContainer.position, 20f);
            }
            
            if (towerSpawnContainer != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(towerSpawnContainer.position, 15f);
            }
            
            if (bossSpawnContainer != null)
            {
                Gizmos.color = Color.purple;
                Gizmos.DrawWireSphere(bossSpawnContainer.position, 10f);
            }
        }
    }
} 