using UnityEngine;
using UnityEngine.AI;

public enum EnemyLevel
{
    Level1,
    Level2,
    Level3
}

public enum PatrolType
{
    XAxisPatrol,
    ZAxisPatrol,
    XZRandomPatrol
}

public abstract class Enemy : MonoBehaviour
{
    public int health;                // ���� ü��
    public float sightRange;          // ���� �þ� ����
    public float attackRange;         // ���� ���� ����
    public float fleeDistance = 20f;  // ���� ����
    public float patrolRange = 15f;   // ���� ����
    public Vector3 initialPosition;

    protected Animator anim;
    protected Transform player;
    protected NavMeshAgent agent;

    public PatrolType patrolType;

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

            if (distanceToPlayer <= attackRange)
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

    public abstract void Attack();

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

    // ����
    public void ChasePlayer(Vector3 targetPosition)
    {
        SetAnimationState("Move", true);
        agent.SetDestination(targetPosition);
    }

    // ����
    public void FleeFromPlayer(Vector3 fleePosition)
    {
        SetAnimationState("Move", true);
        agent.SetDestination(fleePosition);
    }

    private void OnDrawGizmosSelected()
    {
        // �þ� ���� (�Ķ���)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // ���� ���� (������)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // ���� ���� (�ʷϻ�)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fleeDistance);

        // ���� ���� (�����)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(initialPosition, patrolRange);
    }
}