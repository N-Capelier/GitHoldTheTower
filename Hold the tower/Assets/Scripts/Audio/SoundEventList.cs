using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEventList", menuName = "Sound/SoundList")]
public class SoundEventList : ScriptableObject
{

    public SoundEvent[] soundEvents;

    public void InitialiseSoundIDs()
    {
        int ID = 0;

        foreach (var se in soundEvents)
        {
            foreach (var s in se.sounds)
            {
                s.ID = ID;
                ID++;
            }
        }
    }

    public void InitialiseEventIDs()
    {
        int ID = 0;

        foreach (var se in soundEvents)
        {
            se.ID = ID;
            ID++;
        }
    }

    public Sound FindSound(int id)
    {
        foreach (var se in soundEvents)
        {
            foreach (var s in se.sounds)
            {
                if (s.ID == id)
                {
                    return s;
                }
            }
        }
        return null;
    }

    public Sound FindSound(string name)
    {
        foreach (var se in soundEvents)
        {
            foreach (var s in se.sounds)
            {
                if (s.name == name)
                {
                    return s;
                }
            }
        }
        return null;

    }

    public SoundEvent FindEvent(int ID)
    {
        foreach (var se in soundEvents)
        {
            if(se.ID == ID) { return se; }
        }
        return null;
    }
    public SoundEvent FindEvent(string name)
    {
        foreach (var se in soundEvents)
        {
            if (se.name == name) { return se; }
        }
        return null;
    }

}
