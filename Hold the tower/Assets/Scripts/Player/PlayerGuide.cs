using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGuide : MonoBehaviour
{
    [SerializeField]
    private RectTransform objectiveCursor;
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private RectTransform canvas;
    [SerializeField]
    private float borderWidth;

    private PlayerLogic playerLogic;
    private MatchManager matchManager;
    private GameObject targetObject;
    private GameObject flag;
    private GameObject adverseGoal;

    Vector3 forwardAxis;
    private void Start()
    {
        flag = GameObject.Find("Flag");
        matchManager = GameObject.Find("GameManager").GetComponent<MatchManager>();
        playerLogic = GetComponent<PlayerLogic>();
        if(playerLogic.teamName == LobbyPlayerLogic.TeamName.Red)
        {
            adverseGoal = matchManager.blueGoal.gameObject;
        }
        else
        {
            adverseGoal = matchManager.redGoal.gameObject;
        }
    }
    private void Update()
    {
        if(targetObject != null)
        {
            Vector2 viewportPosition = playerCamera.WorldToScreenPoint(targetObject.transform.position);
            Vector2 screenPos = viewportPosition - new Vector2(playerCamera.scaledPixelWidth, playerCamera.scaledPixelHeight) / 2;
            Vector3 flagDirection = targetObject.transform.position - playerCamera.transform.position;
            float angleFromFlag = Vector3.Angle(playerCamera.transform.forward, flagDirection);
            if (angleFromFlag > 90)
            {
                screenPos *= -1;
                while(screenPos.x < (960 - borderWidth) && screenPos.x > -(960 - borderWidth) && screenPos.y < (540 - borderWidth) && screenPos.y > -(540 - borderWidth))
                {
                    screenPos += screenPos.normalized;
                }
            }
            screenPos = new Vector2(Mathf.Clamp(screenPos.x, -(960 - borderWidth), (960 - borderWidth)), Mathf.Clamp(screenPos.y, -(540 - borderWidth), (540 - borderWidth)));
            objectiveCursor.anchoredPosition = screenPos;
            objectiveCursor.gameObject.SetActive(true);
        }
        else
        {
            objectiveCursor.gameObject.SetActive(false);
        }

        if(!playerLogic.hasFlag)
        {
            targetObject = flag;
        }
        else
        {
            targetObject = adverseGoal;
        }
    }

}
