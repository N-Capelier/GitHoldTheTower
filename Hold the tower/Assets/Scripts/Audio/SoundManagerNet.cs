using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SoundManagerNet : NetworkBehaviour
{

    [Command(requiresAuthority = false)]
    public void CmdPlaySoundEvent(string thisEventName, GameObject location)
    {
        RpcPlaySoundEvent(thisEventName, location);
    }

    [ClientRpc]
    public void RpcPlaySoundEvent(string thisEventName, GameObject location)
    {
        AudioSource source = location.GetComponent<AudioSource>();
        SoundManager.Instance.PlaySoundEvent(thisEventName, source);
    }

}
