using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitEnemies : MonoBehaviour
{
    public GameObject enemies;
    public float catSizeSpawn = 1.2f;

    private Cat cat;

    private void Start()
    {
        cat = FindAnyObjectByType<Cat>();
        cat.onSizeChange.AddListener(CheckAndEnable);
    }

    private void CheckAndEnable(float size)
    {
        if (enabled)
        {
            if (size >= catSizeSpawn)
            {
                enabled = false;
                enemies.SetActive(true);
            }
        }
    }

}
