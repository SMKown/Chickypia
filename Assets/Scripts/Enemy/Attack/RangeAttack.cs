using System.Collections;
using UnityEngine;

public class RangeAttack : Enemy // 원거리 공격
{
    [Header("공격 속성")]
    public float projectileSpeed = 3f;
    public int damage = 1;
    public float attackCooldown = 3f;
    [Header("투사체")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    private float lastAttackTime;


    protected override void Awake()
    {
        base.Awake();
    }

    public override void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            SetAnimationState(AnimationState.Idle);
            return;
        }

        if (projectilePrefab == null || firePoint == null)
        {
            return;
        }
        lastAttackTime = Time.time;
        SetAnimationState(AnimationState.Attack);
    }

    public void ExecuteAttack() // 애니메이션 이벤트 함수 사용해서 공격 넣기
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.SetTarget(player.position);
            projectileScript.SetDamage(damage);
            projectileScript.SetSpeed(projectileSpeed);
        }
    }
}