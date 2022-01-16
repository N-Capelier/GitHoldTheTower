using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerGuide : MonoBehaviour
{
    [SerializeField]
    private ScriptableParamsPlayer selfParams;
    [SerializeField]
    private Text objectiveText;
    [SerializeField]
    private RectTransform objectiveCursor;
    [SerializeField]
    private RectTransform allyCursor;
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private float borderWidthRatio;
    [SerializeField]
    private RectTransform overdriveProgressionIcon;
    [SerializeField]
    private Vector2 overdriveProgressionMinMaxPos;
    [SerializeField]
    private Text announcementText;
    [SerializeField]
    private Image announcementBack;
    [SerializeField]
    private Color announcementBackEnemyColor, announcementBackAllyColor;
    [SerializeField]
    private Color reachGoalObjectiveColor, captureObjectiveColor, defendObjectiveColor, protectObjectiveColor;
    [SerializeField]
    private string flagCapturedByEnemyAnnouncement, flagCapturedByAllyAnnouncement, flagReturnedToCenterAnnouncement, flagCapturedByMeAnnouncement;
    [SerializeField]
    private Text allyScoreText, enemyScoreText;
    [SerializeField]
    private Image overdriveProgressionIconImage;
    [SerializeField]
    private Color allyColor, enemyColor;

    private PlayerLogic playerLogic;
    private MatchManager matchManager;
    private GameObject targetObject;
    private GameObject flag;
    private GameObject adverseGoal;
    private GameObject ownGoal;
    private GameObject playerHoldingFlag;

    private List<PlayerLogic> teamMates;
    private List<PlayerLogic> adversaries;
    private bool playersSetUp;
    private Vector3 overdriveCurrentPosition;
    private Image objectiveCursorImage;
    private Color textBaseColor;
    Vector3 forwardAxis;
    private void Start()
    {
        flag = GameObject.Find("Flag");
        matchManager = GameObject.Find("GameManager").GetComponent<MatchManager>();
        playerLogic = GetComponent<PlayerLogic>();
        objectiveCursorImage = objectiveCursor.GetComponent<Image>();
        ownTeamHasOverdrive = false;
        overdriveIsInCenter = true;
        textBaseColor = announcementText.color;
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
    }

    Vector2 viewportPosition;
    Vector2 screenPos;
    Vector3 targetDirection;
    float angleFromTarget;

    bool ownTeamHasOverdrive;
    bool overdriveIsInCenter;
    private void Update()
    {
        if (playerLogic.roundStarted && !playersSetUp)
        {
            if (playerLogic.teamName == LobbyPlayerLogic.TeamName.Red)
            {
                adverseGoal = matchManager.blueGoal.gameObject;
                ownGoal = matchManager.redGoal.gameObject;
            }
            else
            {
                adverseGoal = matchManager.redGoal.gameObject;
                ownGoal = matchManager.blueGoal.gameObject;
            }
            GetAllPlayers();
            playersSetUp = true;
        }


        if (playerLogic.hasAuthority && playersSetUp)
        {
            if (targetObject != null)
            {
                viewportPosition = playerCamera.WorldToScreenPoint(targetObject.transform.position);
                screenPos = ((viewportPosition * 1920) / playerCamera.scaledPixelWidth) - new Vector2(1920, 1080) / 2;
                targetDirection = targetObject.transform.position - playerCamera.transform.position;
                angleFromTarget = Vector3.Angle(playerCamera.transform.forward, targetDirection);
                if (angleFromTarget > 90)
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
                    if (overdriveIsInCenter)
                    {
                        overdriveIsInCenter = false;
                        if (adversaryHasFlag)
                        {
                            StartCoroutine(Announce(flagCapturedByEnemyAnnouncement, false));
                        }
                        else
                        {
                            StartCoroutine(Announce(flagCapturedByAllyAnnouncement, true));
                        }
                    }
                    else
                    {
                        if (adversaryHasFlag && ownTeamHasOverdrive)
                        {
                            ownTeamHasOverdrive = false;
                            StartCoroutine(Announce(flagCapturedByEnemyAnnouncement, false));
                        }

                        if(teamMateHasFlag && !ownTeamHasOverdrive)
                        {
                            ownTeamHasOverdrive = true;
                            StartCoroutine(Announce(flagCapturedByAllyAnnouncement, true));
                        }
                    }

                    if (adversaryHasFlag)
                    {
                        objectiveText.text = selfParams.defendText;
                        objectiveCursorImage.color = defendObjectiveColor;
                        overdriveProgressionIconImage.color = enemyColor;
                    }
                    else
                    {
                        objectiveText.text = selfParams.protectText;
                        objectiveCursorImage.color = protectObjectiveColor;
                        overdriveProgressionIconImage.color = allyColor;
                    }

                    targetObject = playerHoldingFlag;
                    overdriveCurrentPosition = targetObject.transform.position;
                }
                else
                {
                    if(!overdriveIsInCenter)
                    {
                        overdriveIsInCenter = true;
                        ownTeamHasOverdrive = false;
                        StartCoroutine(Announce(flagReturnedToCenterAnnouncement, true));
                    }

                    objectiveText.text = selfParams.captureOverdriveText;
                    objectiveCursorImage.color = captureObjectiveColor;
                    targetObject = flag;
                    overdriveCurrentPosition = flag.transform.position;
                    overdriveProgressionIconImage.color = Color.white;
                }
            }
            else
            {
                if (!ownTeamHasOverdrive)
                {
                    overdriveIsInCenter = false;
                    ownTeamHasOverdrive = true;
                    StartCoroutine(Announce(flagCapturedByMeAnnouncement, true));
                }
                targetObject = adverseGoal;
                overdriveCurrentPosition = transform.position;
                objectiveText.text = selfParams.goToGoalText;
                objectiveCursorImage.color = reachGoalObjectiveColor;
                overdriveProgressionIconImage.color = allyColor;
            }

            Vector3 oDirectionFromOwnGoal = overdriveCurrentPosition - ownGoal.transform.position;
            float overdriveDistanceFromOwnGoal = oDirectionFromOwnGoal.magnitude;
            Vector3 oDirectionFromAdverseGoal = overdriveCurrentPosition - adverseGoal.transform.position;
            float overdriveDistanceFromAdverseGoal = oDirectionFromAdverseGoal.magnitude;

            float oProgressionRatio = overdriveDistanceFromOwnGoal / (overdriveDistanceFromAdverseGoal + overdriveDistanceFromOwnGoal);
            overdriveProgressionIcon.anchoredPosition = new Vector2(Mathf.Lerp(overdriveProgressionMinMaxPos.x, overdriveProgressionMinMaxPos.y, oProgressionRatio), overdriveProgressionIcon.anchoredPosition.y);

            if(teamMates.Count > 0)
            {
                viewportPosition = playerCamera.WorldToScreenPoint(teamMates[0].transform.position);
                screenPos = ((viewportPosition * 1920) / playerCamera.scaledPixelWidth) - new Vector2(1920, 1080) / 2;
                targetDirection = teamMates[0].transform.position - playerCamera.transform.position;
                angleFromTarget = Vector3.Angle(playerCamera.transform.forward, targetDirection);
                if (angleFromTarget > 90)
                {
                    screenPos *= -1;
                    while (screenPos.x < (1920 / 2 - borderWidthRatio) && screenPos.x > -(1920 / 2 - borderWidthRatio) && screenPos.y < (1080 / 2 - borderWidthRatio) && screenPos.y > -(1080 / 2 - borderWidthRatio))
                    {
                        screenPos += screenPos.normalized;
                    }
                }
                screenPos = new Vector2(Mathf.Clamp(screenPos.x, -(1920 / 2 - borderWidthRatio), (1920 / 2 - borderWidthRatio)), Mathf.Clamp(screenPos.y, -(1080 / 2 - borderWidthRatio), (1080 / 2 - borderWidthRatio)));
                allyCursor.anchoredPosition = screenPos;
                allyCursor.gameObject.SetActive(true);
            }
            else
            {
                allyCursor.gameObject.SetActive(false);
            }

            if (playerLogic.teamName == LobbyPlayerLogic.TeamName.Red)
            {
                allyScoreText.text = matchManager.redScore.ToString();
                enemyScoreText.text = matchManager.blueScore.ToString();
            }
            else
            {
                allyScoreText.text = matchManager.blueScore.ToString();
                enemyScoreText.text = matchManager.redScore.ToString();
            }
        }
    }

    private IEnumerator Announce(string announcement, bool isFriendly)
    {
        announcementBack.gameObject.SetActive(true);
        float timer = 0.5f;
        announcementText.text = announcement;
        while(timer > 0)
        {
            announcementText.color = Color.Lerp(textBaseColor, Color.clear, timer / 0.5f);
            announcementBack.color = Color.Lerp(isFriendly ? announcementBackAllyColor : announcementBackEnemyColor, Color.clear, timer / 0.5f);
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }

        yield return new WaitForSeconds(2);
        timer = 0.5f;
        while (timer > 0)
        {
            announcementText.color = Color.Lerp(Color.clear, textBaseColor, timer / 0.5f);
            announcementBack.color = Color.Lerp(Color.clear, isFriendly ? announcementBackAllyColor : announcementBackEnemyColor, timer / 0.5f);
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }
        announcementText.text = string.Empty;
        announcementBack.gameObject.SetActive(false);
    }
}
