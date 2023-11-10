using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : MonoBehaviour
{
    public float followSpeed;
    public float maxAttackDistance;
    public float waitBeforeAttack;
    public float waitAfterAttack;
    public Transform root;
    public Transform attackPoint;
    public float minFollowPath = 0.2f;
    public float attackDistanceDecreaseStep = 0.5f;
    public GameObject attackPrefab;

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
                yield return Follow(player.transform.position);
            }
            yield return null;
        }
    }

    private bool CanAttack()
    {
        return true;
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
        yield return Face(player.transform.position);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(waitBeforeAttack);
        var attackInstance = Instantiate(attackPrefab, attackPoint.transform.position, Quaternion.identity);
        attackInstance.GetComponent<Bullet>().SetDir(player.transform.position - attackPoint.position);
        yield return new WaitForSeconds(waitAfterAttack);
    }
    IEnumerator Follow(Vector3 targetPos)
    {
        agent.stoppingDistance = maxAttackDistance - 0.3f;
        agent.SetDestination(targetPos);
        agent.isStopped = false;
        float t = 0;
        while (t < minFollowPath && agent.isOnNavMesh && !agent.isStopped)
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
