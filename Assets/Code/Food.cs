using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float size;
    public float digestSize;
    public Transform eatPoint;

    public SpriteRenderer sprite;

    public void SetDrawOrder(int order)
    {
        sprite.sortingOrder = order;
    }
}
