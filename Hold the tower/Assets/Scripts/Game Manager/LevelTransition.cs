using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LevelTransition : NetworkBehaviour
{
    [SerializeField]
    private GameObject ThemeObject;

    [HideInInspector]
    public double networkTime = 0;
    private int niveau = 0;

    public double timerChange = 5d;

    private bool doOnce = true;

    void OnChangeTerrain()
    {
        networkTime = NetworkTime.time;

        ThemeObject.GetComponent<ThemeManager>().LoadNextTerrain();
    }

    //Send to all client transition
    [ClientRpc]
    void RpcSendChange()
    {
        OnChangeTerrain();
    }

    void Update()
    {
        if (isServer && ReadyAllPlayers())
        {
            if (NetworkTime.time >= networkTime + timerChange)
            {
                RpcSendChange();
                doOnce = true;
                //OnChangeTerrain();
            }

            if (NetworkTime.time >= networkTime + timerChange - 1f && NetworkTime.time <= networkTime + timerChange - 0.1f && doOnce)
            {
                SoundManager.Instance.PlaySoundEvent("LevelEvolvingSound");
                doOnce = false;
            }
        }

    }

    //Check if all player have load the game
    private bool ReadyAllPlayers()
    {
        foreach (KeyValuePair<int,NetworkConnectionToClient> con in NetworkServer.connections)
        {
            if (!con.Value.isReady)
                return false;

        }
        return true;
    }
}
