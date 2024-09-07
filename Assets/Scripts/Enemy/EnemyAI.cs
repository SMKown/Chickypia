using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public LayerMask playerLayer;
    [Header("���� ��Ÿ��")] public float attackCooldown = 1.5f;
    [Header("�̵� �� ��� �ð�")] public float waitTimeNextMove = 3f;

    [Header("Ȱ�� ����")] public Transform zoneCenter;
    [Header("Ȱ�� ���� �ݰ�")] public float zoneRadius = 20f;

    private NavMeshAgent agent;
    private Transform player;
    private bool isPlayerDetected;
    private State currentState;
    private Enemy enemy;
    private float lastAttackTime;
    private float waitTime;

    private enum State
    {
        Idle,
        Roaming,
        Chasing,
        Attacking
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = true;
        agent.updateRotation = true;
        enemy = GetComponent<Enemy>();
        currentState = State.Roaming;
        SetRandomDestination();
    }

    void Update()
    {
        DetectPlayer();
        Debug.Log("Current State: " + currentState);
        switch (currentState) // ����
        {
            case State.Idle:
                Idle(); // ���
                break;
            case State.Roaming:
                Roam(); // �����Ӱ� �̵�
                break;
            case State.Chasing:
                Chase(); // �߰�
                break;
            case State.Attacking:
                Attack(); // ����
                break;
        }
        
    }

    void Idle()
    {
        enemy.animator.SetBool("Move", false);
        enemy.animator.SetBool("Idle", true);

        // ��� �ð��� ������ �����Ӱ� �̵� ���·� ��ȯ
        if (Time.time - waitTime >= waitTimeNextMove)
        {
            currentState = State.Roaming;
            enemy.animator.SetBool("Idle", false);
            SetRandomDestination();
        }
    }

    void Roam()
    {
        // ��ǥ ������ �����ϸ� ��� ���·� ��ȯ
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = State.Idle;
            waitTime = Time.time;
        }
    }

    void DetectPlayer()
    {
        if (IsPlayerInZone())
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, zoneRadius, playerLayer);
            if (hitColliders.Length > 0)
            {
                player = hitColliders[0].transform;
                if (!isPlayerDetected) // ������ �÷��̾ �������� �ʾ��� ��
                {
                    isPlayerDetected = true;
                    currentState = State.Chasing;
                }
            }
        }
        else if (isPlayerDetected) // �÷��̾ ������ �����
        {
            currentState = State.Roaming;
            isPlayerDetected = false;
            SetRandomDestination();
        }
    }


    bool IsPlayerInZone()
    {
        if (player == null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, zoneRadius, playerLayer);
            if (hitColliders.Length > 0)
            {
                player = hitColliders[0].transform;
            }
        }

        if (player == null) return false;

        float distanceToZoneCenter = Vector3.Distance(player.position, zoneCenter.position);
        return distanceToZoneCenter <= zoneRadius;
    }


    void Chase()
    {
        if (player == null) return;

        // �÷��̾ ������ ������� Ȯ��
        if (!IsPlayerInZone())
        {
            currentState = State.Roaming;  // �÷��̾ ������ ����� �����Ӱ� ���ƴٴϴ� ���·� ��ȯ
            isPlayerDetected = false;
            SetRandomDestination();  // ���ο� ���� �������� ����
            return;
        }

        // �÷��̾ ���� �ȿ� ������ ���� ���
        agent.destination = player.position;
        enemy.animator.SetBool("Move", true);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Debug.Log("Distance to Player: " + distanceToPlayer);

        if (distanceToPlayer < enemy.attackDistance)
        {
            Debug.Log("���ݸ�� ��ȯ");
            currentState = State.Attacking;
        }
    }

    void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        enemy.animator.SetTrigger("Attack");
        lastAttackTime = Time.time;
    }

    void SetRandomDestination()
    {
        // ���� �� ���� ��ġ ����
        Vector3 randomDirection = Random.insideUnitSphere * zoneRadius;
        randomDirection += zoneCenter.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, zoneRadius, 1))
        {
            agent.destination = hit.position;
            agent.updateRotation = true;
            enemy.animator.SetBool("Move", true);
        }
    }

    void OnDrawGizmosSelected() // ���� �ݰ�
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(zoneCenter.position, zoneRadius);
    }
}
