using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItem : MonoBehaviour
{
    public string upgrade_id;
    public int cost;
    public string desc;
    public int rank_max;
    public int rank;

    void Start()
    {
        //Set text elements
        this.transform.Find("Button").transform.Find("TextCost").GetComponent<Text>().text = this.cost.ToString();
        this.transform.Find("Button").transform.Find("TextDesc").GetComponent<Text>().text = this.desc;
    }

    void Update()
    {
        
    }


}
