using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public int maxEnemyCount = 5;
    public float enemySpawnInterval = 3;
    public float catSizeSpawn = 1.2f;

    private Cat cat;

    List<GameObject> enemies = new List<GameObject>();

    private void Start()
    {
        cat = FindAnyObjectByType<Cat>();
        cat.onSizeChange.AddListener(StartSpawning);
    }

    private void StartSpawning(float size)
    {
        if (size >= catSizeSpawn)
        {
            cat.onSizeChange.RemoveListener(StartSpawning);
            StartCoroutine(SpawnSequence());
        }
    }
    private IEnumerator SpawnSequence()
    {
        while (true)
        {
            UpdateList();
            if (enemies.Count < maxEnemyCount)
            {
                enemies.Add(Instantiate(enemy, transform.position, Quaternion.identity));
            }
            yield return new WaitForSeconds(enemySpawnInterval);
        }

    }

    private void UpdateList()
    {
        enemies.RemoveAll(x => x == null);
    }
}
