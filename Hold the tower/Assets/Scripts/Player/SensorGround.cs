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


    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            nbCollide++;
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
            offset = transform.position - target.transform.position;
            
        }

    }

    void FixedUpdate()
    {
        if (target != null  && selfLogic.isGrounded && !selfMovement.isClimbingMovement)
        {
            selfTransform.position = target.transform.position + offset;
            /*selfRbd.velocity += target.GetComponent<BlockBehaviour>().ownVelo;
            Debug.Log(target.GetComponent<BlockBehaviour>().ownVelo);*/
        }

    }
}
