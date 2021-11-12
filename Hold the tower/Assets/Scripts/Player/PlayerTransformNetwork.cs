using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerTransformNetwork : NetworkBehaviour
{
    [SyncVar]
    private Vector3 realPos;

    private void Update()
    {
        if (hasAuthority)
        {
            //transmitPosition(transform.position);
        }
        else
        {
            //transform.position = Vector3.Lerp(transform.position, realPos, 0.1f);
        }
        
    }


    [Command]
    void transmitPosition(Vector3 pos)
    {
        realPos = pos;
    }

    /*public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        writer.Write(transform.position);
        return true;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        Debug.Log(reader);
        if (isLocalPlayer)
        {
            return;
        }

        realPos = reader.ReadVector3();
    }*/
    
}
