using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityTrigger : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider_other)
    {
        //Debug.Log("Player Proximity entered by: " + collider_other.gameObject.name);
        this.gameObject.GetComponentInParent<Turret>().addProximityEnemy(collider_other.transform.parent.gameObject);
    }

    void OnTriggerStay(Collider collider_other) //OnCollisionEnter wont work if one collider is a trigger - LAME
    {
        //Debug.Log("Player Proximity Attack against " + collider_other.gameObject.name);
        //GameObject spr = this.transform.Find("ProximityTrigger").gameObject;
        //if(!spr.GetComponent<Collider>().enabled){ return; } //still called/triggered when collider isn't enabled?

        //Main.main.basicAttack(collider_other.transform.position);
        //Main.main.onHitEnemy(this.clone_id, collider_other.transform.position);
        //collider_other.enabled = false;
        //Destroy(collider_other.gameObject); //destroy projectile
        
    }
    
}
