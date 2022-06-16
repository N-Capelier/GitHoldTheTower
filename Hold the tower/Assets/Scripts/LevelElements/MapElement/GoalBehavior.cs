using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class GoalBehavior : NetworkBehaviour
{
    [HideInInspector]
    public LobbyPlayerLogic.TeamName goalTeam;
    public MatchManager matchManager;
    [SerializeField]
    private ParticleSystem goalEffect;

    public PA_Position pa_pos;

    private bool delay;

    public void Start()
    {
        if(GameObject.Find("Analytics") != null)
        {
            pa_pos = GameObject.Find("Analytics").GetComponent<PA_Position>();
        }
    }

    private void OnTriggerEnter(Collider other) //Add a point to the team
    {
        if(other.tag == "Player")
        {

            PlayerLogic scoringPlayer = other.transform.parent.GetComponent<PlayerLogic>();

            if (scoringPlayer.hasAuthority)
            {
                SteamAchievement.AddStatValue("stat_Goal", 1);
            }

            if (other.transform.parent.GetComponent<PlayerLogic>().teamName != goalTeam && scoringPlayer.hasFlag && delay == false)
            {
                StartCoroutine(DelayManager());
                string textToShow = "";

                if(scoringPlayer.hasAuthority)
				{
                    InGameDataGatherer.Instance.data.points++;
				}

                if (goalTeam == LobbyPlayerLogic.TeamName.Blue)
                {
                    textToShow = matchManager.redTeamTextScore;
                    CmdRedTeamScore();
                    if (matchManager.redScore == matchManager.maxScore)
                    {
                        other.transform.parent.GetComponent<PlayerLogic>().CmdPlayEquipTeamSound("LevelMatchWon", "LevelMatchLost");
                        other.transform.parent.GetComponent<PlayerLogic>().CmdPlayGlobalSound("PlayerOverdriveGoal");
                    }
                    else 
                    {
                        other.transform.parent.GetComponent<PlayerLogic>().CmdPlayEquipTeamSound("LevelTeamScores", "LevelEnemyScores");
                        other.transform.parent.GetComponent<PlayerLogic>().CmdPlayGlobalSound("PlayerOverdriveGoal");
                    }                    
                }

                if (goalTeam == LobbyPlayerLogic.TeamName.Red)
                {
                    textToShow = matchManager.blueTeamTextScore;
                    CmdBlueTeamScore();
                    if (matchManager.blueScore == matchManager.maxScore)
                    {
                        other.transform.parent.GetComponent<PlayerLogic>().CmdPlayEquipTeamSound("LevelMatchWon", "LevelMatchLost");
                        other.transform.parent.GetComponent<PlayerLogic>().CmdPlayGlobalSound("PlayerOverdriveGoal");
                    }
                    else
                    {
                        other.transform.parent.GetComponent<PlayerLogic>().CmdPlayEquipTeamSound("LevelTeamScores", "LevelEnemyScores");
                        other.transform.parent.GetComponent<PlayerLogic>().CmdPlayGlobalSound("PlayerOverdriveGoal");
                    }
                }
                CmdTeamWin(textToShow);
                other.transform.parent.GetComponent<PlayerLogic>().CmdDropFlag();
            }
        }
        
    }


    public void PlayEffect()
    {
        goalEffect.Play();
    }

    public IEnumerator DelayManager()
    {
        delay = true;
        yield return new WaitForSeconds(5f);
        delay = false;
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
