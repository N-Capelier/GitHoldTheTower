using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerGuide : MonoBehaviour
{
    [SerializeField]
    private RectTransform objectiveCursor;
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private float borderWidthRatio;

    private PlayerLogic playerLogic;
    private MatchManager matchManager;
    private GameObject targetObject;
    private GameObject flag;
    private GameObject adverseGoal;
    private GameObject playerHoldingFlag;

    private List<PlayerLogic> teamMates;
    private List<PlayerLogic> adversaries;
    private bool playersSetUp;

    Vector3 forwardAxis;
    private void Start()
    {
        flag = GameObject.Find("Flag");
        matchManager = GameObject.Find("GameManager").GetComponent<MatchManager>();
        playerLogic = GetComponent<PlayerLogic>();
    }


    private void GetAllPlayers()
    {
        teamMates = new List<PlayerLogic>();
        adversaries = new List<PlayerLogic>();

        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject playerObject in allPlayers)
        {
            PlayerLogic logic = playerObject.GetComponent<PlayerLogic>();
            if (logic != null)
            {
                if (logic.teamName == playerLogic.teamName)
                {
                    if (logic != playerLogic)
                    {
                        teamMates.Add(logic);
                    }
                    else
                    {

                    }
                }
                else
                {
                    adversaries.Add(logic);
                }
            }
        }
        /*
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            Debug.Log("found value > " + playerLogic.teamName.ToString() + " : " + conn);
            foreach (NetworkIdentity idOwnedByClient in conn.clientOwnedObjects)
            {
                PlayerLogic logic = idOwnedByClient.gameObject.GetComponent<PlayerLogic>();

                Debug.Log(playerLogic.teamName.ToString() + " : " + idOwnedByClient);

                if (logic != null)
                {
                    if (logic.teamName == playerLogic.teamName)
                    {
                        if(logic != playerLogic)
                        {
                            teamMates.Add(logic);
                            Debug.Log("teamate added");
                        }
                        else
                        {
                            Debug.Log("skip ading myself");
                        }
                    }
                    else
                    {
                        adversaries.Add(logic);
                        Debug.Log("enemy added");
                    }
                }
            }
        }*/
    }

    private void Update()
    {
        if (playerLogic.roundStarted && !playersSetUp)
        {
            if (playerLogic.teamName == LobbyPlayerLogic.TeamName.Red)
            {
                adverseGoal = matchManager.blueGoal.gameObject;
            }
            else
            {
                adverseGoal = matchManager.redGoal.gameObject;
            }
            GetAllPlayers();
            playersSetUp = true;
        }


        if (playerLogic.hasAuthority && playersSetUp)
        {
            if (targetObject != null)
            {
                Vector2 viewportPosition = playerCamera.WorldToScreenPoint(targetObject.transform.position);
                Vector2 screenPos = ((viewportPosition * 1920) / playerCamera.scaledPixelWidth) - new Vector2(1920, 1080) / 2;
                Vector3 flagDirection = targetObject.transform.position - playerCamera.transform.position;
                float angleFromFlag = Vector3.Angle(playerCamera.transform.forward, flagDirection);
                if (angleFromFlag > 90)
                {
                    screenPos *= -1;
                    while (screenPos.x < (1920 / 2 - borderWidthRatio) && screenPos.x > -(1920 / 2 - borderWidthRatio) && screenPos.y < (1080 / 2 - borderWidthRatio) && screenPos.y > -(1080 / 2 - borderWidthRatio))
                    {
                        screenPos += screenPos.normalized;
                    }
                }
                screenPos = new Vector2(Mathf.Clamp(screenPos.x, -(1920 / 2 - borderWidthRatio), (1920 / 2 - borderWidthRatio)), Mathf.Clamp(screenPos.y, -(1080 / 2 - borderWidthRatio), (1080 / 2 - borderWidthRatio)));
                objectiveCursor.anchoredPosition = screenPos;
                objectiveCursor.gameObject.SetActive(true);
            }
            else
            {
                objectiveCursor.gameObject.SetActive(false);
            }

            if (!playerLogic.hasFlag)
            {
                bool teamMateHasFlag = false;
                bool adversaryHasFlag = false;
                foreach (PlayerLogic player in adversaries)
                {
                    if (player.hasFlag)
                    {
                        adversaryHasFlag = true;
                        playerHoldingFlag = player.gameObject;
                    }
                }

                if (!adversaryHasFlag)
                {
                    foreach (PlayerLogic player in teamMates)
                    {
                        if (player.hasFlag)
                        {
                            teamMateHasFlag = true;
                            playerHoldingFlag = player.gameObject;
                        }
                    }
                }

                if (adversaryHasFlag || teamMateHasFlag)
                {
                    targetObject = playerHoldingFlag;
                }
                else
                {
                    targetObject = flag;
                }
            }
            else
            {
                targetObject = adverseGoal;
            }
        }
    }
}
