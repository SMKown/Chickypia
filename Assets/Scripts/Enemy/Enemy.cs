using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { Runtype, FightType }
public enum AnimationState { Idle, Move, Attack, Damage, Die }
public abstract class Enemy : MonoBehaviour
{
    public int health;
    [Header("시야 및 공격 범위")]
    [Space(3)]public float sightRange;
    public float attackRange;
    [Header("공격 쿨타임")]
    public float attackCooldown;

    protected Animator anim;
    protected Transform player;

    [HideInInspector]public NavMeshAgent agent;
    [Header("순찰 포인트")]
    public Vector3[] patrolPoints;

    protected virtual void Awake()
    {
        player = GameObject.Find("Player_AttackMode").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public virtual void TakeDamage(int damage, Vector3 playerForwardDirection, float knockbackForce)
    {
        if (health <= 0) return;

        health -= damage;
        Knockback(playerForwardDirection, knockbackForce);
        StartCoroutine(FlashRed());
        SetAnimationState(AnimationState.Damage);

        if (health > 0)
        {
            EnemyAI enemyAI = GetComponent<EnemyAI>();
            if (enemyAI.enemyType == EnemyType.Runtype && !enemyAI.hasFledOnce)
            {
                enemyAI.SwitchState(new FleeingState(enemyAI));
            }
        }

        if (health <= 0) Die();
    }

    private void Knockback(Vector3 playerForwardDirection, float force)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 knockbackDirection = playerForwardDirection.normalized;
            agent.isStopped = true;
            rb.AddForce(knockbackDirection * force, ForceMode.Impulse);
            StartCoroutine(ReEnableNavMeshAgent());
        }
    }

    private IEnumerator ReEnableNavMeshAgent()
    {
        yield return new WaitForSeconds(0.5f);

        agent.enabled = true;
        agent.isStopped = false;
    }


    private IEnumerator FlashRed()
    {
        Renderer renderer = GetComponentInChildren<Renderer>();

        Color originalColor = renderer.material.color;

        renderer.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);
        renderer.material.color = originalColor;
    }

    public void OnDamageAnimationEnd()
    {
        if (health > 0)
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);

            if (PlayerInAttackRange())
            {
                SetAnimationState(AnimationState.Move);
                SetAnimationState(AnimationState.Attack);
                Attack();
            }
            else if (distanceToPlayer <= sightRange)
            {
                SetAnimationState(AnimationState.Move);
                ChasePlayer(player.position);
            }
            else
            {
                SetAnimationState(AnimationState.Move);
                SetAnimationState(AnimationState.Idle);
            }
        }
    }

    protected void Die()
    {
        SetAnimationState(AnimationState.Die);
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
        Destroy(gameObject, 2f);
    }

    public virtual void Attack(){}

    public void SetAnimationState(AnimationState state)
    {
        if (anim != null)
        {
            anim.SetBool("Idle", state == AnimationState.Idle);
            anim.SetBool("Move", state == AnimationState.Move);

            if (state == AnimationState.Attack)
            {
                anim.SetTrigger("Attack");
            }
            if (state == AnimationState.Damage)
            {
                anim.SetTrigger("Damage");
            }
            if (state == AnimationState.Die)
            {
                anim.SetTrigger("Die");
            }
        }
    }


    public void ResetAnimationState()
    {
        if (anim != null)
        {
            anim.SetBool("Move", false);
            anim.SetBool("Idle", false);
            anim.ResetTrigger("Attack");
            anim.ResetTrigger("Damage");
            anim.ResetTrigger("Die");
        }
    }

    public void ChasePlayer(Vector3 targetPosition)
    {
        if (agent != null)
        {
            agent.SetDestination(targetPosition);
            SetAnimationState(AnimationState.Move);
        }
    }

    public void FleeFromPlayer(Vector3 fleePosition)
    {
        SetAnimationState(AnimationState.Move);
        agent.SetDestination(fleePosition);
    }

    public bool PlayerInAttackRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var collider in colliders)
        {
            if (collider.transform.IsChildOf(player))
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        // 시야 범위 (파란색)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // 공격 범위 (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 순찰 포인트 (초록색)
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Gizmos.DrawSphere(patrolPoints[i], 0.5f);

                if (i + 1 < patrolPoints.Length)
                {
                    Gizmos.DrawLine(patrolPoints[i], patrolPoints[i + 1]);
                }
                else if (i == patrolPoints.Length - 1)
                {
                    Gizmos.DrawLine(patrolPoints[i], patrolPoints[0]);
                }
            }
        }
    }
}