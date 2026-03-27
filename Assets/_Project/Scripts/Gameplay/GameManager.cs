using UnityEngine;
using System;

namespace PixelDestruction.Gameplay
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        LevelComplete
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        public GameState CurrentState { get; private set; }

        public event Action<GameState> OnGameStateChanged;

        [Header("UI Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject playingPanel;
        [SerializeField] private GameObject pausedPanel;
        [SerializeField] private GameObject levelCompletePanel;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            UpdateGameState(GameState.MainMenu);
        }

        public void UpdateGameState(GameState newState)
        {
            CurrentState = newState;
            OnGameStateChanged?.Invoke(newState);
            
            switch (newState)
            {
                case GameState.MainMenu:
                    HandleMainMenu();
                    break;
                case GameState.Playing:
                    HandlePlaying();
                    break;
                case GameState.Paused:
                    HandlePaused();
                    break;
                case GameState.LevelComplete:
                    HandleLevelComplete();
                    break;
            }
        }

        private void HideAllPanels()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (playingPanel != null) playingPanel.SetActive(false);
            if (pausedPanel != null) pausedPanel.SetActive(false);
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        }

        private void HandleMainMenu()
        {
            Time.timeScale = 1f; 
            HideAllPanels();
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        }
        
        private void HandlePlaying()
        {
            Time.timeScale = 1f;
            HideAllPanels();
            if (playingPanel != null) playingPanel.SetActive(true);
        }
        
        private void HandlePaused()
        {
            Time.timeScale = 0f; 
            HideAllPanels();
            
            if (playingPanel != null) playingPanel.SetActive(true);
            if (pausedPanel != null) pausedPanel.SetActive(true);
        }
        
        private void HandleLevelComplete()
        {
            Time.timeScale = 0f;
            HideAllPanels();
            if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
        }

        public void StartGame()
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.StartFromFirstLevel();
            }
            UpdateGameState(GameState.Playing);
        }

        public void PauseGame()
        {
            UpdateGameState(GameState.Paused);
        }

        public void ResumeGame()
        {
            UpdateGameState(GameState.Playing);
        }

        public void NextLevel()
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.NextLevel();
            }
        }

        public void RestartLevel()
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevelIndex);
                UpdateGameState(GameState.Playing);
            }
        }

        public void BackToMainMenu()
        {
            UpdateGameState(GameState.MainMenu);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
