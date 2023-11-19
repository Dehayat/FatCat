using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CityItem
{
    public bool possible;
    public GameObject prefab;
    public int weight;
}

public class GenItem : MonoBehaviour
{
    public float genProbability = 1.0f;
    public CityItem[] possibleItmes;


    private void Awake()
    {
        if (Random.Range(0, 1f) > genProbability)
        {
            return;
        }
        int weightTotal = 0;
        foreach (CityItem item in possibleItmes)
        {
            if (item.possible)
            {
                weightTotal += item.weight;
            }
        }
        int random = Random.Range(0, weightTotal);
        int curWeight = 0;
        foreach (CityItem item in possibleItmes)
        {
            if (item.possible)
            {
                curWeight += item.weight;
                if (random < curWeight)
                {
                    Instantiate(item.prefab, transform.position, Quaternion.identity);
                    return;
                }
            }
        }
    }
}
