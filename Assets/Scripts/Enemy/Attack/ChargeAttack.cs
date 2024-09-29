using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ChargeAttack : Enemy // ���� ����
{
    [Header("���� �Ӽ�")]
    public int damage = 1;
    [Tooltip("���� �ӵ�")]
    public float chargeSpeed = 3f;
    [Tooltip("���� ���� �ð�")]
    public float chargeDuration = 3f;
    [Tooltip("���� ������")]
    public float prepareTime = 2f;
    public float attackCooldown = 5f;

    private float lastAttackTime;
    private LineRenderer lineRenderer;

    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    public override void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
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
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        while (timer > 0)
        {
            agent.velocity = transform.forward * chargeSpeed;
            timer -= Time.deltaTime;
            yield return null;
        }

        agent.velocity = Vector3.zero;
        lineRenderer.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //PlayerInfo playerInfo = collision.gameObject.GetComponent<PlayerInfo>();
            //if (playerInfo != null)
            //{
            //    playerInfo.TakeDamage(damage); // �÷��̾�� ������ ����
            //}
        }
    }
}
