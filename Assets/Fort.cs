using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fort : MonoBehaviour
{
    //public static Fort fort;

    public int hp_base = 20;
    public int hp = 20;
    public int hp_max = 20;

    void Awake()
    {
        /*
        if(fort == null){
            DontDestroyOnLoad(gameObject);
            fort = this;
        }
        else if(fort != this){
            Destroy(gameObject);
        }
        */
    }

    void Start()
    {
        //DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        
    }

    /*
    void OnTriggerStay(Collider collider)
    {
        if(collider.gameObject.tag == "Enemy"){
            Debug.Log("Enemy colliding with fort; negate speed..");
            collider.gameObject.GetComponentInParent<Enemy>().speed = 0.0f; 
        }
    }
    */

    /*
    IEnumerator flash()
    {
        GameObject mesh = this.transform.Find("Mesh").gameObject;
        mesh.GetComponent<MeshRenderer>().material.color = Color.red;

        yield return new WaitForSecondsRealtime(0.2f);

        mesh.GetComponent<MeshRenderer>().material.color = Color.white;
    }
    */
}
