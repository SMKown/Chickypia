using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEAttack : Enemy // 범위 공격
{
    private Enemy enemy;
    private float radius;
    private int damage;

    public AOEAttack(Enemy enemy, float radius, int damage)
    {
        this.enemy = enemy;
        this.radius = radius;
        this.damage = damage;
    }

    public void ExecuteAttack()
    {
        enemy.SetAnimationTrigger("Attack");

        Collider[] hitColliders = Physics.OverlapSphere(enemy.transform.position, radius);
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                //// 플레이어에게 데미지 입힘
                //collider.GetComponent<Player>().TakeDamage(damage);
            }
        }
    }
}
