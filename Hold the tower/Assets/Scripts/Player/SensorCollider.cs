using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SensorCollider : MonoBehaviour
{
    public UnityEvent collide;
    public UnityEvent Uncollide;

    public bool followTheCamera = false;
    public Transform selfCamera;
    public Transform selfCollision;

    public void Update()
    {
        if (followTheCamera)
            selfCollision.localRotation = Quaternion.Euler(new Vector3(0, selfCamera.rotation.eulerAngles.y,0));
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Wall")
        {
            collide.Invoke();
            //Debug.Log(transform.name);
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Wall")
        {
            Uncollide.Invoke();
            //Debug.Log(transform.name);
        }
            
    }
}
