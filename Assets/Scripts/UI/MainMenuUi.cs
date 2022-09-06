using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Signals;

namespace UI
{
    public class MainMenuUi : BaseUi
    {
        private enum SceneNavigationIntent
        {
            StartGame = 1, HelpMenu = 0
        }

        [Header("Main Menu UI")]
        [SerializeField] private TextMeshProUGUI versionText;
        [SerializeField] private GameObject desktopButtons, webButtons;
        private StudioEventEmitter menuSong;

        private readonly CompositeDisposable _disposables = new();

        private void Start()
        {
            menuSong = GetComponent<StudioEventEmitter>();
#if UNITY_WEBGL
            desktopButtons.SetActive(false);
            webButtons.SetActive(true);
#else
            desktopButtons.SetActive(true);
            webButtons.SetActive(false);
#endif
            
            // Set version number
            SetVersionText();
            
            // Set up PlayerPrefs when game is first ever loaded
            if (!PlayerPrefs.HasKey("Music"))
            {
                PlayerPrefs.SetFloat("Music", 0.8f);
                PlayerPrefs.SetFloat("SFX", 0.8f);
                PlayerPrefs.SetString("LevelScores", "{}");
                PlayerPrefs.Save();
            }

            SignalBus<SignalMainMenuStartGame>.Subscribe(StartGame).AddTo(_disposables);
        }

        private void SetVersionText()
        {
            if (Debug.isDebugBuild)
            {
                versionText.text = !string.IsNullOrEmpty(Application.buildGUID)
                    ? $"Version debug-{Application.version}-{Application.buildGUID}"
                    : $"Version debug-{Application.version}-editor";
            }
            else
            {
                versionText.text = $"Version {Application.version}";
            }
        }

        public void StartGame(SignalMainMenuStartGame signal)
        {
            menuSong.Stop();
            SceneManager.LoadScene($"Scenes/LevelScenes/{signal.levelToLoad}");
        }

        public void OpenHelp()
        {
            menuSong.Stop();
            SceneManager.LoadScene("Scenes/HelpMenu");
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        
    }
}