using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class GoalBehavior : NetworkBehaviour
{
    [HideInInspector]
    public LobbyPlayerLogic.TeamName goalTeam;
    public MatchManager matchManager;
    

    private void OnTriggerEnter(Collider other) //Add a point to the team
    {
        if(other.tag == "Player")
        {
            if (other.transform.parent.GetComponent<PlayerLogic>().teamName != goalTeam && other.transform.parent.GetComponent<PlayerLogic>().hasFlag)
            {
                string textToShow = "";
                if (goalTeam == LobbyPlayerLogic.TeamName.Blue)
                {
                    textToShow = matchManager.redTeamTextScore;
                    CmdRedTeamScore();
                }

                if (goalTeam == LobbyPlayerLogic.TeamName.Red)
                {
                    textToShow = matchManager.blueTeamTextScore;
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
        matchManager.redScore++;
    }
    [Command(requiresAuthority = false)]
    private void CmdBlueTeamScore()
    {
        matchManager.blueScore++;
    }

    [Command(requiresAuthority = false)]
    private void CmdTeamWin(string text)
    {
        if (matchManager.redScore == matchManager.maxScore)
        {
            matchManager.RpcEndGame(matchManager.redTeamTextWin);
            return;
        }
        if(matchManager.blueScore == matchManager.maxScore)
        {
            matchManager.RpcEndGame(matchManager.blueTeamTextWin);
            return;
        }
        matchManager.CmdNewRound(text);

    }

}
