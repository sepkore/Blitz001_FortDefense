using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Slider slider;
    public Text text_display;

    public float value_current = 0;
    public float value_max = 0;

    private float currentValue = 0.0f;
    public float CurrentValue{
        get{
            return currentValue;
        }
        set{
            currentValue = value;
            slider.value = currentValue;
            //text_display.text = (100 * slider.value).ToString("0") + "%";
        }
    }

    void Start()
    {
        
    }

    void Update()
    {    
        float percent = value_current / value_max; //current hp / max hp
        CurrentValue = Mathf.Lerp(0, 1, percent);
        
    }
}
