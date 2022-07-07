using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [Obsolete(message: "Should not be used, in favour of a to be introduced FMOD implementation")]
    public class MusicManager : MonoBehaviour
    {
        public AudioMixer mixer;
        public AudioSource sfxDemo, currentMusicPlayer; // SFX Slider in Options, & playing music
        public AudioClip musicMainMenu, musicGameplay;
        public AudioClip selectSound, startGameSound, backSound;
        public AudioLowPassFilter audLowPass;

        private int _lastTrackRequested = -1; // When first created, pick the scene's chosen song

        public static MusicManager i;

        private void Awake()
        {
            if (i != null)
            {
                Destroy(gameObject);
            }
            else
            {
                i = this;
                DontDestroyOnLoad(gameObject);
            }

            audLowPass = GetComponent<AudioLowPassFilter>();
        }

        public void ChangeMusic(float sliderValue)
        {
            mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
            PlayerPrefs.SetFloat("Music", sliderValue);
            PlayerPrefs.Save();
        }

        public void ChangeSfx(float sliderValue)
        {
            mixer.SetFloat("SfxVolume", Mathf.Log10(sliderValue) * 20);
            PlayerPrefs.SetFloat("SFX", sliderValue);
            PlayerPrefs.Save();
            if (!sfxDemo.isPlaying)
            {
                sfxDemo.Play();
            }
        }

        // TODO Consider rewriting. Cam has a version that searches for Unity tags instead.
        public void FindAllSfxAndPlayPause(bool gameIsPaused)
        {
            if (FindObjectsOfType(typeof(AudioSource)) is AudioSource[] listOfSfxObjects)
            {
                foreach (var sfxObject in listOfSfxObjects)
                {
                    switch (gameIsPaused)
                    {
                        case true when sfxObject.isPlaying:
                            sfxObject.Pause();
                            break;
                        case false when !sfxObject.isPlaying:
                            sfxObject.UnPause();
                            break;
                    }
                }
            }
        }

        public void ChangeMusicTrack(int index)
        {
            // Set volumes
            var musicVol = PlayerPrefs.GetFloat("Music");
            var sfxVol = PlayerPrefs.GetFloat("SFX");
            mixer.SetFloat("MusicVolume", Mathf.Log10(musicVol) * 20);
            mixer.SetFloat("SfxVolume", Mathf.Log10(sfxVol) * 20);

            Debug.Log($"Music requested: {index} Music last played: {_lastTrackRequested}");
            if (index != _lastTrackRequested)
            {
                currentMusicPlayer.enabled = true;
                if (currentMusicPlayer.isPlaying)
                {
                    currentMusicPlayer.Stop();
                }
                
                currentMusicPlayer.clip = index switch
                {
                    0 => musicMainMenu,
                    1 => musicGameplay,
                    _ => currentMusicPlayer.clip
                };

                currentMusicPlayer.Play();
                _lastTrackRequested = index;
            }
        }
    }
}