using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public LayerMask playerLayer;
    [Header("공격 쿨타임")] public float attackCooldown = 1.5f;
    [Header("이동 전 대기 시간")] public float waitTimeNextMove = 3f;

    [Header("활동 구역")] public Transform zoneCenter;
    [Header("활동 구역 반경")] public float zoneRadius = 20f;

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
        switch (currentState) // 상태
        {
            case State.Idle:
                Idle(); // 대기
                break;
            case State.Roaming:
                Roam(); // 자유롭게 이동
                break;
            case State.Chasing:
                Chase(); // 추격
                break;
            case State.Attacking:
                Attack(); // 공격
                break;
        }
        
    }

    void Idle()
    {
        enemy.animator.SetBool("Move", false);
        enemy.animator.SetBool("Idle", true);

        // 대기 시간이 지나면 자유롭게 이동 상태로 전환
        if (Time.time - waitTime >= waitTimeNextMove)
        {
            currentState = State.Roaming;
            enemy.animator.SetBool("Idle", false);
            SetRandomDestination();
        }
    }

    void Roam()
    {
        // 목표 지점에 도달하면 대기 상태로 전환
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
                if (!isPlayerDetected) // 이전에 플레이어를 감지하지 않았을 때
                {
                    isPlayerDetected = true;
                    currentState = State.Chasing;
                }
            }
        }
        else if (isPlayerDetected) // 플레이어가 구역을 벗어나면
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

        // 플레이어가 구역을 벗어났는지 확인
        if (!IsPlayerInZone())
        {
            currentState = State.Roaming;  // 플레이어가 구역을 벗어나면 자유롭게 돌아다니는 상태로 전환
            isPlayerDetected = false;
            SetRandomDestination();  // 새로운 랜덤 목적지를 설정
            return;
        }

        // 플레이어가 구역 안에 있으면 추적 계속
        agent.destination = player.position;
        enemy.animator.SetBool("Move", true);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Debug.Log("Distance to Player: " + distanceToPlayer);

        if (distanceToPlayer < enemy.attackDistance)
        {
            Debug.Log("공격모드 전환");
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
        // 구역 내 랜덤 위치 선택
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

    void OnDrawGizmosSelected() // 구역 반경
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(zoneCenter.position, zoneRadius);
    }
}
