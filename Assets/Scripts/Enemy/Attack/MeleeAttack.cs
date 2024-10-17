using System.Collections;
using UnityEngine;

public class MeleeAttack : Enemy // ���� ����
{
    [Header("���� �Ӽ�")]
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
        Debug.Log("hit");
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer <= 60f)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, sightRange);
            foreach (var collider in hitColliders)
            {
                if (collider.CompareTag("Player"))
                {
                    PlayerHP playerHP = collider.GetComponent<PlayerHP>();
                    if (playerHP != null)
                    {
                        playerHP.TakeDamage(damage);
                        Debug.Log("Player hit, dealing damage: " + damage);
                    }
                }
            }
        }
    }

    public void AttackAnimationEnd()
    {
        StartCoroutine(WaitForCooldown());
    }

    private IEnumerator WaitForCooldown()
    {
        SetAnimationState(AnimationState.Idle);
        agent.isStopped = true;

        float elapsedTime = 0f;
        while (elapsedTime < attackCooldown)
        {
            LookAtPlayer();
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
        agent.isStopped = false;

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