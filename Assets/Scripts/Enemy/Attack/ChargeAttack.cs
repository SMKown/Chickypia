using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ChargeAttack : Enemy
{
    [Header("공격 속성")]
    public int damage = 1;
    [Tooltip("돌격 속도")]
    public float chargeSpeed = 6f;

    public float CharginRange = 6f;

    private bool isCharging = false;
    private bool isAttackOnCooldown = false;
    private Vector3 chargeDestination;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Attack()
    {
        if (!isCharging && !isAttackOnCooldown && player != null)
        {
            isCharging = true;
            chargeDestination = player.position;

            Vector3 chargeDirection = (chargeDestination - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(chargeDirection);
            transform.rotation = lookRotation;

            SetAnimationState(AnimationState.Attack);
        }
    }

    private void Update()
    {
        if (isCharging)
        {
            ExecuteCharge();
        }
    }

    private void ExecuteCharge()
    {
        Vector3 direction = (chargeDestination - transform.position).normalized;
        transform.position += direction * chargeSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, chargeDestination) < 0.1f)
        {
            EndCharge();
        }
    }

    private void EndCharge()
    {
        if (isCharging)
        {
            isCharging = false;
            StartCoroutine(WaitForCooldown());
        }
    }

    private IEnumerator WaitForCooldown()
    {
        isAttackOnCooldown = true;
        SetAnimationState(AnimationState.Idle);
        agent.isStopped = true;
        yield return new WaitForSeconds(attackCooldown);

        isAttackOnCooldown = false;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (isCharging && collision.gameObject.CompareTag("Player"))
        {
            ExecuteAttack();
        }
    }

    public void ExecuteAttack()
    {
        
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.TakeDamage(damage);
        }
        EndCharge();
    }

    public new bool PlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) <= attackRange;
    }
}
