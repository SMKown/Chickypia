using System.Collections;
using UnityEngine;

public class RangeAttack : Enemy // 원거리 공격
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed;
    public int damage;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Attack()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            return;
        }
        SetAnimationTrigger("Attack");
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