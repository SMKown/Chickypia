using UnityEngine;
using System.Collections;

public class EnemyRange : Enemy
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public float attackCooldown = 2f;
    public int projectileDamage = 1;
    private bool alreadyAttacked;

    private ProjectilePool projectilePool;

    protected override void Awake()
    {
        base.Awake();
        projectilePool = FindObjectOfType<ProjectilePool>();
    }

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


        GameObject projectile = projectilePool.GetProjectile();
        projectile.transform.position = transform.position + Vector3.up * 1.5f;


        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.damage = projectileDamage;
        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.forward * projectileSpeed, ForceMode.Impulse);

        alreadyAttacked = true;
        yield return new WaitForSeconds(attackCooldown - 0.1f);
        alreadyAttacked = false;
    }
}
