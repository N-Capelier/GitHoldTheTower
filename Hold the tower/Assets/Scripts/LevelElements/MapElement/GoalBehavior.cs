using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class GoalBehavior : NetworkBehaviour
{
    public LobbyPlayerLogic.nameOfTeam goalTeam;
    public MatchManager matchManager;
    public int maxScore;

    [SerializeField]
    private string redTeamTextScore = "Red Team Score";
    [SerializeField]
    private string blueTeamTextScore = "Blue Team Score";

    [SerializeField]
    private string redTeamTextWin = "Red team win the game";
    [SerializeField]
    private string blueTeamTextWin = "Blue team win the game";

    [SyncVar]
    public int redScore;
    [SyncVar]
    public int blueScore;

    private void OnTriggerEnter(Collider other) //Add a point to the team
    {
        if(other.tag == "Player")
        {
            if (other.transform.parent.GetComponent<PlayerLogic>().teamName != goalTeam && other.transform.parent.GetComponent<PlayerLogic>().hasFlag)
            {
                string textToShow = "";
                if (goalTeam == LobbyPlayerLogic.nameOfTeam.blue)
                {
                    textToShow = redTeamTextScore;
                    CmdRedTeamScore();
                }

                if (goalTeam == LobbyPlayerLogic.nameOfTeam.red)
                {
                    textToShow = blueTeamTextScore;
                    CmdBlueTeamScore();
                }
                other.transform.parent.GetComponent<PlayerLogic>().CmdDropFlag();
                CmdTeamWin(textToShow);
                
            }
        }
        
    }

    [Command(requiresAuthority = false)]
    private void CmdRedTeamScore()
    {
        redScore++;
    }
    [Command(requiresAuthority = false)]
    private void CmdBlueTeamScore()
    {
        blueScore++;
    }

    [Command(requiresAuthority = false)]
    private void CmdTeamWin(string text)
    {
        if (redScore == maxScore)
        {
            matchManager.RpcEndGame(redTeamTextWin);
            return;
        }
        if(blueScore == maxScore)
        {
            matchManager.RpcEndGame(blueTeamTextWin);
            return;
        }
        matchManager.CmdNewRound(text);

    }

}
