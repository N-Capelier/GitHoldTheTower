using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuSounds : MonoBehaviour
{
    // Start is called before the first frame update
    public Text volumeMasterText, volumeMusicText, volumeEffectsText, volumeAnnoucersText;
    public ScriptableParamsPlayer selfParams;

    public Slider volumeMasterSlider, volumeMusicSlider, volumeEffectsSlider, volumeAnnoucersSlider;

    public AudioMixer masterMixer;

    private void Start()
    {
        volumeMusicSlider.value = selfParams.musicVolume;
        volumeEffectsSlider.value = selfParams.effectsVolume;
    }
    public void VolumeMasterSlide(float volume)
    {
        selfParams.masterVolume = (int)volume;
        float temp = SoundManager.Instance.ChangeMasterVolume(volume);
        volumeMasterText.text = ((int)temp).ToString();

    }

    public void VolumeMusicSlide(float volume)
    {
        selfParams.musicVolume = (int)volume;
        float temp = SoundManager.Instance.ChangeMusicVolume(volume);
        volumeMusicText.text = ((int)temp).ToString();

    }

    public void VolumeEffectsSlide(float volume)
    {
        selfParams.effectsVolume = (int)volume;
        float temp = SoundManager.Instance.ChangeSfxVolume(volume);
        volumeEffectsText.text = ((int)temp).ToString();

    }

    public void VolumeAnnoucersSlide(float volume)
    {
        selfParams.annoucersVolume = (int)volume;
        float temp = SoundManager.Instance.ChangeAnnoucerVolume(volume);
        volumeAnnoucersText.text = ((int)temp).ToString();
    }
}
