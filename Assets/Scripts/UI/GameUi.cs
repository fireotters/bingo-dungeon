using Audio;
using FMODUnity;
using Signals;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class GameUi : BaseUi
    {
        public string sceneToLoad;

        [SerializeField] private GameUiDialogs _dialogs;
        [SerializeField] private GameUiPlayerUi _playerUi;
        [SerializeField] private GameUiSound _sound;
        [SerializeField] private GameUiFfwUi _ffwUi;
        private Entities.Turn_System.TurnManager _turnManager;
        private PieceCounter _pieceCounter;

        private readonly CompositeDisposable _disposables = new();

        private void Start()
        {
            if (sceneToLoad == "")
                Debug.LogError("GameUi: sceneToLoad not set! Finishing level will crash the game.");

            _sound.musicStage.Play();

            // Assign some UI buttons to objects in scene
            Entities.Player _player = FindObjectOfType<Entities.Player>();
            _turnManager = FindObjectOfType<Entities.Turn_System.TurnManager>();
            _pieceCounter = FindObjectOfType<PieceCounter>();
            _playerUi.btnEndTurn.onClick.AddListener(_player.WaitAfterKillingThenEndTurn);
            _playerUi.btnRetryLevelFromTokenStuck.onClick.AddListener(ResetCurrentLevel);
            _playerUi.btnRetryLevelFromResetTokens.onClick.AddListener(ResetCurrentLevel);
            _playerUi.btnEraseBlackTiles.onClick.AddListener(_turnManager.RemoveTokensOnBlackSquares);
            _playerUi.btnEraseWhiteTiles.onClick.AddListener(_turnManager.RemoveTokensOnWhiteSquares);

            _ffwUi.ToggleFfwUi("getFfwPref");


            SignalBus<SignalToggleFfw>.Subscribe(ToggleFfwTimeScale).AddTo(_disposables);
            SignalBus<SignalEnemyDied>.Subscribe(HandleEnemiesPissed).AddTo(_disposables);
            SignalBus<SignalGameEnded>.Subscribe(HandleEndGame).AddTo(_disposables);
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
                if (!_dialogs.options.activeInHierarchy)
                {
                    GameIsPaused(!_dialogs.paused.activeInHierarchy);
                }
                else
                {
                    _dialogs.options.SetActive(!_dialogs.options.activeInHierarchy);
                }
            }
        }

        public void ToggleFfw()
        {
            // Disable debug FFW if it's enabled
            if (Time.timeScale == 6)
            {
                Time.timeScale = 1;
                _ffwUi.debugFfwDisabledIcon.SetActive(true);
                _ffwUi.debugFfwEnabledIcon.SetActive(false);
            }

            _ffwUi.ToggleFfwUi("setFfwPref");
        }

        private void ToggleFfwTimeScale(SignalToggleFfw signal)
        {
            if (_ffwUi.isFfwActive)
            {
                Time.timeScale = signal.Enabled ? 3 : 1;
            }
        }

        private void GameIsPaused(bool intent)
        {
            // Show or hide pause panel and set timescale
            _sound.musicStage.SetParameter("Menu", intent ? 1 : 0);
            _dialogs.paused.SetActive(intent);
            Time.timeScale = intent ? 0 : 1;
            _sound.fmodMixer.FindAllSfxAndPlayPause(gameIsPaused: intent);
        }

        public void ResumeGame()
        {
            GameIsPaused(false);
        }

        public void ToggleOptionsPanel()
        {
            _dialogs.options.SetActive(!_dialogs.options.activeInHierarchy);
        }

        public void LoadNextScene()
        {
            _sound.fmodMixer.KillEverySound();
            SceneManager.LoadScene(sceneToLoad);
        }
        public void ResetCurrentLevel()
        {
            _sound.fmodMixer.KillEverySound();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        public void ExitGameFromPause()
        {
            _sound.fmodMixer.KillEverySound();
            SceneManager.LoadScene("MainMenu");
            Time.timeScale = 1;
        }

        public void DebugSuperSpeed()
        {
            // Disable normal ffw, since it changes timeScale constantly
            if (_ffwUi.isFfwActive)
                ToggleFfw();

            if (Time.timeScale == 1)
                Time.timeScale = 6;
            else
                Time.timeScale = 1;

            _ffwUi.debugFfwDisabledIcon.SetActive(Time.timeScale != 6);
            _ffwUi.debugFfwEnabledIcon.SetActive(Time.timeScale == 6);
        }

        public void UpdateTokenClearCooldown(int turnsToSet, bool playDestroySound = false)
        {
            //print("Turns til Token Clear Ability: " + turnsToSet);
            _playerUi.txtTokenClearCooldownTooltipText.text = turnsToSet.ToString();
            _playerUi.txtTokenClearCooldownBlockText.text = "----- " + turnsToSet.ToString() + " turns left -----";
            _playerUi.imageTokenClearCooldown.fillAmount = (4 - turnsToSet) / 4f;
            if (playDestroySound)
            {
                _sound.sfxTokenDestroy.Play();
                _playerUi.tokenClearCooldownBlock.SetActive(true);
            }
            if (turnsToSet == 0)
            {
                _playerUi.tokenClearCooldownBlock.SetActive(false);
            }
        }

        private void HandleEndGame(SignalGameEnded context)
        {
            switch (context.result)
            {
                case GameEndCondition.BingoWin:
                    // TODO add unique elements. Think about disabling UI and telling player if their Bingo win was lucky (final token lands in place randomly)
                    break;
                case GameEndCondition.PieceWin:
                    // TODO add unique elements
                    break;
                case GameEndCondition.Loss:
                    _sound.musicStage.SetParameter("Dead", 1);
                    _dialogs.gameLost.SetActive(true);
                    return;
                default:
                    Debug.LogError("GameUi.HandleEndGame: Invalid GameEnded context.");
                    return;
            }

            // Win Conditions trigger some similar behaviour
            _sound.musicStage.SetParameter("Win", 1);
            _dialogs.gameWon.SetActive(true);

            string levelName = SceneManager.GetActiveScene().name;
            GameEndCondition scoreType = context.result;
            int score = _turnManager.totalTurns;
            HighScoreManagement.TryAddLevelScore(levelName, scoreType, score);
        }

        private void HandleEnemiesPissed(SignalEnemyDied signal)
        {
            // Slightly delay the 'Pissed' UI changes, because we need to be sure PieceCounter has the correct piece count. TODO improve this when PieceCounter is removed.
            Invoke(nameof(HandleEnemiesPissed2), 0.1f);
        }

        private void HandleEnemiesPissed2()
        {
            if (_playerUi.goForBingoUiVisible)
            {
                _playerUi.goForBingoUiVisible = false;
                _playerUi.uiGoingForBingoButtons.SetActive(false);
                _playerUi.uiGoingForPiecesButtons.SetActive(true);
                _sound.musicStage.SetParameter("Angry", 1);
                if (_pieceCounter.numOfPieces > 0)
                    _sound.sfxPiecesPissed.Play();
            }
        }

        public bool IsGameplayInterruptingPanelOpen()
        {
            return _dialogs.paused.activeInHierarchy || _dialogs.panelResetTokens.activeInHierarchy || _dialogs.panelTokensStuck.activeInHierarchy;
        }

        public void ShowGameplayButtons(bool show)
        {
            _playerUi.uiAllGameplayButtons.SetActive(show);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }



    [System.Serializable]
    public class GameUiDialogs
    {
        public GameObject paused, options;
        public GameObject gameLost, gameWon;
        public GameObject panelTokensStuck, panelResetTokens;
    }
    [System.Serializable]
    public class GameUiPlayerUi
    {
        public GameObject uiAllGameplayButtons, uiGoingForBingoButtons, uiGoingForPiecesButtons;
        public Button btnEndTurn, btnRetryLevelFromTokenStuck, btnRetryLevelFromResetTokens, btnEraseBlackTiles, btnEraseWhiteTiles;

        public TextMeshProUGUI txtTokenClearCooldownTooltipText, txtTokenClearCooldownBlockText;
        public GameObject tokenClearCooldownBlock;
        public Image imageTokenClearCooldown;
        public bool goForBingoUiVisible = true;
    }
    [System.Serializable]
    public class GameUiFfwUi
    {
        public TextMeshProUGUI ffwTooltipText;
        public readonly string ffwDisabledText = "NORMAL SPEED", ffwEnabledText = "FAST-FORWARDED";
        public GameObject ffwDisabledIcon, ffwEnabledIcon;
        public GameObject debugFfwDisabledIcon, debugFfwEnabledIcon;
        public bool isFfwActive;

        public void ToggleFfwUi(string getset)
        {
            if (getset == "getFfwPref")
                isFfwActive = PlayerPrefs.GetInt("Ffw Enabled", 0) == 1;
            else if (getset == "setFfwPref")
            {
                isFfwActive = !isFfwActive;
                PlayerPrefs.SetInt("Ffw Enabled", isFfwActive ? 1 : 0);
                PlayerPrefs.Save();
            }

            ffwTooltipText.text = isFfwActive ? ffwEnabledText : ffwDisabledText;
            ffwDisabledIcon.SetActive(!isFfwActive);
            ffwEnabledIcon.SetActive(isFfwActive);
        }
    }
    [System.Serializable]
    public class GameUiSound
    {
        public StudioEventEmitter sfxTokenDestroy, musicStage, sfxPiecesPissed;
        public FmodMixer fmodMixer;
    }
}
