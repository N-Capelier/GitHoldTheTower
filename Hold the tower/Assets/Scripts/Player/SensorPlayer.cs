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
    public Transform selfTransform;

    [SerializeField] Camera playerCamera;
    [SerializeField] LayerMask wallLayerMask;
    [SerializeField] GameObject shockwavePrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            selfLogic.StartHitUi(0.5f);
            selfLogic.CmdCreateParticulePunch(transform.position);
            if (other.transform.parent.GetComponent<PlayerLogic>().hasFlag)
            {
                selfTransform.GetComponent<PlayerLogic>().CmdGetFlag();

            }

            selfTransform.GetComponent<PlayerMovement>().StopPunch();
            other.transform.parent.GetComponent<PlayerLogic>().CmdGetPunch(other.transform.parent.GetComponent<NetworkIdentity>(), selfMovement.directionAttack * selfParams.punchBasePropulsionForce);
        }
        else if(other.CompareTag("Wall"))
        {
            if (Mathf.Abs(selfMovement.directionAttack.y) < 0.6f)
            {
                selfTransform.GetComponent<PlayerMovement>().StopPunch();
                selfLogic.CmdPropulse(-selfMovement.directionAttack.normalized * 15);
            }
            BlockBehaviour block = other.GetComponent<BlockBehaviour>();


            if(block.isButton)
			{
                if(block.buttonActiveTerrainIndex == block.loadedTerrainID)
				{
                    GameObject.Find("GameManager").GetComponent<ThemeInteration>().CmdSwitchArea(block.blockID);
                    block.StartButtonActivationEffect();
                }
				return;
			}

            if(block.isDestroyable)
			{
                Vector3 _shockwaveSpawnPoint = RayCollisionPoint(other);

                if (selfMovement.isPerfectTiming)
                {
                    GameObject _shockWave = Instantiate(shockwavePrefab, _shockwaveSpawnPoint, Quaternion.identity);
                    _shockWave.GetComponent<ShockwaveCollider>().Shock(1f);
                    GameObject.Find("GameManager").GetComponent<ThemeInteration>().CmdExplode(block.blockID);
                }
                else
                {
                    GameObject _shockWave = Instantiate(shockwavePrefab, _shockwaveSpawnPoint, Quaternion.identity);
                    ShockwaveCollider _shockWaveCollider = _shockWave.GetComponent<ShockwaveCollider>();
                    _shockWaveCollider.Shock(1f);
                }
            }
		}
	}

    Vector3 RayCollisionPoint(Collider _impactCollider)
	{
        RaycastHit _hit;
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward, Color.blue, 2f);
        Debug.DrawRay(playerCamera.transform.position, _impactCollider.gameObject.transform.position - playerCamera.transform.position, Color.yellow, 2f);
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, Mathf.Infinity, wallLayerMask))
		{
            if(_hit.collider == _impactCollider)
			{
                Debug.LogWarning("Impact point from camera");
                return _hit.point;
			}
		}
        else if(Physics.Raycast(playerCamera.transform.position, _impactCollider.gameObject.transform.position - playerCamera.transform.position, out _hit, Mathf.Infinity, wallLayerMask))
		{
            if (_hit.collider == _impactCollider)
            {
                Debug.LogWarning("Impact point from vector");
                return _hit.point;
            }
        }
        Debug.LogWarning("Impact point from transform");
        return _impactCollider.transform.position;
	}

}
