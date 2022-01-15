using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerPing : NetworkBehaviour
{
    [Header("Reférence")]
    [SerializeField]
    private ScriptableParamsPlayer selfParams;
    [SerializeField]
    private GameObject selfCamera;
    [SerializeField]
    private PlayerLogic selfLogic;
    [SerializeField]
    private PlayerGuide selfGuide;
    [SerializeField]
    private GameObject selfPingObject;

    private int layerMaskBlock, layerMaskBlockRed, layerMaskBlockBlue, layerMaskBlockGreen;

    private Vector3 positionToPing = new Vector3(0,0,0);

    private bool isPinging;
    private Vector3 offSetPing;

    [Header("Params")]
    [SerializeField]
    private float pingTime = 7.5f;

    void Start()
    {
        layerMaskBlock = LayerMask.NameToLayer("Outlined");
        layerMaskBlockRed = LayerMask.NameToLayer("OutlinedRed");
        layerMaskBlockBlue = LayerMask.NameToLayer("OutlinedBlue");
        layerMaskBlockGreen = LayerMask.NameToLayer("OutlinedGreen");

        offSetPing = selfPingObject.transform.localPosition;
        selfPingObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority)
        {
            if (Input.GetMouseButton(selfParams.pingMouseInput))
            {
                RaycastHit hit;
                int layerMask = (1 << layerMaskBlock) | (1 << layerMaskBlockRed) | (1 << layerMaskBlockBlue) | (1 << layerMaskBlockGreen);
                if (Physics.Raycast(selfCamera.transform.position, selfCamera.transform.forward, out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawRay(selfCamera.transform.position, selfCamera.transform.forward * hit.distance, Color.yellow, 5);
                    positionToPing = hit.point + offSetPing;
                    isPinging = true;                    
                }
            }

            if (Input.GetMouseButtonUp(selfParams.pingMouseInput))
            {
                isPinging = false;
                StopCoroutine(PingAlive(positionToPing));
                StartCoroutine(PingAlive(positionToPing));
            }

            if (!isPinging && !Input.GetMouseButton(selfParams.pingMouseInput))
            {
                selfPingObject.transform.position = positionToPing;

            }

        }
        else
        {
            selfPingObject.transform.position = positionToPing;
        }

    }


    //Command to ping ally
    [Command]
    private void CmdPingAllies(Vector3 pos)
    {
        RpcPingAllies(pos);
    }

    [ClientRpc]
    private void RpcPingAllies(Vector3 pos)
    {
        if (selfLogic.authorityPlayer.GetComponent<PlayerLogic>().teamName == selfLogic.teamName)
        {
            positionToPing = pos;
            selfPingObject.transform.position = pos;
            selfPingObject.SetActive(true);
        }
        
    }


    //Command to destroy ping ally
    [Command(requiresAuthority =false)]
    private void CmdPingAlliesDie()
    {
        RpcPingAlliesDie();
    }

    [ClientRpc]
    private void RpcPingAlliesDie()
    {
        selfPingObject.SetActive(false);
    }
    private IEnumerator PingAlive(Vector3 pos)
    {
        selfLogic.CmdPlayGlobalSound("PlayerPing");
        selfPingObject.SetActive(true);
        CmdPingAllies(pos);
        float lifeTime = 0f;
        while(lifeTime < pingTime)
        {
            lifeTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        CmdPingAlliesDie();
        selfPingObject.SetActive(false);
    }
}
