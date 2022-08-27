using Audio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class GameUi : BaseUi
    {
        [Header("Game UI")] public GameObject gamePausePanel;
        public GameObject optionsPanel, gameOverPanel, gameStuckPanel;
        public Button endTurnButton, retryLevelButton;
        private FmodMixer fmodMixer;
        private StudioEventEmitter gameSong;
        public string sceneToLoad;

        private void Start()
        {
            if (sceneToLoad == "")
                Debug.LogError("GameUi: sceneToLoad not set! Finishing level will crash the game.");

            fmodMixer = GetComponent<FmodMixer>();
            gameSong = GetComponent<StudioEventEmitter>();
            Entities.Player _player = FindObjectOfType<Entities.Player>();
            endTurnButton.onClick.AddListener(_player.WaitAfterKillingThenEndTurn);
            retryLevelButton.onClick.AddListener(ResetCurrentLevel);
            gameSong.Play();
        }

        private void Update()
        {
            CheckKeyInputs();
        }

        private void CheckKeyInputs()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Pause if pause panel isn't open, resume if it is open
                if (!optionsPanel.activeInHierarchy)
                {
                    GameIsPaused(!gamePausePanel.activeInHierarchy);
                }
                else
                {
                    optionsPanel.SetActive(!optionsPanel.activeInHierarchy);
                }
            }
        }

        private void GameIsPaused(bool intent)
        {
            // Show or hide pause panel and set timescale
            gameSong.SetParameter("Menu", intent ? 1 : 0);
            gamePausePanel.SetActive(intent);
            Time.timeScale = intent ? 0 : 1;
            fmodMixer.FindAllSfxAndPlayPause(gameIsPaused: intent);
        }

        public void ResumeGame()
        {
            GameIsPaused(false);
        }

        public void ResetCurrentLevel()
        {
            gameSong.Stop();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Time.timeScale = 1;
        }
        
        public void ToggleOptionsPanel()
        {
            optionsPanel.SetActive(!optionsPanel.activeInHierarchy);
        }

        public void LoadNextScene()
        {
            gameSong.Stop();
            SceneManager.LoadScene(sceneToLoad);
            Time.timeScale = 1;
        }

        public void ExitGameFromPause()
        {
            fmodMixer.KillEverySound();
            SceneManager.LoadScene("MainMenu");
            Time.timeScale = 1;
        }

        public void DebugSuperSpeed()
        {
            if (Time.timeScale == 1)
                Time.timeScale = 5;
            else
                Time.timeScale = 1;
        }
    }
}