using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Audio;

namespace UI
{
    public class MainMenuUi : BaseUi
    {
        private enum SceneNavigationIntent
        {
            StartGame = 1,
            HelpMenu = 0
        }

        [Header("Main Menu UI")]
        [FormerlySerializedAs("optionsPanel")] [SerializeField] private OptionsMenu optionsMenu;
        [SerializeField] private TextMeshProUGUI versionText;
        [SerializeField] private GameObject desktopButtons, webButtons;
        
        private void Start()
        {
#if UNITY_WEBGL
            desktopButtons.SetActive(false);
            webButtons.SetActive(true);
#else
            desktopButtons.SetActive(true);
            webButtons.SetActive(false);
#endif
            
            // Set version number
            SetVersionText();
            // Find SFX Slider & tell MusicManager where it is
            MusicManager.i.sfxDemo = optionsMenu.optionSFXSlider.GetComponent<AudioSource>();

            // Set up PlayerPrefs when game is first ever loaded
            if (!PlayerPrefs.HasKey("Music"))
            {
                PlayerPrefs.SetFloat("Music", 0.8f);
                PlayerPrefs.SetFloat("SFX", 0.8f);
            }

            // Change music track & set volume. Disable low pass filter.
            MusicManager.i.ChangeMusicTrack(0);
            MusicManager.i.audLowPass.enabled = false;
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

        public void Transition(int b)
        {
            var intent = (SceneNavigationIntent) b;
            
            if (levelTransitionOverlay != null)
            {
                levelTransitionOverlay.SetBool("levelEndedOrDead", true);    
            }

            switch (intent)
            {
                case SceneNavigationIntent.HelpMenu:
                    Invoke(nameof(OpenHelp), 2);
                    break;
                case SceneNavigationIntent.StartGame:
                    Invoke(nameof(StartGame), 2);
                    break;
                default:
                    Debug.LogError("This option is not defined!");
                    break;
            }
        }

        public void StartGame()
        {
            SceneManager.LoadScene("Scenes/GameScene");
        }

        public void OpenHelp()
        {
            SceneManager.LoadScene("Scenes/HelpMenu");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}