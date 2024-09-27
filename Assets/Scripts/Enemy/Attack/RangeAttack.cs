using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttack : Enemy // ���Ÿ� ����
{
    public Enemy enemy;
    private GameObject projectilePrefab;
    public Transform firePoint;

    public RangeAttack(Enemy enemy, GameObject projectilePrefab, Transform firePoint)
    {
        this.enemy = enemy;
        this.projectilePrefab = projectilePrefab;
        this.firePoint = firePoint;
    }
    public void ExecuteAttack()
    {
        enemy.SetAnimationTrigger("Attack");

        GameObject projectile = Object.Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
    }
}