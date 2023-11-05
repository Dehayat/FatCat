using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.AI;
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
    public float rotationSlerpVariance = 0.1f;
    public float minFollowPath = 0.2f;

    private Cat player;
    private Rigidbody2D playerRb;
    private NavMeshAgent agent;
    private Rigidbody2D rb;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();
        rotationSlerp += UnityEngine.Random.Range(-rotationSlerpVariance / 2, rotationSlerpVariance / 2);
    }

    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        player = FindAnyObjectByType<Cat>();
        playerRb = player.GetComponent<Rigidbody2D>();
        StartCoroutine(DoBehaviour());
    }

    IEnumerator DoBehaviour()
    {

        yield return null;
        while (true)
        {
            if (CloseEnoughToAttack())
            {
                agent.isStopped = true;
                if (IsFacing(player.transform.position))
                {
                    yield return Attack();
                }
                else
                {
                    yield return Face(player.transform.position);
                }
            }
            else
            {
                yield return Follow();
            }
            yield return null;
        }
    }


    IEnumerator Face(Vector3 pos)
    {
        var moveDir = pos - transform.position;
        var targetRot = Vector3.SignedAngle(Vector3.right, moveDir, Vector3.forward);
        root.rotation = Quaternion.Slerp(root.rotation, Quaternion.Euler(0, 0, targetRot), rotationSlerp);
        yield break;
    }

    private bool IsFacing(Vector3 pos, float minDot = 0.9f)
    {
        var lookDir = pos - transform.position;
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
        agent.SetDestination(player.transform.position);
        agent.isStopped = false;
        float oldSpeed = agent.speed;
        agent.speed = 0;
        yield return null;
        while (!IsFacing(agent.steeringTarget, 0.7f))
        {
            yield return Face(agent.steeringTarget);
        }
        agent.speed = oldSpeed;
        float t = 0;
        while (t < minFollowPath && !agent.isStopped)
        {
            yield return Face(agent.steeringTarget);
            t += Time.deltaTime;
        }

        //var moveDir = player.transform.position - transform.position;
        //moveDir.Normalize();
        //rb.velocity = moveDir * followSpeed;
        //yield return Face(player.transform.position);
        //yield break;
    }
}
