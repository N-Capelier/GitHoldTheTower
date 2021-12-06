using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DeadZone : MonoBehaviour
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            RespawnAPlayer(other.gameObject);
        }
    }

    [Server]
    private void RespawnAPlayer(GameObject player)
    {
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            player.GetComponentInParent<PlayerLogic>().RpcRespawn(conn, 3f);
        }
    }
}
