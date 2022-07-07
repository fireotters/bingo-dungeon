using Audio;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OptionsMenu : MonoBehaviour
    {
        [Header("Option Panel")] 
        
        [SerializeField] private Slider _optionMusicSlider;

        public Slider optionSFXSlider;

        // Functions related to Options menu
        public void OptionsOpen()
        {
            _optionMusicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Music"));
            optionSFXSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFX"));
        }

        public void OptionsClose()
        {
            _optionMusicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Music"));
            optionSFXSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFX"));
        }

        public void SwapFullscreen()
        {
            if (Screen.fullScreen)
            {
                Screen.SetResolution(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2, false);
            }
            else
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            }
        }

        public void ChangeMusicPassToManager(float musVolume)
        {
             MusicManager.i.ChangeMusic(musVolume);
        }

        public void ChangeSfxPassToManager(float sfxVolume)
        {
             MusicManager.i.ChangeSfx(sfxVolume);
        }
    }
}