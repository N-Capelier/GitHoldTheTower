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

    private BoxCollider bcollider;
    private Collider[] walls;
    private bool isOnWall;

    private void Start()
    {
        bcollider = GetComponent<BoxCollider>();
    }

    public void Update()
    {
        /*
        if(contactWall != null && !contactWall.boxCollider.enabled)
        {
            nbCollide--;
            if (nbCollide == 0)
            {
                Debug.Log("Isdestroyed");
                contactWall = null;
                Uncollide.Invoke();
            }
        }*/

        walls = Physics.OverlapBox(bcollider.bounds.center, bcollider.bounds.extents, transform.rotation, LayerMask.GetMask("Outlined"));
        if (walls.Length > 0)
        {
            if (!isOnWall)
            {
                collide.Invoke();
                isOnWall = true;
            }
        }
        else
        {
            if (isOnWall)
            {
                Uncollide.Invoke();
                isOnWall = false;
            }
        }
    }

    /*
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
    */
}
