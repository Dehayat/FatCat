using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    public float followSpeed;
    public int damage;
    public float maxAttackDistance;
    public float waitBeforeAttack;
    public float waitAfterAttack;
    public Collider2D hitBox;
    public Transform root;
    public Transform attackPoint;
    public float rotationSlerp = 0.3f;

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
                if (IsFacing())
                {
                    yield return Attack();
                }
                else
                {
                    yield return Face();
                }
            }
            else
            {
                if (IsFacing(0.5f))
                {
                    yield return Follow();
                }
                else
                {
                    yield return Face();
                }
            }
            yield return null;
        }
    }


    IEnumerator Face()
    {
        var moveDir = player.transform.position - transform.position;
        var targetRot = Vector3.SignedAngle(Vector3.right, moveDir, Vector3.forward);
        root.rotation = Quaternion.Slerp(root.rotation, Quaternion.Euler(0, 0, targetRot), rotationSlerp);
        yield break;
    }

    private bool IsFacing(float minDot = 0.9f)
    {
        var lookDir = player.transform.position - transform.position;
        lookDir.Normalize();
        return Vector3.Dot(root.right, lookDir) > minDot;
    }

    private Vector3 closestPlayerPos;
    private bool CloseEnoughToAttack()
    {
        var closestPlayerPos = player.hurtBox.ClosestPoint(attackPoint.position);
        this.closestPlayerPos = closestPlayerPos;
        return Vector3.Distance(attackPoint.position, closestPlayerPos) < maxAttackDistance;
    }
    void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(closestPlayerPos, 0.2f);

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
        yield return Face();
        yield break;
    }
}
