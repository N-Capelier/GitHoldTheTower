using System.Collections;
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

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnChangeTerrain()
    {
        networkTime = NetworkTime.time;
        if (niveau == 0)
            niveau = 1;
        else
            niveau = 0;

        ThemeObject.GetComponent<ThemeManager>().LoadTerrain(ThemeObject.GetComponent<ThemeManager>().terrains[niveau]);
    }

    //Send to all client transition
    [ClientRpc]
    void RpcSendChange()
    {
        OnChangeTerrain();
    }

    void Update()
    {
        if (isServer && AllPlayerReaddy())
        {
            if (NetworkTime.time >= networkTime + timerChange)
            {
                RpcSendChange();
                //OnChangeTerrain();
            }
        }

    }

    //Check if all player have load the game
    private bool AllPlayerReaddy()
    {
        foreach (KeyValuePair<int,NetworkConnectionToClient> con in NetworkServer.connections)
        {
            if (!con.Value.isReady)
                return false;

        }
        return true;
    }
}
