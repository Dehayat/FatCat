using System;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float lifeTime = 10f;
    public float speed;

    internal void Stop()
    {
        speed = 0;
    }

    private void Start()
    {
        Destroy(gameObject,lifeTime);
    }

    private void Update()
    {
        transform.Translate(transform.right * speed * Time.deltaTime,Space.World);
    }
}
