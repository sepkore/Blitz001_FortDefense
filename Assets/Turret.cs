using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public int damage = 5;
    public int damage_base = 5;
    public float attack_rate_base = 0.5f;
    public float attack_rate = 0.5f;
    public float attack_timer = 0.0f;
    public int gold = 0;

    public List<GameObject> enemies_prox = new List<GameObject>(); //list of enemies in proximity (not necessarily targettable); added by ProximityTrigger
    public List<GameObject> targets = new List<GameObject>(); //list of enemies that can currently be targeted
    public GameObject target;
    
    //TODO: add id or object for basic attack (to determine animations, projectile, etc)


    void Awake()
    {
    
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void clear()
    {
        this.enemies_prox.Clear();
        this.targets.Clear();
        this.target = null;
    }

    public void addProximityEnemy(GameObject obj)
    {
        this.enemies_prox.Add(obj);
    }


    public void removeProximityEnemy(GameObject obj) //TODO: Create Dict/Map for faster lookups
    {
        Enemy nme = obj.GetComponent<Enemy>();
        for(int i=this.enemies_prox.Count-1; i>=0; i--){        
            Enemy nme_prox = this.enemies_prox[i].GetComponent<Enemy>();
            if(nme.clone_id == nme_prox.clone_id){
                this.enemies_prox.Remove(nme_prox.gameObject);
            }
        }
    }

    public void scanForTargets()
    {
        //Check living enemies in proximity for line of sight to determine potential targets
        this.targets.Clear();
        foreach(GameObject nme in this.enemies_prox){
            if(!nme.GetComponent<Enemy>().isAlive){ continue; }
            if(hasLineOfSight(nme)){
                this.targets.Add(nme);
                //Debug.Log("Scanned for and added Target");
            }
        }
    }

    public bool hasLineOfSight(GameObject obj)
    {
        bool canSee = false;

        //Raycast from this to obj; if raycast hits obj, canSee = true
        float distance = this.transform.Find("ProxTrigger").GetComponent<BoxCollider>().size.x;
        //Debug.Log("LOS Distance: " + distance);
        Vector3 direction = obj.transform.position - this.transform.position;
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position, direction, out hit, distance)){
            if(hit.transform.gameObject.CompareTag("Enemy")){
                canSee = true;
                //Debug.Log("..I can see you " + obj.name);
            }
        }

        return canSee;
    }

    public GameObject getClosestTarget()
    {
        //Iterate this.targets to check for closest position
        if(this.targets.Count > 0){
            int closest_index = 0;
            float dist_min = 500.0f;

            for(int i=0; i<this.targets.Count; i++){
                float dist = Vector3.Distance(this.transform.position, this.targets[i].transform.position);
                if(dist < dist_min){
                    closest_index = i;
                    dist_min = dist;
                }
            }

            return this.targets[closest_index];
        }
        else{
            return null;
        }
    }

    public void selectClosestTarget()
    {
        this.target = getClosestTarget();
    }
    
}
