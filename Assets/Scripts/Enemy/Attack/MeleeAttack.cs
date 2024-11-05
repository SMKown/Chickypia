using System.Collections;
using UnityEngine;

public class MeleeAttack : Enemy // 근접 공격
{
    [Header("공격 속성")]
    public int damage = 1;
    private float lastAttackTime;

    protected override void Awake()
    {
        base.Awake(); 
    }

    public override void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown && !isAttacking)
        {
            isAttacking = true;
            lastAttackTime = Time.time;
            
            if (player != null)
            {
                SetAnimationState(AnimationState.Attack);
            }
        }
        else
        {
            LookAtPlayer();
        }
    }

    public void ExecuteAttack()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (angleToPlayer <= 60f && distanceToPlayer <= attackRange)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
            foreach (var collider in hitColliders)
            {
                if (collider.transform.parent != null && collider.transform.parent.CompareTag("Player"))
                {
                    PlayerHP playerHP = collider.GetComponent<PlayerHP>();
                    if (playerHP != null)
                    {
                        playerHP.TakeDamage(damage);
                    }
                }
            }
        }
        isAttacking = false;
    }


    public void AttackAnimationEnd()
    {
        isAttacking = false;
        StartCoroutine(WaitForCooldown());
    }

    private IEnumerator WaitForCooldown()
    {
        isAttacking = false;
        SetAnimationState(AnimationState.Idle);
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        float elapsedTime = 0f;
        while (elapsedTime < attackCooldown)
        {
            if (GetComponent<EnemyAI>().CurrentState is FleeingState)
            {
                isAttacking = false;
                agent.isStopped = false;
                yield break;
            }

            LookAtPlayer();
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
        agent.isStopped = false;

        if (!(GetComponent<EnemyAI>().CurrentState is FleeingState))
        {
            if (PlayerInAttackRange())
            {
                Attack();
            }
            else
            {
                ChasePlayer(player.position);
            }
        }
    }
}