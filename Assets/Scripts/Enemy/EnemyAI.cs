using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float detectionRadius = 10f;
    public LayerMask playerLayer;
    public float attackCooldown = 1.5f;
    public float waitTimeNextPatrolling = 3f;

    private int currentPatrolIndex;
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
        Patrolling,
        Chasing,
        Attacking
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<Enemy>();
        currentState = State.Patrolling;
        SetNextPatrolPoint();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Patrolling:
                Patrol();
                break;
            case State.Chasing:
                Chase();
                break;
            case State.Attacking:
                Attack();
                break;
        }

        DetectPlayer();
    }

    void Idle()
    {
        enemy.animator.SetBool("Move", false);
        enemy.animator.SetBool("Idle", true);

        if (Time.time - waitTime > waitTimeNextPatrolling)
        {
            currentState = State.Patrolling;
            enemy.animator.SetBool("Idle", false);
            SetNextPatrolPoint();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (agent.remainingDistance < agent.stoppingDistance)
        {
            currentState = State.Idle;
            waitTime = Time.time;
        }
    }

    void DetectPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hitColliders.Length > 0)
        {
            player = hitColliders[0].transform;
            isPlayerDetected = true;
            currentState = State.Chasing;
        }
        else if (currentState == State.Chasing)
        {
            isPlayerDetected = false;
            currentState = State.Patrolling;
            SetNextPatrolPoint();
        }
    }

    void Chase()
    {
        if (player == null) return;

        agent.destination = player.position;
        enemy.animator.SetBool("Move", true);

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
        if (patrolPoints.Length == 0) return;

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        agent.destination = patrolPoints[currentPatrolIndex].position;
        enemy.animator.SetBool("Move", true);
    }
}
