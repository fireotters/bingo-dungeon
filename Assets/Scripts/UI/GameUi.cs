using Audio;
using FMODUnity;
using Signals;
using TMPro;
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
        private FmodMixer _fmodMixer;
        private StudioEventEmitter _gameSong;
        
        [Header("FFW")]
        public TextMeshProUGUI ffwText;
        private readonly string ffwDisabledText = "FFW Disabled", ffwEnabledText = "FFW Enabled";
        private bool _isGamePaused, _isFfwActive;

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
            
            // FFW
            ffwText.text = _isFfwActive ? ffwEnabledText : ffwDisabledText;
            SignalBus<SignalToggleFfw>.Subscribe(ToggleFfw).AddTo(_disposables);
        }

        private void Update()
        {
            CheckKeyInputs();

            ffwText.text = _isFfwActive ? ffwEnabledText : ffwDisabledText;
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

            if (Input.GetKeyDown(KeyCode.F))
            {
                _isFfwActive = !_isFfwActive;
            }
        }

        private void ToggleFfw(SignalToggleFfw signal)
        {
            if (_isFfwActive)
            {
                Time.timeScale = signal.Enabled ? 3 : 1;
            }
        }

        private void GameIsPaused(bool intent)
        {
            // Show or hide pause panel and set timescale
            _isGamePaused = intent;
            _gameSong.SetParameter("Menu", intent ? 1 : 0);
            gamePausePanel.SetActive(intent);
            Time.timeScale = intent ? 0 : 1;
            _fmodMixer.FindAllSfxAndPlayPause(gameIsPaused: intent);
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
            _gameSong.Stop();
            SceneManager.LoadScene(sceneToLoad);
            Time.timeScale = 1;
        }

        public void ExitGameFromPause()
        {
            _fmodMixer.KillEverySound();
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
