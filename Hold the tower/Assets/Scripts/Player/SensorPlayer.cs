using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

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
        else if(other.CompareTag("Wall"))
		{
            BlockBehaviour block = other.GetComponent<BlockBehaviour>();
            if (block.gameObject.name.Contains("("))
                GameObject.Find("GameManager").GetComponent<ThemeInteration>().CmdWaitAndExplode(int.Parse(new string(block.gameObject.name.Where(char.IsDigit).ToArray())));
            else
                GameObject.Find("GameManager").GetComponent<ThemeInteration>().CmdWaitAndExplode(0);
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
