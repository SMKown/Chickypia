using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ChargeAttack : Enemy // 돌격 공격
{
    public int damage;
    public float chargeSpeed;
    public float chargeDuration;
    public float prepareTime;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Attack()
    {
        if (anim != null)
        {
            SetAnimationTrigger("Attack");
        }
        ExecuteAttack();
    }

    public void ExecuteAttack()
    {
        StartCoroutine(ChargeCoroutine());
    }

    private IEnumerator ChargeCoroutine()
    {
        yield return new WaitForSeconds(prepareTime);

        float timer = chargeDuration;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        while (timer > 0)
        {
            agent.velocity = transform.forward * chargeSpeed;
            timer -= Time.deltaTime;
            yield return null;
        }

        agent.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //PlayerInfo playerInfo = collision.gameObject.GetComponent<PlayerInfo>();
            //if (playerInfo != null)
            //{
            //    playerInfo.TakeDamage(damage); // 플레이어에게 데미지 입힘
            //}
        }
    }
}
