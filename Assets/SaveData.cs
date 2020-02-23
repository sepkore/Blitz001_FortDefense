using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //Game States

    public void addGameData()
    {
        
    }


    //Upgrades & Abilities



    //Player
    private int damage_base = 5;
    private float attack_rate_base = 0.5f;
    private int gold = 0;
    public void addPlayerData(Turret player)
    {
        this.damage_base = player.damage_base;
        this.attack_rate_base = player.attack_rate_base;
        this.gold = player.gold;
    }


    //Fort    
    public int hp_base = 20;
    public void addFortData(Fort fort)
    {
        this.hp_base = fort.hp_base;
    }


    //Enemies

    public void addEnemyData()
    {
        
    }
}
