using UnityEngine;
using System.Collections;

public class EnemyMelee : Enemy
{
    public int attackDamage;
    public float attackCooldown = 1f;
    private bool alreadyAttacked;

    public override void Attack()
    {
        if (!alreadyAttacked)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        SetAnimationTrigger("Attack");
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            //PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            //if (playerHealth != null)
            //{
            //    playerHealth.TakeDamage(attackDamage);
            //}
        }

        alreadyAttacked = true;
        yield return new WaitForSeconds(attackCooldown - 0.1f);
        alreadyAttacked = false;
    }
}