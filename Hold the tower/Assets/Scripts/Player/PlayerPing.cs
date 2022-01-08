using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerPing : NetworkBehaviour
{
    [SerializeField]
    private ScriptableParamsPlayer selfParams;
    [SerializeField]
    private GameObject selfCamera;
    [SerializeField]
    private GameObject selfPingObject;

    private int layerMaskBlock, layerMaskBlockRed, layerMaskBlockBlue, layerMaskBlockGreen;

    private Vector3 positionToPing = new Vector3(0,0,0);

    private bool isPinging;

    void Start()
    {
        layerMaskBlock = LayerMask.NameToLayer("Outlined");
        layerMaskBlockRed = LayerMask.NameToLayer("OutlinedRed");
        layerMaskBlockBlue = LayerMask.NameToLayer("OutlinedBlue");
        layerMaskBlockGreen = LayerMask.NameToLayer("OutlinedGreen");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(selfParams.pingMouseInput))
        {
            RaycastHit hit;
            int layerMask = (1 << layerMaskBlock) | (1 << layerMaskBlockRed) | (1 << layerMaskBlockBlue) | (1 << layerMaskBlockGreen);
            //layerMask = ~layerMask;
            if (Physics.Raycast(selfCamera.transform.position, selfCamera.transform.forward, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(selfCamera.transform.position, selfCamera.transform.forward * hit.distance, Color.yellow,5);
                positionToPing = hit.point;
                isPinging = true;
            }
        }

        if (Input.GetMouseButtonUp(selfParams.pingMouseInput))
        {
            isPinging = false;
            Debug.Log(positionToPing);
        }

        if (!isPinging && !Input.GetMouseButton(selfParams.pingMouseInput))
        {
            selfPingObject.transform.position = positionToPing;
        }

        
    }
}
