using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class SensorPlayer : MonoBehaviour
{

    [SerializeField]
    private UnityEvent collidePlayer;
    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    [SerializeField]
    private PlayerMovement selfMovement;
    [SerializeField]
    private PlayerLogic selfLogic;
    [SerializeField]
    private Transform selfCameraTransform;
    [SerializeField]
    private ScriptableParamsPlayer selfParams;
    public Transform selfTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(selfLogic.HitUi(1.5f));
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
                if(selfMovement.isPerfectTiming)
				{
                    GameObject.Find("GameManager").GetComponent<ThemeInteration>().CmdExplode(block.blockID);

                }
                else
				{
                    GameObject.Find("GameManager").GetComponent<ThemeInteration>().CmdWaitAndExplode(block.blockID);
                }
            }
            else if(block.buttonActiveTerrainIndex == block.loadedTerrainID)
			{
				GameObject.Find("GameManager").GetComponent<ThemeInteration>().CmdSwitchArea(block.blockID);
			}
		}
    }


}
