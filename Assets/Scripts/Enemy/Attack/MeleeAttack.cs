using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeAttack : Enemy // 근접 공격
{
    private Enemy enemy;
    private int damage;

    public MeleeAttack(Enemy enemy, int damage)
    {
        this.enemy = enemy;
        this.damage = damage;
    }

    public void ExecuteAttack()
    {
        enemy.SetAnimationTrigger("Attack");

        Collider[] hitColliders = Physics.OverlapSphere(enemy.transform.position, enemy.attackRange);
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                //collider.GetComponent<Player>().TakeDamage(damage);
            }
        }
    }
}