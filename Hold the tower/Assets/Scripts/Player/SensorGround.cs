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
            }
            //Debug.Log(transform.name);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            selfMovement.groundSpeed = other.GetComponent<BlockBehaviour>().speedPerframe;
        }

    }

}
