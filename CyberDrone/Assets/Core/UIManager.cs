using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    /// <summary>
    /// Manages all UI elements and interactions in the game.
    /// Handles HUD, menus, and UI-related events.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject levelCompletePanel;

        [Header("HUD Elements")]
        [SerializeField] private Slider energyBar;
        [SerializeField] private TextMeshProUGUI altitudeText;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private GameObject lowEnergyWarning;

        [Header("References")]
        [SerializeField] private DroneController playerDrone;
        [SerializeField] private DroneEnergySystem energySystem;

        private void Awake()
        {
            // Find references if not assigned
            if (playerDrone == null)
                playerDrone = FindObjectOfType<DroneController>();

            if (energySystem == null && playerDrone != null)
                energySystem = playerDrone.GetComponent<DroneEnergySystem>();

            // Hide all panels initially
            HideAllPanels();
        }

        private void Start()
        {
            // Set up event listeners
            if (energySystem != null)
            {
                energySystem.OnCriticalEnergy.AddListener(ShowLowEnergyWarning);
                energySystem.OnEnergyRecharged.AddListener(HideLowEnergyWarning);
            }

            // Initialize UI based on initial game state
            UpdateUI(GameManager.Instance.GetComponent<GameManager>().currentState);
        }

        private void Update()
        {
            // Update HUD elements if HUD is active
            if (hudPanel.activeSelf)
            {
                UpdateEnergyBar();
                UpdateFlightInfo();
            }
        }

        /// <summary>
        /// Updates UI based on current game state
        /// </summary>
        public void UpdateUI(GameManager.GameState gameState)
        {
            HideAllPanels();

            switch (gameState)
            {
                case GameManager.GameState.MainMenu:
                    mainMenuPanel.SetActive(true);
                    break;

                case GameManager.GameState.Playing:
                    hudPanel.SetActive(true);
                    break;

                case GameManager.GameState.Paused:
                    hudPanel.SetActive(true);
                    pauseMenuPanel.SetActive(true);
                    break;

                case GameManager.GameState.GameOver:
                    gameOverPanel.SetActive(true);
                    break;

                case GameManager.GameState.LevelComplete:
                    levelCompletePanel.SetActive(true);
                    break;
            }
        }

        /// <summary>
        /// Updates the energy bar in the HUD
        /// </summary>
        private void UpdateEnergyBar()
        {
            if (energySystem != null && energyBar != null)
            {
                energyBar.value = energySystem.EnergyPercentage / 100f;
            }
        }

        /// <summary>
        /// Updates flight information in the HUD (altitude, speed)
        /// </summary>
        private void UpdateFlightInfo()
        {
            if (playerDrone != null)
            {
                // Update altitude text
                if (altitudeText != null)
                {
                    float altitude = playerDrone.transform.position.y;
                    altitudeText.text = $"ALT: {altitude:F1}m";
                }

                // Update speed text
                if (speedText != null)
                {
                    Rigidbody rb = playerDrone.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        float speed = rb.linearVelocity.magnitude;
                    
                        speedText.text = $"SPD: {speed:F1}m/s";
                    }
                }
            }
        }

        /// <summary>
        /// Shows the game over screen
        /// </summary>
        public void ShowGameOver()
        {
            HideAllPanels();
            gameOverPanel.SetActive(true);
        }

        /// <summary>
        /// Shows the level complete screen
        /// </summary>
        public void ShowLevelComplete()
        {
            HideAllPanels();
            levelCompletePanel.SetActive(true);
        }

        /// <summary>
        /// Shows the low energy warning
        /// </summary>
        public void ShowLowEnergyWarning()
        {
            if (lowEnergyWarning != null)
                lowEnergyWarning.SetActive(true);
        }

        /// <summary>
        /// Hides the low energy warning
        /// </summary>
        public void HideLowEnergyWarning()
        {
            if (lowEnergyWarning != null)
                lowEnergyWarning.SetActive(false);
        }

        /// <summary>
        /// Hides all UI panels
        /// </summary>
        private void HideAllPanels()
        {
            mainMenuPanel.SetActive(false);
            hudPanel.SetActive(false);
            pauseMenuPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            levelCompletePanel.SetActive(false);
        }

        #region Button Event Handlers

        public void OnStartGameClicked()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.StartNewGame();
        }

        public void OnResumeClicked()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.TogglePause();
        }

        public void OnMainMenuClicked()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.ReturnToMainMenu();
        }

        public void OnRestartClicked()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.StartNewGame();
        }

        public void OnQuitClicked()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        #endregion
    }
}