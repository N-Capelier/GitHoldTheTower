using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateformeMovement : MonoBehaviour
{

    public Vector3 speedPerframe;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = transform.position;
        transform.position += new Vector3(0, Mathf.Cos(Time.time), 0) * Time.fixedDeltaTime * 0.5f;
        speedPerframe = transform.position - temp;
    }
}
