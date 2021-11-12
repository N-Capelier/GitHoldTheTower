using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class CursedMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            NetworkTransformChild comp = gameObject.AddComponent<NetworkTransformChild>();
            comp.target = transform.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
