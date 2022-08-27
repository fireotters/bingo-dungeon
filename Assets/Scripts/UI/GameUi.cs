using Audio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class GameUi : BaseUi
    {
        public string sceneToLoad;

        [Header("Menus")]
        public GameObject gamePausePanel;
        public GameObject optionsPanel, gameOverPanel, gameStuckPanel;

        [Header("Player UI")]
        public Image tokenClearCooldownImage;
        public TextMeshProUGUI tokenClearCooldownText, tokenClearCooldownBlockText;
        public GameObject tokenClearCooldownBlock;
        public Button endTurnButton, retryLevelButton, eraseBlackTilesButton, eraseWhiteTilesButton;

        [Header("Music & Sound")]
        public StudioEventEmitter tokenDestroySound;
        private FmodMixer fmodMixer;
        private StudioEventEmitter gameSong;

        private void Start()
        {
            if (sceneToLoad == "")
                Debug.LogError("GameUi: sceneToLoad not set! Finishing level will crash the game.");

            fmodMixer = GetComponent<FmodMixer>();
            gameSong = GetComponent<StudioEventEmitter>();
            gameSong.Play();

            // Assign some UI buttons to objects in scene
            Entities.Player _player = FindObjectOfType<Entities.Player>();
            Entities.Turn_System.TurnManager _turnManager = FindObjectOfType<Entities.Turn_System.TurnManager>();
            endTurnButton.onClick.AddListener(_player.WaitAfterKillingThenEndTurn);
            retryLevelButton.onClick.AddListener(ResetCurrentLevel);
            eraseBlackTilesButton.onClick.AddListener(_turnManager.RemoveTokensOnBlackSquares);
            eraseWhiteTilesButton.onClick.AddListener(_turnManager.RemoveTokensOnWhiteSquares);
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

        public void UpdateTokenClearCooldown(int turnsToSet, bool playDestroySound = false)
        {
            print("Turns til Token Clear Ability: " + turnsToSet);
            tokenClearCooldownText.text = turnsToSet.ToString();
            tokenClearCooldownBlockText.text = "----- " + turnsToSet.ToString() + " turns left -----";
            tokenClearCooldownImage.fillAmount = (4 - turnsToSet) / 4f;
            if (playDestroySound)
            {
                tokenDestroySound.Play();
                tokenClearCooldownBlock.SetActive(true);
            }
            if (turnsToSet == 0)
            {
                tokenClearCooldownBlock.SetActive(false);
            }
        }
    }
}