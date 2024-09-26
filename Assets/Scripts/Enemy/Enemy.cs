using UnityEngine;
using UnityEngine.AI;

public enum EnemyLevel
{
    Level1,
    Level2,
    Level3
}

public abstract class Enemy : MonoBehaviour
{
    public int health;
    public float sightRange;
    public float attackRange;
    public Vector3 initialPosition;
    public IEnemyAttack attackPattern;

    protected Animator anim;
    protected Transform player;
    public NavMeshAgent agent;

    public Vector3[] patrolPoints;

    protected virtual void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        initialPosition = transform.position;
    }

    public virtual void TakeDamage(int damage)
    {
        if (health <= 0) return;

        health -= damage;
        SetAnimationTrigger("Damage");

        if (health <= 0) Die();
    }

    public void OnDamageAnimationEnd()
    {
        if (health > 0)
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);

            if (PlayerInAttackRange())
            {
                SetAnimationState("Move", false);
                SetAnimationTrigger("Attack");
                Attack();
            }
            else if (distanceToPlayer <= sightRange)
            {
                SetAnimationState("Move", true);
                ChasePlayer(player.position);
            }
            else
            {
                SetAnimationState("Move", false);
                SetAnimationState("Idle", true);
            }
        }
    }

    protected void Die()
    {
        SetAnimationTrigger("Die");
        agent.enabled = false;
        Destroy(gameObject, 2f);
    }

    public virtual void Attack()
    {
        attackPattern?.ExecuteAttack();
    }

    public void SetAnimationTrigger(string triggerName)
    {
        if (anim != null)
        {
            anim.SetTrigger(triggerName);
        }
    }

    public void SetAnimationState(string stateName, bool state)
    {
        if (anim != null)
        {
            anim.SetBool(stateName, state);
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
        SetAnimationState("Move", true);
        agent.SetDestination(targetPosition);
    }

    public void FleeFromPlayer(Vector3 fleePosition)
    {
        SetAnimationState("Move", true);
        agent.SetDestination(fleePosition);
    }

    public bool PlayerInAttackRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var collider in colliders)
        {
            CharacterController characterController = collider.GetComponent<CharacterController>();
            if (characterController != null && collider.transform == player)
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