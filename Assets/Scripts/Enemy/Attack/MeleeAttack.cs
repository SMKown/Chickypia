using System.Collections;
using UnityEngine;

public class MeleeAttack : Enemy // 근접 공격
{
    [Header("공격 속성")]
    public int damage = 1;

    private float lastAttackTime;
    private bool isAttacking = false;

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
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
                transform.rotation = lookRotation;

                SetAnimationState(AnimationState.Attack);
            }
        }
        else
        {
            if (player != null)
            {
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
    }

    public void ExecuteAttack()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer <= 60f)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
            foreach (var collider in hitColliders)
            {
                if (collider.CompareTag("Player"))
                {
                    PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();
                    if (playerMovement != null)
                    {
                        playerMovement.TakeDamage(damage);
                    }
                }
            }
        }
    }

    public void AttackAnimationEnd()
    {
        StartCoroutine(WaitForCooldown());
    }

    private IEnumerator WaitForCooldown()
    {
        SetAnimationState(AnimationState.Idle);
        agent.isStopped = true;
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        agent.isStopped = false;

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