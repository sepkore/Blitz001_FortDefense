using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider_other) //OnCollisionEnter wont work if one collider is a trigger - LAME
    {
        Main.main.onHitGround(collider_other.transform.position);
        collider_other.enabled = false;
        Destroy(collider_other.gameObject); //destroy projectile
    }
}
