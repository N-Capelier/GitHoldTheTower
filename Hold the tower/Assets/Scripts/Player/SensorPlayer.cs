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

            if(!block.isButton)
			{
                GameObject.Find("GameManager").GetComponent<ThemeInteration>().CmdWaitAndExplode(BlockHelper.GetBlockID(block.gameObject.name));
            }
            else
			{
                int[] _indexes = new int[block.switchables.Length];
				for (int i = 0; i < _indexes.Length; i++)
				{
                    _indexes[i] = BlockHelper.GetBlockID(block.switchables[i].gameObject.name);
				}
			}
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
