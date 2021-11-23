using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    [HideInInspector] public int ID;

    public AudioClip clip;

    public float volume;



}
