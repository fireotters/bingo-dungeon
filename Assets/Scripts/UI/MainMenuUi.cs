using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Signals;
using System.Collections.Generic;

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

        // Easter Egg. Code from Jessespike on Unity Forums (http://answers.unity.com/answers/1241572/view.html)
        const int MaxInputHistory = 8;
        [HideInInspector] public List<string> inputHistory = new List<string>();

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

            SignalBus<SignalUiMainMenuStartGame>.Subscribe(StartGame).AddTo(_disposables);
        }

        private void Update()
        {
            UpdateEasterEggState();
        }

        private void UpdateEasterEggState()
        {
            if (!string.IsNullOrEmpty(Input.inputString))
            {
                inputHistory.Insert(0, Input.inputString);
                while (inputHistory.Count > MaxInputHistory)
                {
                    inputHistory.RemoveAt(inputHistory.Count - 1);
                }
            }

            if (CheckForKeyword("loss"))
            {
                Debug.Log("It's morbin' time");
                inputHistory.Clear();
                menuSong.Stop();
                SceneManager.LoadScene($"Scenes/LevelScenes/ZZ Loss Secret (thx Tesla)");
            }
        }

        private bool CheckForKeyword(string keyword)
        {
            if (inputHistory.Count >= keyword.Length)
            {
                for (int i = 0; i < keyword.Length; i++)
                {
                    if (keyword[keyword.Length - 1 - i].ToString() != inputHistory[i])
                    {
                        // input does not match keyword
                        return false;
                    }
                }
                // input and keyword match has been found
                return true;
            }

            // not enough input to do comparison
            return false;
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

        public void StartGame(SignalUiMainMenuStartGame signal)
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