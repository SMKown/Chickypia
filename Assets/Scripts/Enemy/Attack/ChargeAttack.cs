using System.Collections;
using UnityEngine;

public class ChargeAttack : Enemy
{
    [Header("공격 속성")]
    public int damage = 1;
    public float chargeSpeed = 5f;
    public float attackStartRange = 5f;

    private float lastAttackTime;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Attack()
    {
        if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackStartRange)
            {
                isAttacking = true;
                lastAttackTime = Time.time;
                ExecuteAttack();
            }
        }
    }

    public void ExecuteAttack()
    {
        SetAnimationState(AnimationState.Attack);

        Vector3 targetPosition = player.position;
        Vector3 initialPosition = transform.position;

        Vector3 directionToPlayer = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = lookRotation;

        if (Vector3.Distance(transform.position, targetPosition) <= 0.5f)
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

        agent.isStopped = false;
        isAttacking = false;

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