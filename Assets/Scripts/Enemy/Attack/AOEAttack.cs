using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEAttack : Enemy // ���� ����
{
    [Header("���� �Ӽ�")]
    public int damage = 1;
    [Tooltip("���� �ݰ�")]
    public float radius = 3f;
    [Tooltip("���� ������")]
    public float prepareTime = 2f;
    public float attackCooldown = 3f;

    private float lastAttackTime;
    private bool isAttacking = false; 
    private LineRenderer lineRenderer; 

    protected override void Awake()
    {
        base.Awake();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    public override void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            SetAnimationState(AnimationState.Idle);
            return;
        }

        SetAnimationState(AnimationState.Attack);
        isAttacking = true;
        StartCoroutine(PrepareAndExecuteAttack());
    }

    private IEnumerator PrepareAndExecuteAttack()
    {
        lineRenderer.enabled = true;
        Vector3 startPoint = transform.position;
        Vector3 endPoint = startPoint + transform.forward * radius;

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        yield return new WaitForSeconds(prepareTime);

        ExecuteAttack();
        lineRenderer.enabled = false;
        isAttacking = false;
    }

    public void ExecuteAttack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                //PlayerInfo playerInfo = collider.GetComponent<PlayerInfo>();
                //if (playerInfo != null)
                //{
                //    playerInfo.TakeDamage(damage);
                //}
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (isAttacking)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
