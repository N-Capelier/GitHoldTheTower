using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundReference
{
    public string name;

    public int ID;

    public Sound sound;

    public AudioSource audioSource;


    public void ApplySoundToAudioSource(Sound sound, bool loop, AudioMixerGroup mixer)
    {
        audioSource.clip = sound.clip;
        audioSource.volume = sound.volume;
        audioSource.loop = loop;
        audioSource.outputAudioMixerGroup = mixer;
    }




}
