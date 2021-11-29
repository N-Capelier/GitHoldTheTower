using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CollidEvent : UnityEvent<GameObject>
{

}

public class SensorPlayer : MonoBehaviour
{

    [SerializeField]
    private PlayerLogic selfLogic;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.transform.parent.GetComponent<PlayerLogic>().hasFlag)
        {
            //Change var hasflag of player that been tuch in false
            other.transform.parent.GetComponent<PlayerLogic>().hasFlag = false;

            //Change var of player that punch to true
            selfLogic.hasFlag = true;
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
