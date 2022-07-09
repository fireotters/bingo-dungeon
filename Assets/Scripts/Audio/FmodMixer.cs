using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodMixer : MonoBehaviour
{
    FMOD.Studio.Bus sfx;
    FMOD.Studio.Bus music;
    [SerializeField][Range(-80, 10f)] float sfxVolume;
    [SerializeField][Range(-80, 10f)] float musicVolume;

    void Start()
    {
        sfx = FMODUnity.RuntimeManager.GetBus("bus:/Sfx");
        music = FMODUnity.RuntimeManager.GetBus("bus:/Music");
    }


    //Change parameters only when the audio slider changes
    void Update()
    {
        sfx.setVolume(DecibelToLinear(sfxVolume));
        music.setVolume(DecibelToLinear(sfxVolume));
    }

    private float DecibelToLinear (float dB)
    {
        float linear = Mathf.Pow(10f, dB / 20f);
        return linear;
    }
}
