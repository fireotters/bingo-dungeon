using Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameUi : BaseUi
    {
        [Header("Game UI")] public GameObject gamePausePanel;
        public GameObject optionsPanel;

        private void Start()
        {
            // levelTransitionOverlay.gameObject.SetActive(true);
            // Change music track
            MusicManager.i.ChangeMusicTrack(1);
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
            }
        }

        private void GameIsPaused(bool intent)
        {
            // Show or hide pause panel and set timescale
            gamePausePanel.SetActive(intent);
            Time.timeScale = intent ? 0 : 1;
            MusicManager.i.FindAllSfxAndPlayPause(gameIsPaused: intent);
        }

        public void ToggleOptionsPanel()
        {
            optionsPanel.SetActive(!optionsPanel.activeInHierarchy);
        }

        public void ExitGameFromPause()
        {
            SceneManager.LoadScene("MainMenu");
            Time.timeScale = 1;
        }
    }
}