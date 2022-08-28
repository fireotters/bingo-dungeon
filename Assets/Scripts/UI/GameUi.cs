using Audio;
using FMODUnity;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        public Button endTurnButton, retryLevelButton, retryLevelButton2, eraseBlackTilesButton, eraseWhiteTilesButton;
        public GameObject uiGoingForBingo, uiGoingForPieces;

        [Header("Music & Sound")]
        public StudioEventEmitter tokenDestroySound;
        private FmodMixer _fmodMixer;
        private StudioEventEmitter _gameSong;

        [Header("FFW")]
        public TextMeshProUGUI ffwText;
        private readonly string ffwDisabledText = "NORMAL SPEED", ffwEnabledText = "FAST-FORWARDED";
        public GameObject ffwDisabledIcon, ffwEnabledIcon;
        public GameObject debugFfwDisabledIcon, debugFfwEnabledIcon;
        private bool _isFfwActive;

        private bool bingoUiVisible = true;
        private readonly CompositeDisposable _disposables = new();

        private void Start()
        {
            if (sceneToLoad == "")
                Debug.LogError("GameUi: sceneToLoad not set! Finishing level will crash the game.");

            _fmodMixer = GetComponent<FmodMixer>();
            _gameSong = GetComponent<StudioEventEmitter>();
            _gameSong.Play();

            // Assign some UI buttons to objects in scene
            Entities.Player _player = FindObjectOfType<Entities.Player>();
            Entities.Turn_System.TurnManager _turnManager = FindObjectOfType<Entities.Turn_System.TurnManager>();
            endTurnButton.onClick.AddListener(_player.WaitAfterKillingThenEndTurn);
            retryLevelButton.onClick.AddListener(ResetCurrentLevel);
            retryLevelButton2.onClick.AddListener(ResetCurrentLevel);
            eraseBlackTilesButton.onClick.AddListener(_turnManager.RemoveTokensOnBlackSquares);
            eraseWhiteTilesButton.onClick.AddListener(_turnManager.RemoveTokensOnWhiteSquares);

            // FFW
            _isFfwActive = PlayerPrefs.GetInt("Ffw Enabled", 0) == 1;
            ffwText.text = _isFfwActive ? ffwEnabledText : ffwDisabledText;
            ffwDisabledIcon.SetActive(!_isFfwActive);
            ffwEnabledIcon.SetActive(_isFfwActive);
            SignalBus<SignalToggleFfw>.Subscribe(ToggleFfwTimeScale).AddTo(_disposables);

            // Pieces aggressive
            SignalBus<SignalEnemyDied>.Subscribe((x) =>
            {
                if (bingoUiVisible)
                {
                    bingoUiVisible = false;
                    uiGoingForBingo.SetActive(false);
                    uiGoingForPieces.SetActive(true);
                }
            }).AddTo(_disposables);
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

        public void ToggleFfw()
        {
            // Disable debug FFW if it's enabled
            if (Time.timeScale == 6)
            {
                Time.timeScale = 1;
                debugFfwDisabledIcon.SetActive(true);
                debugFfwEnabledIcon.SetActive(false);
            }

            _isFfwActive = !_isFfwActive;
            ffwText.text = _isFfwActive ? ffwEnabledText : ffwDisabledText;
            ffwDisabledIcon.SetActive(!_isFfwActive);
            ffwEnabledIcon.SetActive(_isFfwActive);
            PlayerPrefs.SetInt("Ffw Enabled", _isFfwActive ? 1 : 0);
        }

        private void ToggleFfwTimeScale(SignalToggleFfw signal)
        {
            if (_isFfwActive)
            {
                Time.timeScale = signal.Enabled ? 3 : 1;
            }
        }

        private void GameIsPaused(bool intent)
        {
            // Show or hide pause panel and set timescale
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
            _gameSong.Stop();
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
            // Disable normal ffw, since it changes timeScale constantly
            if (_isFfwActive)
                ToggleFfw();

            if (Time.timeScale == 1)
                Time.timeScale = 6;
            else
                Time.timeScale = 1;

            debugFfwDisabledIcon.SetActive(Time.timeScale != 6);
            debugFfwEnabledIcon.SetActive(Time.timeScale == 6);
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
