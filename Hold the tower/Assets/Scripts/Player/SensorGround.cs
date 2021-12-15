using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SensorGround : MonoBehaviour
{
    public UnityEvent collide;
    public UnityEvent Uncollide;

    private int nbCollide = 0;

    [SerializeField]
    private PlayerMovement selfMovement;
    [SerializeField]
    private PlayerLogic selfLogic;

    [SerializeField]
    private Transform selfTransform;
    [SerializeField]
    private Rigidbody selfRbd;

    private GameObject target;
    private Vector3 offset;
    private BlockBehaviour contactGround;

    public void Update()
    {
        if (contactGround != null && !contactGround.boxCollider.enabled)
        {
            nbCollide--;
            if (nbCollide == 0)
            {
                contactGround = null;
                Uncollide.Invoke();
            }
        }
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            nbCollide++;
            if (nbCollide == 1)
            {
                contactGround = other.GetComponent<BlockBehaviour>();
                collide.Invoke();
            }

            //Debug.Log(transform.name);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            nbCollide--;
            if (nbCollide == 0)
            {
                contactGround = null;
                Uncollide.Invoke();
                target = null;
            }
            
            //Debug.Log(transform.name);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            target = other.gameObject;
   
        }

    }

    void FixedUpdate()
    {
        if (target != null  && selfLogic.isGrounded && !selfMovement.isClimbingMovement)
        {
            if (selfLogic.hasAuthority)
            {
                
            }
            else
            {
                selfRbd.velocity += target.GetComponent<BlockBehaviour>().ownVelo;
            }

            ////////////////////////// Add BlockBehaviour to spawn blocks
            if(target.GetComponent<BlockBehaviour>())
			{
				selfTransform.position += target.GetComponent<BlockBehaviour>().ownVelo;
				//selfRbd.velocity += target.GetComponent<BlockBehaviour>().ownVelo;

			}
            //Debug.Log(target.GetComponent<BlockBehaviour>().ownVelo);
        }

    }
}
