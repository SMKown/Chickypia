using System.Collections;
using UnityEngine;

public class RangeAttack : Enemy // 원거리 공격
{
    [Header("공격 속성")]
    public float projectileSpeed = 3f;
    public int damage = 1;
    [Header("투사체")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    private float lastAttackTime;

    protected override void Awake()
    {
        base.Awake();
        lastAttackTime = Time.time;
    }

    public override void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown && !isAttacking)
        {
            isAttacking = true;
            lastAttackTime = Time.time;

            if (player != null && projectilePrefab != null && firePoint != null)
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
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            Projectile projectileScript = projectile.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                projectileScript.SetDamage(damage);
                projectileScript.SetSpeed(projectileSpeed);
            }

            StartCoroutine(WaitForCooldown());
        }
    }

    public void AttackAnimationEnd()
    {
        isAttacking = false;
        StartCoroutine(WaitForCooldown());
    }

    private IEnumerator WaitForCooldown()
    {
        Debug.Log("쿨타임중~~~~~~~~~~~~~~~~~~");
        SetAnimationState(AnimationState.Idle);
        agent.isStopped = true;

        float elapsedTime = 0f;
        while (elapsedTime < attackCooldown)
        {
            if (GetComponent<EnemyAI>().CurrentState is FleeingState)
            {
                Debug.Log("도망 상태로 전환됨 - 쿨타임 중지");
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