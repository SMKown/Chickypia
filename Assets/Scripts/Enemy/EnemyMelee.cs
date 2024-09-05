using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : Enemy
{
    protected override void Attack()
    {
        animator.SetTrigger("Attack");
    }
}