using Audio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameUi : BaseUi
    {
        [Header("Game UI")] public GameObject gamePausePanel;
        public GameObject optionsPanel;
        public GameObject gameOverPanel;
        private FmodMixer fmodMixer;
        private StudioEventEmitter gameSong;
        public string sceneToLoad;

        private void Start()
        {
            fmodMixer = GetComponent<FmodMixer>();
            gameSong = GetComponent<StudioEventEmitter>();
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        public void ToggleOptionsPanel()
        {
            optionsPanel.SetActive(!optionsPanel.activeInHierarchy);
        }

        public void LoadNextScene()
        {
            SceneManager.LoadScene(sceneToLoad);
            Time.timeScale = 1;
        }

        public void ExitGameFromPause()
        {
            fmodMixer.KillEverySound();
            SceneManager.LoadScene("MainMenu");
            Time.timeScale = 1;
        }
    }
}