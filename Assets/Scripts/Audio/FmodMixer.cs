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
        
        private void Start()
        {
            sfx = RuntimeManager.GetBus("bus:/Sfx");
            music = RuntimeManager.GetBus("bus:/Music");


            float dbMusic = PlayerPrefs.GetFloat("Music");
            float dbSfx = PlayerPrefs.GetFloat("SFX");

            music.setVolume(DecibelToLinear(dbMusic));
            sfx.setVolume(DecibelToLinear(dbSfx));
        }

        public void ChangeMusicVolume(float dB)
        {
            if (dB <= -19.5f)
                dB = -200f;
            music.setVolume(DecibelToLinear(dB));
            PlayerPrefs.SetFloat("Music", dB);
            PlayerPrefs.Save();
        }

        public void ChangeSfxVolume(float dB)
        {
            if (dB <= -19.5f)
                dB = -200f;
            sfx.setVolume(DecibelToLinear(dB));
            PlayerPrefs.SetFloat("SFX", dB);
            PlayerPrefs.Save();
        }

        public void FindAllSfxAndPlayPause(bool gameIsPaused)
        {
            if (FindObjectsOfType(typeof(StudioEventEmitter)) is StudioEventEmitter[] eventEmitters)
            {
                foreach (var eventEmitter in eventEmitters)
                {
                    // EventReference.Path cannot be used in builds. This GUID represents the Stage Music.
                    // Pause every sound except the Stage Music.
                    if (eventEmitter.EventReference.Guid.ToString() != "{3f293b51-10ca-4c8a-bff9-897937d8445f}")
                    {
                        switch (gameIsPaused)
                        {
                            case true when eventEmitter.IsPlaying():
                                eventEmitter.EventInstance.setPaused(true);
                                break;
                            case false when !eventEmitter.IsPlaying():
                                eventEmitter.EventInstance.setPaused(false);
                                break;
                        }
                    }
                }
            }
        }

        public void KillEverySound()
        {
            print("DEATH TO AUDIO MUSIC IS FORBIDDEN BY THE LAW OF ROBOTNIK");
            if (FindObjectsOfType(typeof(StudioEventEmitter)) is StudioEventEmitter[] eventEmitters)
            {
                foreach (var eventEmitter in eventEmitters)
                {
                    eventEmitter.AllowFadeout = false;
                    eventEmitter.Stop();
                }
            }
        }

        public void KillEverySoundExceptEraseScores()
        {
            print("DEATH TO AUDIO MUSIC IS FORBIDDEN BY THE LAW OF ROBOTNIK");
            if (FindObjectsOfType(typeof(StudioEventEmitter)) is StudioEventEmitter[] eventEmitters)
            {
                foreach (var eventEmitter in eventEmitters)
                {
                    if (eventEmitter.EventReference.Guid.ToString() != "{00534ba2-fd7d-4840-9a05-f93af61cdfde}")
                    {
                        eventEmitter.AllowFadeout = false;
                        eventEmitter.Stop();
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