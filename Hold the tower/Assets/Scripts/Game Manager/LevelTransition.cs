using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LevelTransition : NetworkBehaviour
{
    [SerializeField]
    private GameObject ThemeObject;

    [HideInInspector]
    public double networkTime = 0;

    public double timerChange = 5d;

    private bool doOnce = true;
    private bool doOnce1 = true;
    private bool doOnce2 = true;

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
                doOnce1 = true;
                doOnce2 = true;
                //OnChangeTerrain();
            }

            if (NetworkTime.time >= networkTime + timerChange - 5f && NetworkTime.time <= networkTime + timerChange - 4f && doOnce1)
            {
                SoundManager.Instance.PlaySoundEvent("LevelEvolvingAlarm");
                doOnce1 = false;
            }

            if (NetworkTime.time >= networkTime + timerChange - 1.5f && NetworkTime.time <= networkTime + timerChange - 0.5f && doOnce2)
            {
                SoundManager.Instance.PlaySoundEvent("LevelEvolvingAnoucement");
                doOnce2 = false;
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
