using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class GoalBehavior : NetworkBehaviour
{
    public LobbyPlayerLogic.nameOfTeam goalTeam;
    public MatchManager matchManager;
    public int maxScore;

    [SyncVar]
    public int redScore;
    [SyncVar]
    public int blueScore;

    private void OnTriggerEnter(Collider other) //Add a point to the team
    {
        if (other.GetComponent<PlayerLogic>().teamName != goalTeam && other.GetComponent<PlayerLogic>().hasFlag)
        {
            if(goalTeam == LobbyPlayerLogic.nameOfTeam.blue)
            {
                blueScore++;
            }

            if(goalTeam == LobbyPlayerLogic.nameOfTeam.red)
            {
                redScore++;
            }
            TeamWin();
            matchManager.NewRound();
        }
    }

    private void TeamWin()
    {
        if(redScore == maxScore)
        {
            TeamRedWin();
        }
        if(blueScore == maxScore)
        {
            TeamBlueWin();
        }

    }

    private void TeamRedWin()
    {

    }

    private void TeamBlueWin()
    {

    }
}
