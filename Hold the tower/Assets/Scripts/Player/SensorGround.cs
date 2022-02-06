using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SensorGround : MonoBehaviour
{
    public UnityEvent collide;
    public UnityEvent Uncollide;

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
    private BoxCollider bcollider;
    private Collider[] grounds;
    private bool isOnGround;

    private void Start()
    {
        bcollider = GetComponent<BoxCollider>();
    }

    public void Update()
    {

        grounds = Physics.OverlapBox(bcollider.bounds.center, bcollider.bounds.extents, transform.rotation, LayerMask.GetMask("Outlined"));
        if(grounds.Length > 0)
        {
            if(!isOnGround)
            {
                collide.Invoke();
                isOnGround = true;
            }
        }
        else
        {
            if (isOnGround)
            {
                Uncollide.Invoke();
                isOnGround = false;
            }
        }

    }
    /*
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            nbCollide++;
            contactGround = other.GetComponent<BlockBehaviour>();
            if (nbCollide == 1)
            {
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
    */
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
                if (selfLogic.hasAuthority)
                {
                    selfTransform.position += target.GetComponent<BlockBehaviour>().ownVelo;
                }
                else
                {
                    selfTransform.position += target.GetComponent<BlockBehaviour>().ownVelo*2f;
                }
            }
            //Debug.Log(target.GetComponent<BlockBehaviour>().ownVelo);
        }

    }
}
