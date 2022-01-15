using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorWallDie : MonoBehaviour
{   
    public PlayerLogic selfLogic;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") && other.GetComponent<BlockBehaviour>() != null)
        {
            if(other.GetComponent<BlockBehaviour>().ownVelo.y != 0 && selfLogic.isGrounded)
            {
                selfLogic.CmdForceRespawn(3f);
            }
        }
    }
}
