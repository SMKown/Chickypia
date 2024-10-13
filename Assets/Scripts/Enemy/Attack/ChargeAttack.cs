using System.Collections;
using UnityEngine;

public class ChargeAttack : Enemy
{
    [Header("공격 속성")]
    public int damage = 1;
    public float chargeSpeed = 5f;
    public float chargeDistance = 10f;
    public float attackStartRange = 5f;

    private bool isCharging = false;
    private float lastAttackTime;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Attack()
    {
        if (!isCharging && Time.time - lastAttackTime >= attackCooldown)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackStartRange)
            {
                isCharging = true;
                lastAttackTime = Time.time;
                StartCoroutine(ChargeTowardsPlayer());
            }
        }
    }

    private IEnumerator ChargeTowardsPlayer()
    {
        SetAnimationState(AnimationState.Attack);

        Vector3 targetPosition = player.position;
        Vector3 initialPosition = transform.position;

        while (Vector3.Distance(transform.position, targetPosition) > 0.5f &&
               Vector3.Distance(initialPosition, transform.position) < chargeDistance)
        {
            Vector3 directionToPlayer = (targetPosition - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
            transform.rotation = lookRotation;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, chargeSpeed * Time.deltaTime);
            yield return null;
        }

        if (Vector3.Distance(transform.position, targetPosition) <= 0.5f)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
            foreach (var collider in hitColliders)
            {
                if (collider.CompareTag("Player"))
                {
                    PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();
                    if (playerMovement != null)
                    {
                        playerMovement.TakeDamage(damage);
                    }
                }
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        SetAnimationState(AnimationState.Idle);
        isCharging = false;

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
