using System.Collections;
using UnityEngine;

public class MeleeAttack : Enemy // 근접 공격
{
    [Header("공격 속성")]
    public int damage = 1;
    public float attackCooldown = 3f;

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

        lastAttackTime = Time.time;

        SetAnimationState(AnimationState.Attack);
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