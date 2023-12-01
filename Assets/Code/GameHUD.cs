using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    public Slider slider;
    public Cat cat;

    private int maxHealth;
    private void Start()
    {
        maxHealth = cat.health;
    }
    private void Update()
    {
        slider.value = (float)cat.health / maxHealth;
    }
}
