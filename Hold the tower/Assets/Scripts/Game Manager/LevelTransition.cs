using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LevelTransition : NetworkBehaviour
{
    [SerializeField]
    private GameObject ThemeObject;

    private double networkTime = 0;
    private int niveau = 0;

    [SerializeField]
    private double timerChange = 5d;

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
                //OnChangeTerrain();
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
