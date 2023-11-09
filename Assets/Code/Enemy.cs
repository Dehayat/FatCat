using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
    public float attackDistanceDecreaseStep = 0.5f;
    public bool attackFromSides = false;
    public Vector3 sideOffset;
    public float sideDotMin = 0.7f;

    private Cat player;
    private NavMeshAgent agent;
    private Rigidbody2D rb;
    private float savedAttackDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();
        rotationSlerp += UnityEngine.Random.Range(-rotationSlerpVariance / 2, rotationSlerpVariance / 2);
        savedAttackDistance = maxAttackDistance;
    }

    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        player = FindAnyObjectByType<Cat>();
        StartCoroutine(DoBehaviour());
    }

    IEnumerator DoBehaviour()
    {

        yield return null;
        while (true)
        {
            if (WithinAttackDistanceOfPlayer() && CanAttack())
            {
                agent.isStopped = true;
                if (IsFacing(player.transform.position))
                {
                    if (CanSeePlayer())
                    {
                        ResetAttackDistance();
                        yield return Attack();
                    }
                    else
                    {
                        DecreaseAttackDistance();
                    }
                }
                else
                {
                    yield return Face(player.transform.position);
                }
            }
            else
            {
                if (attackFromSides)
                {
                    var sidePos = GetClosestSideFromPlayer();
                    if (!OnSideOfPlayer())
                    {
                        yield return Follow(sidePos);
                    }
                    else
                    {
                        yield return Follow(player.transform.position);
                    }
                }
                else
                {
                    yield return Follow(player.transform.position);
                }
            }
            yield return null;
        }
    }

    private bool CanAttack()
    {
        if (attackFromSides)
        {
            if (!OnSideOfPlayer())
            {
                return false;
            }
        }
        return true;
    }

    private bool OnSideOfPlayer()
    {
        var toPlayer = player.transform.position - transform.position;
        var dot = Vector3.Dot(toPlayer.normalized, Vector3.right);
        return Mathf.Abs(dot) > sideDotMin;
    }

    private void ResetAttackDistance()
    {
        maxAttackDistance = savedAttackDistance;
    }

    private bool CanSeePlayer()
    {
        return !agent.Raycast(player.transform.position, out _);
    }

    private void DecreaseAttackDistance()
    {
        maxAttackDistance -= attackDistanceDecreaseStep;
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

    private bool WithinAttackDistanceOfPlayer()
    {
        return WithinAttackDistance(player.HurtBox.ClosestPoint(attackPoint.position));
    }
    private bool WithinAttackDistance(Vector3 position)
    {
        closestPlayerPos = position;
        return Vector3.Distance(attackPoint.position, position) < maxAttackDistance;
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
    IEnumerator Follow(Vector3 targetPos)
    {
        agent.stoppingDistance = maxAttackDistance;
        agent.SetDestination(targetPos);
        agent.isStopped = false;
        float oldSpeed = agent.speed;
        while (!IsFacing(agent.steeringTarget, 0.7f))
        {
            agent.speed = 0;
            //yield return null;
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

    private Vector3 GetClosestSideFromPlayer()
    {
        var side1 = player.transform.position + sideOffset;
        var side2 = player.transform.position - sideOffset;
        if (Vector3.Distance(transform.position, side1) < Vector3.Distance(transform.position, side2))
        {
            return side1;
        }
        else
        {
            return side2;
        }
    }
}
