using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortTrigger : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerStay(Collider collider)
    {
        if(collider.gameObject.tag == "Enemy"){
            Debug.Log("Enemy colliding with fort; negate speed..");
            collider.gameObject.GetComponentInParent<Enemy>().speed = 0.0f; 
        }
    }
}
