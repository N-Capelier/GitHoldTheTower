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

    private void RespawnAPlayer(GameObject player)
    {
        Debug.Log(player.transform.parent);
        player.transform.parent.GetComponentInParent<PlayerLogic>().RpcRespawn(player.transform.parent.GetComponent<NetworkIdentity>().connectionToClient, 3f);
    }
}
