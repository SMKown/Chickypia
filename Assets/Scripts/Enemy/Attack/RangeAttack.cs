using System.Collections;
using UnityEngine;

public class RangeAttack : Enemy // ���Ÿ� ����
{
    [Header("���� �Ӽ�")]
    public float projectileSpeed = 3f;
    public int damage = 1;
    [Header("����ü")]
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
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
                transform.rotation = lookRotation;
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