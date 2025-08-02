using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlasmaDragon.Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        _instance = go.AddComponent<GameManager>();
                    }
                }
                return _instance;
            }
        }

        [Header("Game Settings")]
        [SerializeField] private bool _isPaused = false;
        [SerializeField] private float _gameTime = 0f;

        [Header("Player Stats")]
        [SerializeField] private int _playerScore = 0;
        [SerializeField] private int _tokensEarned = 0;

        public bool IsPaused => _isPaused;
        public float GameTime => _gameTime;
        public int PlayerScore => _playerScore;
        public int TokensEarned => _tokensEarned;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (!_isPaused)
            {
                _gameTime += Time.deltaTime;
            }
        }

        public void PauseGame()
        {
            _isPaused = true;
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            _isPaused = false;
            Time.timeScale = 1f;
        }

        public void AddScore(int points)
        {
            _playerScore += points;
        }

        public void AddTokens(int tokens)
        {
            _tokensEarned += tokens;
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}