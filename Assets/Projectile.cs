using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 0.0f;
    public float speed = 30.0f;

    //TODO: add values from turret or add reference to firing turret
    public Turret source;

    void Start()
    {
        if(lifetime > 0){ Destroy(this.gameObject, lifetime); }
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {

    }
}
