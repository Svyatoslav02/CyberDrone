using UnityEngine;
using UnityEngine.SceneManagement;

namespace DroneGame.Core
{
    /// <summary>
    /// Central manager for game state and core functionality.
    /// Handles game initialization, state transitions, and global game events.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private bool isPaused = false;
        [SerializeField] private GameState currentState = GameState.MainMenu;

        [Header("References")]
        [SerializeField] private DroneController playerDrone;
        [SerializeField] private UIManager uiManager;

        // Singleton instance
        public static GameManager Instance { get; private set; }

        // Game state enum
        public enum GameState
        {
            MainMenu,
            Playing,
            Paused,
            GameOver,
            LevelComplete
        }

        private void Awake()
        {
            // Singleton pattern implementation
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Find references if not assigned
            if (playerDrone == null)
                playerDrone = FindObjectOfType<DroneController>();

            if (uiManager == null)
                uiManager = FindObjectOfType<UIManager>();
        }

        private void Start()
        {
            // Initialize game
            SetGameState(currentState);
        }

        private void Update()
        {
            // Handle pause input
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        /// <summary>
        /// Sets the game state and handles related actions
        /// </summary>
        public void SetGameState(GameState newState)
        {
            currentState = newState;

            switch (currentState)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    isPaused = false;
                    break;

                case GameState.Playing:
                    Time.timeScale = 1f;
                    isPaused = false;
                    break;

                case GameState.Paused:
                    Time.timeScale = 0f;
                    isPaused = true;
                    break;

                case GameState.GameOver:
                    Time.timeScale = 0f;
                    isPaused = true;
                    // Show game over screen
                    if (uiManager != null)
                        uiManager.ShowGameOver();
                    break;

                case GameState.LevelComplete:
                    Time.timeScale = 1f;
                    isPaused = false;
                    // Show level complete screen
                    if (uiManager != null)
                        uiManager.ShowLevelComplete();
                    break;
            }

            // Update UI
            if (uiManager != null)
                uiManager.UpdateUI(currentState);
        }

        /// <summary>
        /// Toggles game pause state
        /// </summary>
        public void TogglePause()
        {
            if (currentState == GameState.Playing)
            {
                SetGameState(GameState.Paused);
            }
            else if (currentState == GameState.Paused)
            {
                SetGameState(GameState.Playing);
            }
        }

        /// <summary>
        /// Starts a new game
        /// </summary>
        public void StartNewGame()
        {
            SceneManager.LoadScene("GameLevel");
            SetGameState(GameState.Playing);
        }

        /// <summary>
        /// Returns to main menu
        /// </summary>
        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
            SetGameState(GameState.MainMenu);
        }

        /// <summary>
        /// Handles drone crash or destruction
        /// </summary>
        public void OnDroneCrashed()
        {
            SetGameState(GameState.GameOver);
        }

        /// <summary>
        /// Completes the current level
        /// </summary>
        public void CompleteLevel()
        {
            SetGameState(GameState.LevelComplete);
        }
    }
}