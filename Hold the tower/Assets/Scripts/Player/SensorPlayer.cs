using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
            if (other.transform.parent.GetComponent<PlayerLogic>().hasFlag)
            {
                selfTransform.GetComponent<PlayerLogic>().CmdGetFlag();
            }

            selfTransform.GetComponent<PlayerMovement>().StopPunch();
            other.transform.parent.GetComponent<PlayerLogic>().CmdGetPunch(other.transform.parent.GetComponent<NetworkIdentity>(), selfMovement.directionAttack * selfParams.punchBasePropulsionForce );
        }
        else if(other.CompareTag("Wall"))
		{
            BlockBehaviour block = other.GetComponent<BlockBehaviour>();

            if(!block.isButton)
			{
                //Problème ici
                GameObject.Find("GameManager").GetComponent<ThemeInteration>().CmdWaitAndExplode(block.blockID);
            }
            else
			{
                int[] _indexes = new int[block.buttonManager.switchables.Length];
				for (int i = 0; i < _indexes.Length; i++)
				{
                    _indexes[i] = block.buttonManager.switchables[i].blockID;
                }
                //et ici
                GameObject.Find("GameManager").GetComponent<ThemeInteration>().CmdSwitchArea(_indexes);

            }
		}
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        //other.GetComponent<PlayerLogic>().cantBeHit();
    //    }

    //}
}
