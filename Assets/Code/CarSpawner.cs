using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public float spawnInterval = 3;
    [Range(0f, 1f)]
    public float spawnProbability = 0.5f;
    public GameObject carPrefab;

    private void Start()
    {
        StartCoroutine(SpawnCars());
    }

    IEnumerator SpawnCars()
    {
        while(true)
        {
            if (Random.Range(0, 1f) < spawnProbability)
            {
                Instantiate(carPrefab, transform.position, transform.rotation);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
