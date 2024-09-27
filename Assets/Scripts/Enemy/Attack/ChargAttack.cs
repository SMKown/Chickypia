using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargeAttack : Enemy // 돌격 공격
{
    private Enemy enemy;
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