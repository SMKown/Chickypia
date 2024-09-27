using System.Collections;
using UnityEngine;

public class MeleeAttack : Enemy // 근접 공격
{
    public int damage;

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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var collider in hitColliders)
        {
            //if (collision.gameObject.CompareTag("Player"))
            //{
            //    PlayerInfo playerInfo = collision.gameObject.GetComponent<PlayerInfo>();
            //    if (playerInfo != null)
            //    {
            //        playerInfo.TakeDamage(damage);
            //    }
            //}
        }
    }
}