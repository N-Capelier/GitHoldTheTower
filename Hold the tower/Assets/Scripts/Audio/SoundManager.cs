using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Audio;


public class SoundManager : Singleton<SoundManager>
{

    public SoundEventList soundEventList;

    public AudioMixerGroup sfxMixer;

    public AudioMixerGroup musicMixer;

    public GameObject emptyGameObject;

    public SoundReference actualMusic;

    public AudioSource MusicSource1;

    public AudioSource MusicSource2;

    public ScriptableParamsPlayer playersParam;

    private void Awake()
	{
        CreateSingleton(true);
	}

	void Start()
    {
        if (soundEventList != null)
        {
            soundEventList.InitialiseSoundIDs();
            soundEventList.InitialiseEventIDs();
        }
    }

    public void TestSound()
    {
        //Une fonction qui permet de tester un son

        PlaySoundEvent(0);
    }

    public void TestSpatialisedSound()
    {
        //Une fonction qui permet de tester un son

        //SoundReference soundRef PlaySoundEvent(0, emptyGameObject);
    }

    public void PlayMusic(SoundEvent thisEvent)
    {
        if (MusicSource1.clip != null)
        {
            MusicSource1.Stop();
        }

        MusicSource1.clip = thisEvent.sounds[0].clip;

        MusicSource1.volume = thisEvent.sounds[0].volume;

        MusicSource1.Play();

    }

    public void StopMusic(SoundEvent thisEvent)
    {
        MusicSource1.Stop();

        MusicSource1.clip = null;

    }



    public SoundReference PlaySoundEvent(SoundEvent thisEvent)
    {
        //Joue un son écoutable par tous mais qui n'est pas localisé dans l'espace.

        //On commence par créer la référence puis créer l'audioSource qui va jouer le son.
        SoundReference soundRef = new SoundReference();
        soundRef.audioSource = gameObject.AddComponent<AudioSource>();

        //On change le volume du son;
        soundRef.audioSource.volume = playersParam.effectsVolume;

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

        if (soundRef.sound.clip == null)
        {
            soundRef.sound = soundEventList.FindEvent(0).sounds[0];
        }

        //On met la référence du son joué
        actualMusic = soundRef;

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

        //On change le volume du son;
        soundRef.audioSource.volume = playersParam.effectsVolume;

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

        if (soundRef.sound.clip == null)
        {
            soundRef.sound = soundEventList.FindEvent(0).sounds[0];
        }

        //On met la référence du son joué
        actualMusic = soundRef;

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

        //On change le volume du son;
        soundRef.audioSource.volume = playersParam.effectsVolume;

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

        if (soundRef.sound.clip == null)
        {
            soundRef.sound = soundEventList.FindEvent(0).sounds[0];
        }

        //On met la référence du son joué
        actualMusic = soundRef;

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

        //On change le volume du son;
        soundRef.audioSource.volume = playersParam.effectsVolume;

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

        if (soundRef.sound.clip == null)
        {
            soundRef.sound = soundEventList.FindEvent(0).sounds[0];
        }
        //On met la référence du son joué
        actualMusic = soundRef;

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

        //On change le volume du son;
        soundRef.audioSource.volume = playersParam.effectsVolume;

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

        if (soundRef.sound.clip == null)
        {
            soundRef.sound = soundEventList.FindEvent(0).sounds[0];
        }

        //On met la référence du son joué
        actualMusic = soundRef;

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

        //On change le volume du son;
        soundRef.audioSource.volume = playersParam.effectsVolume;

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

        if (soundRef.sound.clip == null)
        {
            soundRef.sound = soundEventList.FindEvent(0).sounds[0];
        }

        //On met la référence du son joué
        actualMusic = soundRef;

        //On joue le son !
        StartCoroutine(PlaySFX(soundRef));

        //On retourne la référence du son pour qu'il soit modifiable pas la suite la ou on l'appel.
        return soundRef;
    }
    public SoundReference PlaySoundEvent(string thisEventName, AudioSource source)
    {
        //Joue un son écoutable par tous et localisé dans l'espace.

        //On commence par créer la référence puis créer l'audioSource qui va jouer le son.
        SoundReference soundRef = new SoundReference();
        soundRef.audioSource = source;

        //On change le volume du son;
        soundRef.audioSource.volume = playersParam.effectsVolume;

        //On va chercher l'event avec l'ID correspondant;
        SoundEvent thisEvent = soundEventList.FindEvent(thisEventName);
        if(thisEvent != null)
        {
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

            if (soundRef.sound.clip == null)
            {
                soundRef.sound = soundEventList.FindEvent(0).sounds[0];
            }

            //On joue le son !
            soundRef.audioSource.Play();
        }
        //On met la référence du son joué
        actualMusic = soundRef;

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
    public IEnumerator StopSoundWithDelay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);

        source.Stop();

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
