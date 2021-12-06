using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Mirror;

public class SensorPlayer : MonoBehaviour
{

    [SerializeField]
    private UnityEvent collidePlayer;

    [SerializeField]
    private PlayerMovement selfMovement;
    [SerializeField]
    private PlayerLogic selfLogic;
    [SerializeField]
    private Transform selfCameraTransform;
    [SerializeField]
    private ScriptableParamsPlayer selfParams;

    private Transform selfTransform;

    public void Start()
    {
        selfTransform = transform.parent.transform.parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            selfTransform.GetComponent<PlayerMovement>().StopPunch();
            other.transform.parent.GetComponent<PlayerLogic>().CmdGetPunch(other.transform.parent.GetComponent<NetworkIdentity>(), selfMovement.directionAttack * selfParams.punchBasePropulsionForce);
            if (other.transform.parent.GetComponent<PlayerLogic>().hasFlag)
            {
                collidePlayer.Invoke();
                other.transform.parent.GetComponent<PlayerLogic>().CmdDropFlag();
            }
        }
        else if(other.CompareTag("Wall"))
		{
            BlockBehaviour block = other.GetComponent<BlockBehaviour>();

            if(!block.isButton)
			{
                //GameObject.Find("GameManager").GetComponent<ThemeInteration>().CmdWaitAndExplode(BlockHelper.GetBlockID(block.gameObject.name));
            }
            else
			{
                int[] _indexes = new int[block.switchables.Length];
				for (int i = 0; i < _indexes.Length; i++)
				{
                    _indexes[i] = BlockHelper.GetBlockID(block.switchables[i].gameObject.name);
				}
                GameObject.Find("GameManager").GetComponent<ThemeInteration>().CmdSwitchArea(_indexes);

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
