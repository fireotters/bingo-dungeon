using Audio;
using FMODUnity;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameUi : BaseUi
    {
        [Header("Game UI")] public GameObject gamePausePanel;
        public GameObject optionsPanel;
        public GameObject gameOverPanel;
        public TextMeshProUGUI ffwText;
        private readonly string ffwDisabledText = "FFW Disabled", ffwEnabledText = "FFW Enabled";
        private FmodMixer _fmodMixer;
        private StudioEventEmitter _gameSong;
        private bool _isGamePaused, _isFfwActive;
        public string sceneToLoad;
        private readonly CompositeDisposable _disposables = new();


        private void Start()
        {
            _fmodMixer = GetComponent<FmodMixer>();
            _gameSong = GetComponent<StudioEventEmitter>();
            _gameSong.Play();
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
    }
}