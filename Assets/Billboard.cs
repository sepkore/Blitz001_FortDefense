using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        //transform.LookAt(Camera.main.transform.position, -Vector3.up); //+up
        float x = Camera.main.transform.forward.x;
        float y = 0; //negate y to prevent pitch up and down //Camera.main.transform.forward.y;
        float z = Camera.main.transform.forward.z;
        Vector3 alignment = new Vector3(x, y, z);
        this.transform.forward = alignment;
    }
}
