using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChunckActivation : MonoBehaviour
{
    [SerializeField]
    private Image switchInputIndicator;
    [SerializeField]
    private ScriptableParamsPlayer selfParams;
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private LayerMask switcherLayer;
    [SerializeField]
    private LayerMask switcherLayerOnly;
    [SerializeField]
    private bool needToBeInSight;
    [SerializeField]
    private bool useSwitcherCustomRange;
    [SerializeField]
    private LineRenderer switchLineDirection;

    private ChunckSwitcher aimedSwitcher;
    private bool isAimingSwitcher;
    private bool isNearbySwitcher;
    private PlayerLogic playerLogic;

    private void Start()
    {
        playerLogic = GetComponent<PlayerLogic>();
        DisableSwitchLine();
    }

    private void Update()
    {
        if(playerLogic.hasAuthority)
        {
            CheckPlayerAim();
            if(useSwitcherCustomRange)
            {
                CheckSwitcherPos();
            }
            else
            {
                CheckPlayerPos();
            }
            CheckPlayerActivation();
        }
    }

    public void CheckPlayerAim()
    {
        isAimingSwitcher = false;
        RaycastHit hit;
        Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, selfParams.switchChunckAimMaxDistance, switcherLayer);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Switcher"))
            {
                aimedSwitcher = hit.collider.GetComponent<ChunckSwitcher>();
                if (aimedSwitcher != null)
                {
                    isAimingSwitcher = true;
                }
            }
        }
    }

    float mindistFromSwitch;
    Vector3 directionToSwitcher;
    public void CheckPlayerPos()
    {
        isNearbySwitcher = false;
        Collider[] switcherColliders = Physics.OverlapSphere(transform.position, selfParams.switchChunckRangeMaxDistance, switcherLayerOnly);

        if (switcherColliders.Length > 0)
        {
            mindistFromSwitch = selfParams.switchChunckRangeMaxDistance + 50;
            for(int i = 0; i < switcherColliders.Length; i++)
            {
                directionToSwitcher = switcherColliders[i].transform.position - transform.position;

                RaycastHit hit;
                Physics.Raycast(transform.position, directionToSwitcher, out hit, selfParams.switchChunckRangeMaxDistance, switcherLayer);

                if (directionToSwitcher.magnitude < mindistFromSwitch && ((hit.collider != null && hit.collider.CompareTag("Switcher")) || !needToBeInSight))
                {
                    mindistFromSwitch = directionToSwitcher.magnitude;
                    if(aimedSwitcher != null)
                    {
                        aimedSwitcher.linkedChunck.HighlightChunck(false);
                        aimedSwitcher.UnSelect();
                        DisableSwitchLine();
                    }
                    aimedSwitcher = switcherColliders[i].GetComponent<ChunckSwitcher>();
                    isNearbySwitcher = true;
                }
            }
        }
    }
    public void CheckSwitcherPos()
    {
        isNearbySwitcher = false;
        mindistFromSwitch = 2000;
        Collider[] switcherColliders = Physics.OverlapSphere(transform.position, selfParams.switchChunckRangeMaxDistance, switcherLayerOnly);
        List<ChunckSwitcher> switcherNearby = new List<ChunckSwitcher>();
        ChunckSwitcher potentialSwitch;
        if (switcherColliders.Length > 0)
        {
            Debug.Log("switcher in lit");
            for (int i = 0; i < switcherColliders.Length; i++)
            {
                if (switcherColliders[i].CompareTag("Switcher"))
                {
                    potentialSwitch = switcherColliders[i].GetComponent<ChunckSwitcher>();
                    Debug.Log("added : " + potentialSwitch);
                    switcherNearby.Add(potentialSwitch);
                    isNearbySwitcher = true;
                }
            }
        }

        if (isNearbySwitcher)
        {
            potentialSwitch = null;

            foreach (ChunckSwitcher switcher in switcherNearby)
            {
                directionToSwitcher = switcher.transform.position - transform.position;

                if (directionToSwitcher.magnitude < switcher.activationRange && directionToSwitcher.magnitude < mindistFromSwitch)
                {
                    mindistFromSwitch = directionToSwitcher.magnitude;
                    if (potentialSwitch != null)
                    {
                        potentialSwitch.linkedChunck.HighlightChunck(false);
                        potentialSwitch.UnSelect();
                        DisableSwitchLine();
                    }
                    potentialSwitch = switcher;
                }
            }

            aimedSwitcher = potentialSwitch;
        }
    }

    public void CheckPlayerActivation()
    {
        if (isNearbySwitcher || isAimingSwitcher)
        {
            switchInputIndicator.gameObject.SetActive(true);
            switchInputIndicator.fillAmount = aimedSwitcher.linkedChunck.GetCDRatio();
            aimedSwitcher.linkedChunck.HighlightChunck(true);
            EnableSwitchLine(aimedSwitcher.gameObject);
            aimedSwitcher.Select();

            if (Input.GetKeyDown(selfParams.switchChunckKey) || Input.GetButtonDown("YButton"))
            {
                if (aimedSwitcher.linkedChunck.GetCDRatio() == 1)
                {
                    aimedSwitcher.linkedChunck.CmdUse();
                    aimedSwitcher.Use();
                    aimedSwitcher.SwitchChunck();
                    playerLogic.CmdPlayGlobalSound("LevelChunkMovement");
                    SoundManager.Instance.PlaySoundEvent("LevelButtonActivated");
                }
            }
        }
        else
        {
            switchInputIndicator.gameObject.SetActive(false);
            if (aimedSwitcher != null)
            {
                aimedSwitcher.linkedChunck.HighlightChunck(false);
                DisableSwitchLine();
                aimedSwitcher.UnSelect();
            }
        }
    }

    public void EnableSwitchLine(GameObject switcher)
    {
        switchLineDirection.enabled = true;
        switchLineDirection.SetPosition(0, transform.position);
        switchLineDirection.SetPosition(1, switcher.transform.position);
    }
    public void DisableSwitchLine()
    {
        switchLineDirection.enabled = false;
    }
}
