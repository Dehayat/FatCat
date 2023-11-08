using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SideEnemy : MonoBehaviour
{
    public float followSpeed;
    public int damage;
    public float maxAttackDistance;
    public float sideStopDistance;
    public float waitBeforeAttack;
    public float waitAfterAttack;
    public Collider2D hitBox;
    public Transform root;
    public Transform attackPoint;
    public float minFollowPath = 0.2f;
    public float attackDistanceDecreaseStep = 0.5f;
    public Vector3 sideOffset;
    public float sideDotMin = 0.85f;

    private Cat player;
    private NavMeshAgent agent;
    private Rigidbody2D rb;
    private float savedAttackDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();
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
                yield return Face(player.transform.position);
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
                var sidePos = GetClosestSideFromPlayer();
                if (!OnSideOfPlayer())
                {
                    yield return GoToSide(sidePos);
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
        if (!OnSideOfPlayer())
        {
            return false;
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
        var scale = transform.localScale;
        if (pos.x > transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x);

        }
        transform.localScale = scale;
        yield break;
    }


    private Vector3 closestPlayerPos;

    private bool WithinAttackDistanceOfPlayer()
    {
        return WithinAttackDistance(player.hurtBox.ClosestPoint(attackPoint.position));
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
        agent.speed = oldSpeed;
        float t = 0;
        while (t < minFollowPath && !agent.isStopped)
        {
            yield return Face(agent.steeringTarget);
            t += Time.deltaTime;
        }
    }
    IEnumerator GoToSide(Vector3 targetPos)
    {
        agent.stoppingDistance = sideStopDistance;
        agent.SetDestination(targetPos);
        agent.isStopped = false;
        float oldSpeed = agent.speed;
        agent.speed = oldSpeed;
        float t = 0;
        while (t < minFollowPath && !agent.isStopped)
        {
            yield return Face(agent.steeringTarget);
            t += Time.deltaTime;
        }
        agent.stoppingDistance = maxAttackDistance;
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
