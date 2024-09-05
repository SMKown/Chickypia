using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public LayerMask playerLayer;
    [Header("플레이어 탐지 반경")] public float detectionRadius = 10f;
    [Header("순찰 지점")] public Transform[] patrolPoints;
    [Header("공격 쿨타임")] public float attackCooldown = 1.5f;
    [Header("이동 전 대기 시간")] public float waitTimeNextPatrol = 3f;

    // 현재 순찰 지점 인덱스
    private int currentPatrolIndex;
    // NavMeshAgent 컴포넌트
    private NavMeshAgent agent;
    // 플레이어 Transform
    private Transform player;
    // 플레이어 탐지 여부
    private bool isPlayerDetected;
    // 현재 상태
    private State currentState;
    // Enemy 컴포넌트
    private Enemy enemy;
    // 마지막 공격 시간
    private float lastAttackTime;
    // 대기 시간
    private float waitTime;

    // 적의 상태를 나타내는 열거형
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
        switch (currentState) //상태
        {
            case State.Idle:
                Idle(); //대기
                break;
            case State.Patrolling:
                Patrol(); //순찰
                break;
            case State.Chasing:
                Chase(); //추격
                break;
            case State.Attacking:
                Attack(); //공격
                break;
        }
        DetectPlayer();
    }

    void Idle()
    {
        enemy.animator.SetBool("Move", false);
        enemy.animator.SetBool("Idle", true);

        // 대기 시간이 지나면 순찰 상태로 전환
        if (Time.time - waitTime >= waitTimeNextPatrol)
        {
            currentState = State.Patrolling;
            enemy.animator.SetBool("Idle", false);
            SetNextPatrolPoint();
        }
    }

    void Patrol()
    {
        // 순찰 지점이 없으면 리턴
        if (patrolPoints.Length == 0) return;

        // 목표 지점에 도달하면 대기 상태로 전환
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = State.Idle;
            waitTime = Time.time;
        }
    }

    void DetectPlayer()
    {
        // 탐지 반경 내의 플레이어 탐지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hitColliders.Length > 0)
        {
            // 플레이어를 발견하면 추적 상태로 전환
            player = hitColliders[0].transform;
            isPlayerDetected = true;
            currentState = State.Chasing;
        }
        else if (currentState == State.Chasing)
        {
            // 플레이어를 놓치면 순찰 상태로 전환
            isPlayerDetected = false;
            currentState = State.Patrolling;
            SetNextPatrolPoint();
        }
    }

    void Chase()
    {
        // 플레이어가 없으면 리턴
        if (player == null) return;

        // 플레이어를 목표 지점으로 설정
        agent.destination = player.position;
        enemy.animator.SetBool("Move", true);

        // 플레이어와의 거리가 가까워지면 공격 상태로 전환
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
        // 순찰 지점이 없으면 리턴
        if (patrolPoints.Length == 0) return;

        // 다음 순찰 지점 설정
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        agent.destination = patrolPoints[currentPatrolIndex].position;
        agent.updateRotation = true;
        enemy.animator.SetBool("Move", true);
    }

    void OnDrawGizmosSelected() //탐지반경
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}