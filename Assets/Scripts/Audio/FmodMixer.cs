using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Audio
{
    public class FmodMixer : MonoBehaviour
    {
        private Bus sfx;
        private Bus music;
        [SerializeField] private StudioEventEmitter sfxDemoEmitter;
        
        private void Start()
        {
            sfx = RuntimeManager.GetBus("bus:/Sfx");
            music = RuntimeManager.GetBus("bus:/Music");
        }

        public void ChangeMusicVolume(float dB)
        {
            print($"Set music decibels to {dB}");
            music.setVolume(DecibelToLinear(dB));
            PlayerPrefs.SetFloat("Music", dB);
            PlayerPrefs.Save();
        }

        public void ChangeSfxVolume(float dB)
        {
            print($"Set sfx decibels to {dB}");
            sfx.setVolume(DecibelToLinear(dB));
            PlayerPrefs.SetFloat("SFX", dB);
            PlayerPrefs.Save();

            if (!sfxDemoEmitter.IsPlaying())
            {
                sfxDemoEmitter.Play();
            }
        }

        public void FindAllSfxAndPlayPause(bool gameIsPaused)
        {
            if (FindObjectsOfType(typeof(StudioEventEmitter)) is StudioEventEmitter[] eventEmitters)
            {
                foreach (var eventEmitter in eventEmitters)
                {
                    switch (gameIsPaused)
                    {
                        case true when eventEmitter.IsPlaying():
                            eventEmitter.Stop();
                            break;
                        case false when !eventEmitter.IsPlaying():
                            eventEmitter.Play();
                            break;
                    }
                }
            }
        }

        private float DecibelToLinear(float dB)
        {
            var linear = Mathf.Pow(10f, dB / 20f);
            return linear;
        }
    }
}