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
    private LineRenderer lineRenderer;
    private Rigidbody rb;
    private bool isCharging;

    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        isCharging = false;
    }

    public override void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown || isCharging) return;
        lastAttackTime = Time.time;

        SetAnimationState(AnimationState.Charge);
        ExecuteAttack();
    }

    public void ExecuteAttack()
    {
        StartCoroutine(ChargeCoroutine());
    }

    private IEnumerator ChargeCoroutine()
    {
        lineRenderer.enabled = true;
        Vector3 startPoint = transform.position;

        float chargeDistance = chargeSpeed * chargeDuration;
        Vector3 endPoint = startPoint + transform.forward * chargeDistance;

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        yield return new WaitForSeconds(prepareTime);

        float timer = chargeDuration;
        isCharging = true;
        rb.isKinematic = false;
        rb.velocity = transform.forward * chargeSpeed;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        isCharging = false;
        lineRenderer.enabled = false;

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
