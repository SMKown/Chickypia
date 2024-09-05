using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public LayerMask playerLayer;
    [Header("�÷��̾� Ž�� �ݰ�")] public float detectionRadius = 10f;
    [Header("���� ����")] public Transform[] patrolPoints;
    [Header("���� ��Ÿ��")] public float attackCooldown = 1.5f;
    [Header("�̵� �� ��� �ð�")] public float waitTimeNextPatrol = 3f;

    // ���� ���� ���� �ε���
    private int currentPatrolIndex;
    // NavMeshAgent ������Ʈ
    private NavMeshAgent agent;
    // �÷��̾� Transform
    private Transform player;
    // �÷��̾� Ž�� ����
    private bool isPlayerDetected;
    // ���� ����
    private State currentState;
    // Enemy ������Ʈ
    private Enemy enemy;
    // ������ ���� �ð�
    private float lastAttackTime;
    // ��� �ð�
    private float waitTime;

    // ���� ���¸� ��Ÿ���� ������
    private enum State
    {
        Idle,
        Patrolling,
        Chasing,
        Attacking
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
        enemy = GetComponent<Enemy>();
        currentState = State.Patrolling;
        SetNextPatrolPoint();
    }

    void Update()
    {
        switch (currentState) //����
        {
            case State.Idle:
                Idle(); //���
                break;
            case State.Patrolling:
                Patrol(); //����
                break;
            case State.Chasing:
                Chase(); //�߰�
                break;
            case State.Attacking:
                Attack(); //����
                break;
        }
        DetectPlayer();
    }

    void Idle()
    {
        enemy.animator.SetBool("Move", false);
        enemy.animator.SetBool("Idle", true);

        // ��� �ð��� ������ ���� ���·� ��ȯ
        if (Time.time - waitTime >= waitTimeNextPatrol)
        {
            currentState = State.Patrolling;
            enemy.animator.SetBool("Idle", false);
            SetNextPatrolPoint();
        }
    }

    void Patrol()
    {
        // ���� ������ ������ ����
        if (patrolPoints.Length == 0) return;

        // ��ǥ ������ �����ϸ� ��� ���·� ��ȯ
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = State.Idle;
            waitTime = Time.time;
        }
    }

    void DetectPlayer()
    {
        // Ž�� �ݰ� ���� �÷��̾� Ž��
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hitColliders.Length > 0)
        {
            // �÷��̾ �߰��ϸ� ���� ���·� ��ȯ
            player = hitColliders[0].transform;
            isPlayerDetected = true;
            currentState = State.Chasing;
        }
        else if (currentState == State.Chasing)
        {
            // �÷��̾ ��ġ�� ���� ���·� ��ȯ
            isPlayerDetected = false;
            currentState = State.Patrolling;
            SetNextPatrolPoint();
        }
    }

    void Chase()
    {
        // �÷��̾ ������ ����
        if (player == null) return;

        // �÷��̾ ��ǥ �������� ����
        agent.destination = player.position;
        enemy.animator.SetBool("Move", true);

        // �÷��̾���� �Ÿ��� ��������� ���� ���·� ��ȯ
        if (Vector3.Distance(transform.position, player.position) < agent.stoppingDistance)
        {
            currentState = State.Attacking;
        }
    }

    void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        enemy.animator.SetTrigger("Attack");
        lastAttackTime = Time.time;
    }

    void SetNextPatrolPoint()
    {
        // ���� ������ ������ ����
        if (patrolPoints.Length == 0) return;

        // ���� ���� ���� ����
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        agent.destination = patrolPoints[currentPatrolIndex].position;
        agent.updateRotation = true;
        enemy.animator.SetBool("Move", true);
    }

    void OnDrawGizmosSelected() //Ž���ݰ�
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}