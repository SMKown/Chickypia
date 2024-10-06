using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ChargeAttack : Enemy // 돌격 공격
{
    [Header("공격 속성")]
    public int damage = 1;
    [Tooltip("돌격 속도")]
    public float chargeSpeed = 3f;
    [Tooltip("돌격 지속 시간")]
    public float chargeDuration = 3f;
    [Tooltip("공격 딜레이")]
    public float prepareTime = 2f;

    private float lastAttackTime;
    private Rigidbody rb;
    private bool isCharging;
    private Vector3 chargeDirection;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        isCharging = false;
    }

    public override void Attack()
    {
        Debug.Log("Attack method called");
        if (Time.time - lastAttackTime < attackCooldown || isCharging) return;
        lastAttackTime = Time.time;

        SetAnimationState(AnimationState.Attack);
        ExecuteAttack();
    }

    public void ExecuteAttack()
    {
        StartCoroutine(ChargeCoroutine());
    }

    private IEnumerator ChargeCoroutine()
    {
        chargeDirection = (player.position - transform.position).normalized;
        Debug.Log("Charge direction: " + chargeDirection);
        float prepareTimer = prepareTime;
        while (prepareTimer > 0)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            prepareTimer -= Time.deltaTime;
            yield return null;
        }

        float timer = chargeDuration;
        isCharging = true;
        rb.isKinematic = false;
        rb.velocity = chargeDirection * chargeSpeed;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        isCharging = false;

        yield return new WaitForSeconds(attackCooldown);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isCharging && collision.collider.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.collider.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(damage);
            }

            rb.velocity = Vector3.zero;
            isCharging = false;
        }
    }
}
