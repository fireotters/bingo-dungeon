using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI
{
    public class OptionsMenu : BaseUi
    {
        [Header("Option Panel")] 
        
        [SerializeField] private Slider _optionMusicSlider;
        public Slider optionSFXSlider;
        [SerializeField] private Toggle _optionsFullScreenToggle;
        private Audio.FmodMixer fmodMixer;

        private void Awake()
        {
            fmodMixer = FindObjectOfType<Canvas>().GetComponent<Audio.FmodMixer>();

            if (Screen.fullScreen)
                _optionsFullScreenToggle.isOn = true;
        }

        private void OnEnable()
        {
            OptionsOpen();
        }

        private void OnDisable()
        {
            OptionsClose();
        }

        public void PassMusicVolChange(float dB)
        {
            fmodMixer.ChangeMusicVolume(dB);
        }

        public void PassSfxVolChange(float dB)
        {
            fmodMixer.ChangeSfxVolume(dB);
        }

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

        public void ResetHighscores()
        {
            fmodMixer.KillEverySoundExceptEraseScores();
            HighScoreManagement.ResetLevelScores();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}