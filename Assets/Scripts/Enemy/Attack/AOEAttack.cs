using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEAttack : Enemy // 범위 공격
{
    [Header("공격 속성")]
    public int damage = 1;
    [Tooltip("공격 반경")]
    public float radius = 3f;

    private float lastAttackTime;
    private bool isAttacking = false;
    private LineRenderer lineRenderer;

    protected override void Awake()
    {
        base.Awake();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.loop = true;
        lineRenderer.positionCount = 50;
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
        ShowAttackArea();
    }

    private void ShowAttackArea()
    {
        if (player != null)
        {
            Vector3 playerPosition = player.position;
            playerPosition.y = 0.1f;

            float angleStep = 360f / lineRenderer.positionCount;
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                Vector3 pointPosition = new Vector3(x, 0, z) + playerPosition;
                lineRenderer.SetPosition(i, pointPosition);
            }

            lineRenderer.enabled = true;
        }
    }

    public void ExecuteAttack()
    {
        if (player != null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(player.position, radius);
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

        HideAttackArea();
        isAttacking = false;
        lastAttackTime = Time.time;
    }

    private void HideAttackArea()
    {
        lineRenderer.enabled = false;
    }
}
