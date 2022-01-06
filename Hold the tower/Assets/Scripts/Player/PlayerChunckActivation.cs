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

    private ChunckSwitcher aimedSwitcher;
    private bool isAimingSwitcher;
    private void Update()
    {
        CheckPlayerAim();
    }

    public void CheckPlayerAim()
    {
        isAimingSwitcher = false;
        aimedSwitcher = null;
        RaycastHit hit;
        Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, selfParams.switchChunckMaxDistance, switcherLayer);

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

        if (isAimingSwitcher)
        {
            switchInputIndicator.gameObject.SetActive(true);
            switchInputIndicator.fillAmount = aimedSwitcher.linkedChunck.GetCDRatio();
            if(Input.GetKeyDown(selfParams.switchChunckKey))
            {
                if(aimedSwitcher.linkedChunck.GetCDRatio() == 1)
                {
                    aimedSwitcher.linkedChunck.CmdUse();
                    aimedSwitcher.SwitchChunck();
                }
            }
        }
        else
        {
            switchInputIndicator.gameObject.SetActive(false);
        }
    }
}
