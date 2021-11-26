using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SensorCollider : MonoBehaviour
{
    public UnityEvent collide;
    public UnityEvent Uncollide;


    private int nbCollide = 0;

    public void Update()
    {
 
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Wall"))
        {
            nbCollide++;
            if(nbCollide == 1)
            {
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
            if (nbCollide == 0) {
                Uncollide.Invoke();
            }
            //Debug.Log(transform.name);
        }

        if (other.name == "Flag")
        {
            
        }

    }

}
