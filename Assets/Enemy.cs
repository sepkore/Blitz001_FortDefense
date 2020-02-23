using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Rigidbody rb;
    public int hp = 10;
    public int hp_max = 10;
    public float speed = 1.0f;
    public Vector3 trajectory = new Vector3(0.0f, 0.0f, 0.0f);
    public int order = 0; //order of spawning in wave
    public float delay = 0.0f; //delay before spawning when turn
    public bool isAlive = true;
    public string clone_id = "";
    public float attack_range = 2.0f; 
    public float attack_rate = 0.5f;
    public float attack_timer = 100.0f;
    public int attack_damage = 1;
    public int gold = 1;

    Color original_color;

    public int hit_count = 0; //TEMP: DEBUGGING

    void Start()
    {
        original_color = this.transform.Find("Mesh").GetComponent<MeshRenderer>().material.GetColor("_BaseColor");
    }

    void Update()
    {
        if(this.hp <= 0){ this.isAlive = false; }
    }

    void FixedUpdate()
    {
        //Main.main.moveToGround(this.gameObject); //pin to ground to nagigate terrain
    }

    void OnTriggerEnter(Collider collider_other) //OnCollisionEnter wont work if one collider is a trigger - LAME
    {
        //GameObject spr = this.transform.Find("Sprite").gameObject;
        //if(!spr.GetComponent<Collider>().enabled){ return; } //still called/triggered when collider isn't enabled?

        //Debug.Log("Enemy triggered " + this.name);
 
        
        if(collider_other.gameObject.tag == "Projectile"){
            //StartCoroutine(flash()); //sprite flash
            StartCoroutine(flashMesh(this.gameObject));
            Turret source = collider_other.gameObject.GetComponentInParent<Projectile>().source;
            Main.main.onHitEnemy(source, this, collider_other.transform.position);
            collider_other.enabled = false;
            Destroy(collider_other.gameObject); //destroy projectile
        }
        
        
    }

    IEnumerator flash()
    {
        GameObject spr = this.transform.Find("Sprite").gameObject;
        spr.GetComponent<SpriteRenderer>().material.color = Color.red;

        yield return new WaitForSecondsRealtime(0.2f);

        spr.GetComponent<SpriteRenderer>().material.color = Color.white;
    }

    IEnumerator flashMesh(GameObject obj)
    {
        //Debug.Log("Flashing Mesh");
        GameObject mesh = obj.transform.Find("Mesh").gameObject;
        MeshRenderer rnd = mesh.GetComponent<MeshRenderer>();
        
        rnd.material.SetColor("_BaseColor", Color.red);
        yield return new WaitForSecondsRealtime(0.05f);
        rnd.material.SetColor("_BaseColor", this.original_color);
    }
    
}
