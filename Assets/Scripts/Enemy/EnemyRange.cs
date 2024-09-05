using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRange : Enemy
{
    public GameObject BallPrefab;
    public Transform firePoint;

    protected override void Attack()
    {
        ShootBall();
    }

    void ShootBall()
    {
        if (BallPrefab != null && player != null)
        {
            GameObject ball = Instantiate(BallPrefab, firePoint.position, firePoint.rotation);

            Projectile projectile = ball.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(player.position);
            }

            animator.SetTrigger("Attack");
        }
    }
}