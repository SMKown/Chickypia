using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRange : Enemy
{
    public GameObject BallPrefab; 

    // 원거리 공격 로직
    protected override void Attack()
    {
        ShootBall(); // 투사체 발사
    }

    void ShootBall()
    {

    }
}