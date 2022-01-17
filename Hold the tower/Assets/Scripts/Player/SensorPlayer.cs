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

    Clock shockwaveDelayTimer;
    bool isShockwaveAvailable = true;

	private void Start()
	{
        shockwaveDelayTimer = new Clock();
        shockwaveDelayTimer.ClockEnded += OnShockwaveComputed;
	}

	private void OnDestroy()
	{
        shockwaveDelayTimer.ClockEnded -= OnShockwaveComputed;
	}

	void OnShockwaveComputed()
	{
        isShockwaveAvailable = true;
    }

	private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            selfLogic.StartHitUi(0.5f);
            selfLogic.CmdCreateParticulePunch(transform.position);
            selfLogic.CmdPlayerSource("PlayerPunchImpact");
            if (other.transform.parent.GetComponent<PlayerLogic>().hasFlag)
            {
                selfTransform.GetComponent<PlayerLogic>().CmdGetFlag();
                selfLogic.CmdPlayGlobalSound("PlayerOverdriveSteal");
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

            if(block!= null && block.isButton)
			{
                if(block.buttonActiveTerrainIndex == block.loadedTerrainID)
				{
                    GameObject.Find("GameManager").GetComponent<ThemeInteraction>().CmdSwitchArea(block.blockID);
                    block.StartButtonActivationEffect();
                }
				return;
			}

            if(block != null)
            {
                if (isShockwaveAvailable && block.isDestroyable)
                {
                    isShockwaveAvailable = false;
                    shockwaveDelayTimer.SetTime(1f);

                    Vector3 _shockwaveSpawnPoint = RayCollisionPoint(other);

                    GameObject.Find("GameManager").GetComponent<ThemeInteraction>().CmdInstantiateShockwave(_shockwaveSpawnPoint, selfMovement.punchRatio);

                    if (selfMovement.isPunchInstantDestroy)
                    {
                        GameObject _shockWave = Instantiate(shockwavePrefab, _shockwaveSpawnPoint, Quaternion.identity);
                        _shockWave.GetComponent<ShockwaveCollider>().Shock(selfMovement.punchRatio);
                        GameObject.Find("GameManager").GetComponent<ThemeInteraction>().CmdExplode(block.blockID);
                    }
                    else
                    {
                        GameObject _shockWave = Instantiate(shockwavePrefab, _shockwaveSpawnPoint, Quaternion.identity);
                        _shockWave.GetComponent<ShockwaveCollider>().Shock(selfMovement.punchRatio);
                    }
                }
            }
		}
	}

    Vector3 RayCollisionPoint(Collider _impactCollider)
	{
        RaycastHit _hit;
        //Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 2f, Color.blue, 2f);
        //Debug.DrawRay(playerCamera.transform.position, _impactCollider.gameObject.transform.position - playerCamera.transform.position, Color.yellow, 2f);
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, 10f, wallLayerMask))
		{
            if(_hit.collider == _impactCollider)
			{
                return _hit.point;
			}
		}
        else if(Physics.Raycast(playerCamera.transform.position, (_impactCollider.gameObject.transform.position - playerCamera.transform.position).normalized * 2f, out _hit, 10f, wallLayerMask))
		{
            if (_hit.collider == _impactCollider)
            {
                return _hit.point;
            }
        }
        return _impactCollider.transform.position;
	}

}
