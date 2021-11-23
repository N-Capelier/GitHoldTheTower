using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SensorFlag : MonoBehaviour
{
    public UnityEvent collide;

    public void OnTriggerEnter(Collider other)
    {
        if(other.name == "Flag")
        {
            collide.Invoke();
            Destroy(other.gameObject);
        }
    }

}
