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
    private float borderWidthHorizontal;
    [SerializeField]
    private float borderWidthVertical;
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

    public Color allyColor, enemyColor;
    [SerializeField]
    private GameObject objectiveCursorEffect;

    private PlayerLogic playerLogic;
    private MatchManager matchManager;
    private Vector3 targetPos;
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
    int redScore;
    int blueScore;

    [HideInInspector]
    public bool ownTeamHasOverdrive;
    [HideInInspector]
    public bool overdriveIsInCenter;
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
            if (targetPos != Vector3.zero)
            {
                viewportPosition = playerCamera.WorldToScreenPoint(targetPos);
                screenPos = ((viewportPosition * 1920) / playerCamera.scaledPixelWidth) - new Vector2(1920, 1080) / 2;
                targetDirection = targetPos - playerCamera.transform.position;
                angleFromTarget = Vector3.Angle(playerCamera.transform.forward, targetDirection);
                if (angleFromTarget > 90)
                {
                    screenPos *= -1;
                    while (screenPos.x < (1920 / 2 - borderWidthHorizontal) && screenPos.x > -(1920 / 2 - borderWidthHorizontal) && screenPos.y < (1080 / 2 - borderWidthVertical) && screenPos.y > -(1080 / 2 - borderWidthVertical))
                    {
                        screenPos += screenPos.normalized;
                    }
                }
                screenPos = new Vector2(Mathf.Clamp(screenPos.x, -(1920 / 2 - borderWidthHorizontal), (1920 / 2 - borderWidthHorizontal)), Mathf.Clamp(screenPos.y, -(1080 / 2 - borderWidthVertical), (1080 / 2 - borderWidthVertical)));
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

                    targetPos = playerHoldingFlag.transform.position + Vector3.up;
                    overdriveCurrentPosition = targetPos;
                }
                else
                {
                    if (!overdriveIsInCenter)
                    {
                        overdriveIsInCenter = true;
                        ownTeamHasOverdrive = false;
                        if(redScore != matchManager.redScore || blueScore != matchManager.blueScore)
                        {
                            redScore = matchManager.redScore;
                            blueScore = matchManager.blueScore;
                        }
                        else
                        {
                            StartCoroutine(Announce(flagReturnedToCenterAnnouncement, false));
                        }
                    }


                    objectiveText.text = selfParams.captureOverdriveText;
                    objectiveCursorImage.color = captureObjectiveColor;
                    targetPos = flag.transform.position;
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
                targetPos = adverseGoal.transform.position;
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
                viewportPosition = playerCamera.WorldToScreenPoint(teamMates[0].transform.position + Vector3.up);
                screenPos = ((viewportPosition * 1920) / playerCamera.scaledPixelWidth) - new Vector2(1920, 1080) / 2;
                targetDirection = (teamMates[0].transform.position + Vector3.up) - playerCamera.transform.position;
                angleFromTarget = Vector3.Angle(playerCamera.transform.forward, targetDirection);
                if (angleFromTarget > 90)
                {
                    screenPos *= -1;
                    while (screenPos.x < (1920 / 2 - borderWidthHorizontal) && screenPos.x > -(1920 / 2 - borderWidthHorizontal) && screenPos.y < (1080 / 2 - borderWidthVertical) && screenPos.y > -(1080 / 2 - borderWidthVertical))
                    {
                        screenPos += screenPos.normalized;
                    }
                }
                screenPos = new Vector2(Mathf.Clamp(screenPos.x, -(1920 / 2 - borderWidthHorizontal), (1920 / 2 - borderWidthHorizontal)), Mathf.Clamp(screenPos.y, -(1080 / 2 - borderWidthVertical), (1080 / 2 - borderWidthVertical)));
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

    private IEnumerator WarnObjectiveCursor(int repetitionNumber)
    {
        for (int i = 0; i < repetitionNumber; i++)
        {
            Image effectImage = Instantiate(objectiveCursorEffect, objectiveCursor).GetComponent<Image>();
            effectImage.color = objectiveCursorImage.color;
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator Announce(string announcement, bool isFriendly)
    {
        StartCoroutine(WarnObjectiveCursor(5));
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
