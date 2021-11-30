using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class SensorPlayer : MonoBehaviour
{

    [SerializeField]
    private UnityEvent collidePlayer;

    [SerializeField]
    private PlayerLogic selfLogic;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.transform.parent.GetComponent<PlayerLogic>().hasFlag)
        {
            collidePlayer.Invoke();
            other.transform.parent.GetComponent<PlayerLogic>().CmdDropFlag();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //other.GetComponent<PlayerLogic>().cantBeHit();
        }

    }
}
