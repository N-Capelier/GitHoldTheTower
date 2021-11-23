using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; 

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public List<int> IDs = new List<int>();

    public SoundEventList soundEventList;

    public AudioMixerGroup sfxMixer;


    void Start()
    {
        if (soundEventList != null)
        {
            soundEventList.InitialiseSoundIDs();
            soundEventList.InitialiseEventIDs();
        }

    }

    private void Update()
    {

    }




    public SoundReference PlaySoundEvent(SoundEvent thisEvent)
    {
        //Joue un son écoutable par tous mais qui n'est pas localisé dans l'espace.

        //On commence par créer la référence puis créer l'audioSource qui va jouer le son.
        SoundReference soundRef = new SoundReference();
        soundRef.audioSource = gameObject.AddComponent<AudioSource>();
        
        //On appliques les infos du son dans l'audiosource
        if(thisEvent.isRandom)
        {
            int temp = UnityEngine.Random.Range(0, thisEvent.sounds.Length - 1);

            soundRef.sound = thisEvent.sounds[temp];
            soundRef.ApplySoundToAudioSource(thisEvent.sounds[temp], thisEvent.isLoop, sfxMixer);
        }
        else
        {
            soundRef.sound = thisEvent.sounds[0];
            soundRef.ApplySoundToAudioSource(thisEvent.sounds[0], thisEvent.isLoop, sfxMixer);
        }

        //On joue le son !
        StartCoroutine(PlaySFX(soundRef));

        //On retourne la référence du son pour qu'il soit modifiable pas la suite la ou on l'appel.
        return soundRef;
    }

    public SoundReference PlaySoundEvent(SoundEvent thisEvent, GameObject location)
    {
        //Joue un son écoutable par tous et localisé dans l'espace.

        //On commence par créer la référence puis créer l'audioSource qui va jouer le son.
        SoundReference soundRef = new SoundReference();
        soundRef.audioSource = location.AddComponent<AudioSource>();

        //On appliques les infos du son dans l'audiosource
        if (thisEvent.isRandom)
        {
            int temp = UnityEngine.Random.Range(0, thisEvent.sounds.Length - 1);

            soundRef.sound = thisEvent.sounds[temp];
            soundRef.ApplySoundToAudioSource(thisEvent.sounds[temp], thisEvent.isLoop, sfxMixer);
        }
        else
        {
            soundRef.sound = thisEvent.sounds[0];
            soundRef.ApplySoundToAudioSource(thisEvent.sounds[0], thisEvent.isLoop, sfxMixer);
        }

        //On joue le son !
        StartCoroutine(PlaySFX(soundRef));

        //On retourne la référence du son pour qu'il soit modifiable pas la suite la ou on l'appel.
        return soundRef;
    }

    public SoundReference PlaySoundEvent(int thisEventID)
    {
        //Joue un son écoutable par tous mais qui n'est pas localisé dans l'espace.

        //On commence par créer la référence puis créer l'audioSource qui va jouer le son.
        SoundReference soundRef = new SoundReference();
        soundRef.audioSource = gameObject.AddComponent<AudioSource>();

        //On va chercher l'event avec l'ID correspondant;
        SoundEvent thisEvent = soundEventList.FindEvent(thisEventID);

        //On appliques les infos du son dans l'audiosource
        if (thisEvent.isRandom)
        {
            int temp = UnityEngine.Random.Range(0, thisEvent.sounds.Length - 1);

            soundRef.sound = thisEvent.sounds[temp];
            soundRef.ApplySoundToAudioSource(thisEvent.sounds[temp], thisEvent.isLoop, sfxMixer);
        }
        else
        {
            soundRef.sound = thisEvent.sounds[0];
            soundRef.ApplySoundToAudioSource(thisEvent.sounds[0], thisEvent.isLoop, sfxMixer);
        }

        //On joue le son !
        StartCoroutine(PlaySFX(soundRef));

        //On retourne la référence du son pour qu'il soit modifiable pas la suite la ou on l'appel.
        return soundRef;
    }

    public SoundReference PlaySoundEvent(int thisEventID, GameObject location)
    {
        //Joue un son écoutable par tous et localisé dans l'espace.

        //On commence par créer la référence puis créer l'audioSource qui va jouer le son.
        SoundReference soundRef = new SoundReference();
        soundRef.audioSource = location.AddComponent<AudioSource>();

        //On va chercher l'event avec l'ID correspondant;
        SoundEvent thisEvent = soundEventList.FindEvent(thisEventID);

        //On appliques les infos du son dans l'audiosource
        if (thisEvent.isRandom)
        {
            int temp = UnityEngine.Random.Range(0, thisEvent.sounds.Length - 1);

            soundRef.sound = thisEvent.sounds[temp];
            soundRef.ApplySoundToAudioSource(thisEvent.sounds[temp], thisEvent.isLoop, sfxMixer);
        }
        else
        {
            soundRef.sound = thisEvent.sounds[0];
            soundRef.ApplySoundToAudioSource(thisEvent.sounds[0], thisEvent.isLoop, sfxMixer);
        }

        //On joue le son !
        StartCoroutine(PlaySFX(soundRef));

        //On retourne la référence du son pour qu'il soit modifiable pas la suite la ou on l'appel.
        return soundRef;
    }

    public SoundReference PlaySoundEvent(string thisEventName)
    {
        //Joue un son écoutable par tous mais qui n'est pas localisé dans l'espace.

        //On commence par créer la référence puis créer l'audioSource qui va jouer le son.
        SoundReference soundRef = new SoundReference();
        soundRef.audioSource = gameObject.AddComponent<AudioSource>();

        //On va chercher l'event avec l'ID correspondant;
        SoundEvent thisEvent = soundEventList.FindEvent(thisEventName);

        //On appliques les infos du son dans l'audiosource
        if (thisEvent.isRandom)
        {
            int temp = UnityEngine.Random.Range(0, thisEvent.sounds.Length - 1);

            soundRef.sound = thisEvent.sounds[temp];
            soundRef.ApplySoundToAudioSource(thisEvent.sounds[temp], thisEvent.isLoop, sfxMixer);
        }
        else
        {
            soundRef.sound = thisEvent.sounds[0];
            soundRef.ApplySoundToAudioSource(thisEvent.sounds[0], thisEvent.isLoop, sfxMixer);
        }

        //On joue le son !
        StartCoroutine(PlaySFX(soundRef));

        //On retourne la référence du son pour qu'il soit modifiable pas la suite la ou on l'appel.
        return soundRef;
    }

    public SoundReference PlaySoundEvent(string thisEventName, GameObject location)
    {
        //Joue un son écoutable par tous et localisé dans l'espace.

        //On commence par créer la référence puis créer l'audioSource qui va jouer le son.
        SoundReference soundRef = new SoundReference();
        soundRef.audioSource = location.AddComponent<AudioSource>();

        //On va chercher l'event avec l'ID correspondant;
        SoundEvent thisEvent = soundEventList.FindEvent(thisEventName);

        //On appliques les infos du son dans l'audiosource
        if (thisEvent.isRandom)
        {
            int temp = UnityEngine.Random.Range(0, thisEvent.sounds.Length - 1);

            soundRef.sound = thisEvent.sounds[temp];
            soundRef.ApplySoundToAudioSource(thisEvent.sounds[temp], thisEvent.isLoop, sfxMixer);
        }
        else
        {
            soundRef.sound = thisEvent.sounds[0];
            soundRef.ApplySoundToAudioSource(thisEvent.sounds[0], thisEvent.isLoop, sfxMixer);
        }

        //On joue le son !
        StartCoroutine(PlaySFX(soundRef));

        //On retourne la référence du son pour qu'il soit modifiable pas la suite la ou on l'appel.
        return soundRef;
    }

    public IEnumerator PlaySFX(SoundReference soundRef)
    {
        soundRef.audioSource.Play();

        while (soundRef.audioSource.isPlaying)
        {
            yield return null;
        }

        soundRef.audioSource.Stop();

        Destroy(soundRef.audioSource);

    }




      //Fontion pour jouer un son simplement.
       /*       public void PlaySoundEffect(SoundEvent playedEvent)
       {

           //On cherche le son que l'on va jouer dans la liste de son.

               if (source.audioSource.isPlaying == false)
               {
                   Sound sound = new Sound();

                   sound.clip = s.clip;
                   sound.volume = s.volume * SoundEffectsVolume;

                   sound.source = source.audioSource;

                   sounds.Add(sound);

                   source.audioSource.clip = s.clip;
                   source.audioSource.volume = s.volume * SoundEffectsVolume;

                   source.audioSource.loop = false;

                   source.audioSource.outputAudioMixerGroup = sfxMixer;

                   source.audioSource.Play();

                   break;
               }
           
       }*/




}
