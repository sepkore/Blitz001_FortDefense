using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave
{
    //TODO: first create array of enemy IDs: "enemy01, enemy01, enemy02, boss01"
    //TODO: create parallel array for delays
    //TODO: describe wave in terms of sequence of enemies and their timing

    public List<string> enemy_ids = new List<string>(); 
    public List<float> delays = new List<float>();

    public int enemy_count = 0;

    public int turn = 0; //turn order of spawning in wave
    public float timer = 0.0f; //incremental timer for delays

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public bool hasEnemiesLeft()
    {
        return (turn < enemy_count);
    }

    public string getNextEnemy()
    {
        timer = 0.0f;
        return enemy_ids[turn++];
    }

    public void addEnemy(string enemy_id, float delay)
    {
        this.enemy_ids.Add(enemy_id);
        this.delays.Add(delay);
        this.enemy_count++;
    }
}
