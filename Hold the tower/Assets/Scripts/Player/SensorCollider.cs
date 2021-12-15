using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SensorCollider : MonoBehaviour
{
    public UnityEvent collide;
    public UnityEvent Uncollide;

    private BlockBehaviour contactWall;

    private int nbCollide = 0;

    public void Update()
    {
        if(contactWall != null && !contactWall.boxCollider.enabled)
        {
            nbCollide--;
            if (nbCollide == 0)
            {
                Debug.Log("Isdestroyed");
                contactWall = null;
                Uncollide.Invoke();
            }
        }
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Wall"))
        {
            nbCollide++;

            if (nbCollide == 1)
            {
                contactWall = other.GetComponent<BlockBehaviour>();
                collide.Invoke();
            }
            
            //Debug.Log(transform.name);
        }

        if(other.name == "Flag") // A voir
        {
            
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Wall"))
        {
            nbCollide--;
            if (nbCollide == 0)
            {
                contactWall = null;
                Uncollide.Invoke();
            }
            //Debug.Log(transform.name);
        }

        if (other.name == "Flag")
        {
            
        }

    }

}
