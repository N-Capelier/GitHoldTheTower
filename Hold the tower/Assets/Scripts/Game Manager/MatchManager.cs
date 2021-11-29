using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MatchManager : NetworkBehaviour
{
    [SyncVar]
    public bool matchCanStart = false;
    public bool startGame = false;
    
    void Start()
    {
        
    }

    void Update()
    {
        if(AllClientAreReady() && !startGame && isServer)
        {
            startGame = true;
            ActivatePlayer();

        }
        
    }

    [Command(requiresAuthority = false)]
    public void CmdNewRound(string text)
    {
        if (AllClientAreReady())
        {
            ChangeUiPlayer(text);
        }
    }

    [ServerCallback]
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

    [Server]
    private void ActivatePlayer()
    {
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            foreach (NetworkIdentity idOwnedByClient in conn.clientOwnedObjects)
            {
                if (idOwnedByClient.gameObject.GetComponent<PlayerLogic>() != null)
                {
                    idOwnedByClient.gameObject.GetComponent<PlayerLogic>().Respawn(conn,3f);
                }
            }
        }
    }

    [Server]
    private void ChangeUiPlayer(string text)
    {
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            foreach (NetworkIdentity idOwnedByClient in conn.clientOwnedObjects)
            {
                if (idOwnedByClient.gameObject.GetComponent<PlayerLogic>() != null)
                {
                    idOwnedByClient.gameObject.GetComponent<PlayerLogic>().RpcShowGoal(conn,text);
                }
            }
        }
    }

    [Server]
    public void RpcEndGame(string text)
    {
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            foreach (NetworkIdentity idOwnedByClient in conn.clientOwnedObjects)
            {
                if (idOwnedByClient.gameObject.GetComponent<PlayerLogic>() != null)
                {
                    idOwnedByClient.gameObject.GetComponent<PlayerLogic>().RpcEndGame(conn, text);
                }
            }
        }
    }
}
