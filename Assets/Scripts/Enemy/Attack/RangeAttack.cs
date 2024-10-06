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

            if (player != null && projectilePrefab != null && firePoint != null)
            {
                SetAnimationState(AnimationState.Attack);
            }
        }
        else
        {
            SetAnimationState(AnimationState.Idle);
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