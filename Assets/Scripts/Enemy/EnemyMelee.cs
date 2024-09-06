using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : Enemy
{
    protected override void Attack()
    {
        Debug.Log("EnemyMelee Attack triggered");
        animator.SetTrigger("Attack");

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log("Animator State: " + stateInfo.IsName("Attack"));
    }
}