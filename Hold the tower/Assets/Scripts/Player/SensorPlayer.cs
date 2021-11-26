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
        if (other.CompareTag("Player"))
        {
            other.transform.parent.GetComponent<PlayerLogic>().GetHit(transform.parent.transform.parent);
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
