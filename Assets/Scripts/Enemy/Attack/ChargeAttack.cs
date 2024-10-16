using System.Collections;
using TMPro;
using UnityEngine;

public class ChargeAttack : Enemy
{
    [Header("공격 속성")]
    public int damage = 1;
    public float chargeSpeed = 2f;
    public float chargeDuration = 2f;

    private float lastAttackTime;

    protected override void Awake()
    {
        base.Awake();
        agent.updateRotation = false;
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
        if (PlayerInAttackRange())
        {
            Vector3 targetPosition = player.position;
            StartCoroutine(ChargeTowardsTarget(targetPosition));
        }
    }

    private IEnumerator ChargeTowardsTarget(Vector3 targetPosition)
    {
        agent.isStopped = false;
        agent.SetDestination(targetPosition);
        float chargeStartTime = Time.time;

        while (Time.time - chargeStartTime < chargeDuration && !agent.pathPending)
        {
            if (Vector3.Distance(transform.position, targetPosition) <= 0.5f)
            {
                break;
            }

            yield return null;
        }
        AttackAnimationEnd();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isAttacking)
        {
            PlayerHP playerHP = other.GetComponent<PlayerHP>();
            if (playerHP != null)
            {
                playerHP.TakeDamage(damage);
                Debug.Log("Player hit by charge in trigger!");
            }
            AttackAnimationEnd();
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