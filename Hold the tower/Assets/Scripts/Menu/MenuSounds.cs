using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuSounds : MonoBehaviour
{
    // Start is called before the first frame update
    public Text volumeMusicText, volumeEffectsText;
    public ScriptableParamsPlayer selfParams;

    public Slider volumeMusicSlider, volumeEffectsSlider;

    public AudioMixer masterMixer;

    private void Start()
    {
        volumeMusicSlider.value = selfParams.musicVolume;
        volumeEffectsSlider.value = selfParams.effectsVolume;
    }
    public void VolumeMusicSlide(float volume)
    {
        selfParams.musicVolume = volume;
        masterMixer.SetFloat("MusicVolume", volume);
        volumeMusicText.text = volume.ToString();

    }

    public void VolumeEffectsSlide(float volume)
    {
        selfParams.effectsVolume = volume;
        masterMixer.SetFloat("EffectsVolume", volume);
        volumeEffectsText.text = volume.ToString();
    }
}
