using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRange : Enemy
{
    public GameObject BallPrefab; 

    // ���Ÿ� ���� ����
    protected override void Attack()
    {
        ShootBall(); // ����ü �߻�
    }

    void ShootBall()
    {

    }
}