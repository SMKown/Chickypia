using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IEnemyAttack
{
    void ExecuteAttack();
}

public class ChargeAttack : IEnemyAttack
{
    private Enemy enemy;
    private NavMeshAgent agent;
    private float chargeSpeed;
    private float chargeDuration;

    public ChargeAttack(Enemy enemy, NavMeshAgent agent, float speed, float duration)
    {
        this.enemy = enemy;
        this.agent = agent;
        this.chargeSpeed = speed;
        this.chargeDuration = duration;
    }

    public void ExecuteAttack()
    {
        enemy.StartCoroutine(ChargeCoroutine());
    }

    private IEnumerator ChargeCoroutine()
    {
        float timer = chargeDuration;
        while (timer > 0)
        {
            agent.velocity = enemy.transform.forward * chargeSpeed;
            timer -= Time.deltaTime;
            yield return null;
        }
        agent.velocity = Vector3.zero;
    }
}

public class AOEAttack : IEnemyAttack
{
    private Enemy enemy;
    private float radius;
    private int damage;

    public AOEAttack(Enemy enemy, float radius, int damage)
    {
        this.enemy = enemy;
        this.radius = radius;
        this.damage = damage;
    }

    public void ExecuteAttack()
    {
        Collider[] colliders = Physics.OverlapSphere(enemy.transform.position, radius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // 플레이어에게 데미지를 주는 로직
                Debug.Log("Player hit by AOE Attack");
            }
        }
    }
}