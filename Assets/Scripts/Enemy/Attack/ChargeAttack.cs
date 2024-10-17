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
            LookAtPlayer();
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
        if (other.gameObject.name == "TakeDamageRange")
        {
            PlayerHP playerhp = other.GetComponent<PlayerHP>();
            if (playerhp != null)
            {
                playerhp.TakeDamage(damage);
                Debug.Log("Player hit, dealing damage: " + damage);
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

        float elapsedTime = 0f;
        while (elapsedTime < attackCooldown)
        {
            LookAtPlayer();
            elapsedTime += Time.deltaTime;
            yield return null;
        }

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