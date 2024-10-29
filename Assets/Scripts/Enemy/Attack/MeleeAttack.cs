using System.Collections;
using UnityEngine;

public class MeleeAttack : Enemy // 근접 공격
{
    [Header("공격 속성")]
    public int damage = 1;
    private float lastAttackTime;

    protected override void Awake()
    {
        base.Awake(); 
    }

    public override void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown && !isAttacking)
        {
            isAttacking = true;
            lastAttackTime = Time.time;
            
            if (player != null)
            {
                SetAnimationState(AnimationState.Attack);
            }
        }
        else
        {
            LookAtPlayer();
        }
    }

    public void ExecuteAttack()
    {
        Debug.Log("때림");
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (angleToPlayer <= 60f && distanceToPlayer <= attackRange)
        {
            Debug.Log("공격 범위 내 플레이어 감지");
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
            foreach (var collider in hitColliders)
            {
                Debug.Log("충돌한 객체: " + collider.gameObject.name);
                if (collider.transform.parent != null && collider.transform.parent.CompareTag("Player"))
                {
                    PlayerHP playerHP = collider.GetComponent<PlayerHP>();
                    if (playerHP != null)
                    {
                        playerHP.TakeDamage(damage);
                        Debug.Log("Player hit, dealing damage: " + damage);
                    }
                    else
                    {
                        Debug.LogWarning("PlayerHP 컴포넌트가 없습니다.");
                    }
                }
            }
        }
        else
        {
            Debug.Log("플레이어가 공격 범위 밖에 있습니다.");
        }
    }


    public void AttackAnimationEnd()
    {
        isAttacking = false;
        StartCoroutine(WaitForCooldown());
    }

    private IEnumerator WaitForCooldown()
    {
        Debug.Log("쿨타임중~~~~~~~~~~~~~~~~~~");
        SetAnimationState(AnimationState.Idle);
        agent.isStopped = true;

        float elapsedTime = 0f;
        while (elapsedTime < attackCooldown)
        {
            if (GetComponent<EnemyAI>().CurrentState is FleeingState)
            {
                Debug.Log("도망 상태로 전환됨 - 쿨타임 중지");
                isAttacking = false;
                agent.isStopped = false;
                yield break;
            }

            LookAtPlayer();
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
        agent.isStopped = false;

        if (!(GetComponent<EnemyAI>().CurrentState is FleeingState))
        {
            if (PlayerInAttackRange())
            {
                Attack();
            }
            else
            {
                ChasePlayer(player.position);
            }
        }
    }
}