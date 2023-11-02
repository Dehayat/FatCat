using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float followSpeed;
    public int damage;
    public float maxAttackDistance;
    public float waitBeforeAttack;
    public float waitAfterAttack;
    public Collider2D hitBox;

    private Cat player;
    private Rigidbody2D playerRb;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        player = FindAnyObjectByType<Cat>();
        playerRb = player.GetComponent<Rigidbody2D>();
        StartCoroutine(DoBehaviour());
    }

    IEnumerator DoBehaviour()
    {
        while (true)
        {
            if (CloseEnoughToAttack())
            {
                yield return Attack();
            }
            else
            {
                yield return Follow();
            }
            yield return null;
        }
    }
    private Vector3 closestPlayerPos;
    private Vector3 closestMyPos;
    private bool CloseEnoughToAttack()
    {
        var closestPlayerPos = player.hurtBox.ClosestPoint(transform.position);
        var closestMyPos = hitBox.ClosestPoint(closestPlayerPos);
        this.closestMyPos = closestMyPos;
        this.closestPlayerPos = closestPlayerPos;
        return Vector3.Distance(closestMyPos, closestPlayerPos) < maxAttackDistance;
    }
    void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(closestPlayerPos, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(closestMyPos, 0.2f);

    }
    IEnumerator Attack()
    {
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(waitBeforeAttack);
        player.Attack(damage);
        yield return new WaitForSeconds(waitAfterAttack);
    }
    IEnumerator Follow()
    {
        var moveDir = player.transform.position - transform.position;
        moveDir.Normalize();
        rb.velocity = moveDir * followSpeed;
        var scale = transform.localScale;
        if (moveDir.x < -0.1)
        {
            scale.x = -Mathf.Abs(scale.x);
        }
        else if (moveDir.x > 0.1)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        transform.localScale = scale;
        yield return null;
    }
}
