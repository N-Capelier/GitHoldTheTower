using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundEvent
{

    public string name;
    public int ID;

    [Space()]
    public Sound[] sounds;

    [Space()]
    [Tooltip("Le Son est-il localisé (joué sur place) ou global ?")]
    public bool isLocalized;

    [Space()]
    [Tooltip("S'il y a plusieurs sons, est-il joué de manière aléatoire ?")]
    public bool isRandom;

    [Space()]
    [Tooltip("Le Son se répète-il indéfiniment ?")]
    public bool isLoop;

    [Space()]
    [Tooltip("Le Son est-il une musique ?")]
    public bool isMusic;

    [Space()]
    [Tooltip("Le Son est-il une musique ?")]
    public bool isAnnoucer;

}
