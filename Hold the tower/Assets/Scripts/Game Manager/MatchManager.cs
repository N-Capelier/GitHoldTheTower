using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MatchManager : NetworkBehaviour
{
    [SyncVar]
    public bool matchCanStart = false;
    [SyncVar]
    public bool startGame = false;

    public float timerStartRound;


    void Start()
    {
        
    }

    void Update()
    {
        if(AllClientAreReady() && !startGame)
        {
            startGame = true;
            ActivatePlayer();

        }
        
    }

    public void NewRound()
    {
        if (AllClientAreReady())
        {
            ActivatePlayer();
        }
    }

    private bool AllClientAreReady()
    {
        foreach( NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (!conn.isReady)
            {
                return false;
            }
        }
        return true;
    }

    private void ActivatePlayer()
    {
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            foreach (NetworkIdentity idOwnedByClient in conn.clientOwnedObjects)
            {
                if (idOwnedByClient.gameObject.GetComponent<PlayerLogic>() != null)
                {
                    Debug.Log("test");
                    idOwnedByClient.gameObject.GetComponent<PlayerLogic>().Respawn(3f);
                }
            }
        }
    }
}
