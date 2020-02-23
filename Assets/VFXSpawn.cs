using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSpawn : MonoBehaviour
{
    public float lifetime = 1.0f;

    void Start()
    {
        Destroy(this.gameObject, lifetime);
    }

    void Update()
    {
        
    }
}
