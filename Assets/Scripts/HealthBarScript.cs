using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Slider slider;
    public Text health;

    public void SetHealth(int health)
    {
        slider.value = health;
        this.health.text = health.ToString() + " MP";
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        this.health.text = health.ToString() + " MP";
    }

    public int GetHealth() { return (int)slider.value; }
}
