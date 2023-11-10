using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public int damage = 3;
    public float maxLifetime;

    private Vector3 velocity;

    public void SetDir(Vector3 dir)
    {
        velocity = dir.normalized * speed;
    }
    private void Start()
    {
        Destroy(gameObject,maxLifetime);
    }

    private void Update()
    {
        transform.Translate(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Cat>();
        if(player != null)
        {
            player.Attack(damage);
            Destroy(gameObject);
        }
    }
}
