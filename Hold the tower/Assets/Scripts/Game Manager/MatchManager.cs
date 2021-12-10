using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MatchManager : NetworkBehaviour
{
    [SyncVar]
    public bool matchCanStart = false;
    public bool startGame = false;

    public int maxScore;

    [SyncVar]
    [HideInInspector]  public int redScore;
    [SyncVar]
    [HideInInspector]  public int blueScore;

    public GoalBehavior redGoal;
    public GoalBehavior blueGoal;

    [Header("Text à afficher")]
    [SerializeField]
    public string redTeamTextScore = "Red Team Score";
    [SerializeField]
    public string blueTeamTextScore = "Blue Team Score";

    [SerializeField]
    public string redTeamTextWin = "Red team win the game";
    [SerializeField]
    public string blueTeamTextWin = "Blue team win the game";

    void Start()
    {
        redGoal.goalTeam = LobbyPlayerLogic.TeamName.Red;
        blueGoal.goalTeam = LobbyPlayerLogic.TeamName.Blue;
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
                    idOwnedByClient.gameObject.GetComponent<PlayerLogic>().RpcRespawn(conn,3f);
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
