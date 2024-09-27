using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEAttack : Enemy // 범위 공격
{
    private Enemy enemy;
    private float radius;
    private int damage;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Attack()
    {
        if (anim != null)
        {
            SetAnimationTrigger("Attack");
        }
        ExecuteAttack();
    }

    public void ExecuteAttack()
    {
        enemy.SetAnimationTrigger("Attack");

        Collider[] hitColliders = Physics.OverlapSphere(enemy.transform.position, radius);
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                //PlayerInfo playerInfo = collision.gameObject.GetComponent<PlayerInfo>();
                //    if (playerInfo != null)
                //    {
                //        playerInfo.TakeDamage(damage);
                //    }
            }
        }
    }
}
