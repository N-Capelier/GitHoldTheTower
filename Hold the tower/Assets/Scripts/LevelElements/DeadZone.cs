using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class DeadZone : MonoBehaviour
{

    public delegate void PlayerDeath(bool isTagged);
    public static event PlayerDeath OnDeath;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            RespawnAPlayer(other.gameObject);
        }
    }

    private void RespawnAPlayer(GameObject player)
    {
        PlayerLogic logic = player.transform.parent.GetComponent<PlayerLogic>();

        InvokePlayerDeath(logic);

        logic.RpcRespawn(player.transform.parent.GetComponent<NetworkIdentity>().connectionToClient, 3f);
    }

    public static void InvokePlayerDeath(PlayerLogic logic)
	{
        OnDeath?.Invoke(logic.isTagged);
        logic.taggedTimer.Stop();
        logic.isTagged = false;
    }
}
