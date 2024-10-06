using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum PatrolType { FourPt, TwoPt }
public class EnemyAI : MonoBehaviour
{
    [Header("적 속성")]
    public NavMeshAgent agent;
    public Transform player;
    [Header("적의 종류")]
    public EnemyType enemyType;
    public PatrolType patrolType;

    public EnemyState currentState;
    private bool isTransitioningState;
    [Header("도망 여부")]
    public bool hasFledOnce = false;
    private Enemy enemy;

    public Enemy GetEnemy()
    {
        return enemy;
    }
    public EnemyState CurrentState
    {
        get { return currentState; }
    }

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        player = GameObject.Find("Player_AttackMode").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        currentState = new PatrollingState(this, patrolType);
        currentState.EnterState();
    }

    private void Update()
    {
        if (!isTransitioningState)
        {
            currentState.UpdateState();

            EnemyState nextState = currentState.CheckStateTransitions();
            if (nextState != currentState)
            {
                SwitchState(nextState);
            }
        }
    }

    // 상태 전환
    public void SwitchState(EnemyState newState)
    {
        if (isTransitioningState) return;
        isTransitioningState = true;

        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();

        isTransitioningState = false;
    }


    // 플레이어 추적
    public void ChasePlayer()
    {
        if (player != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Vector3 targetPosition = player.position - directionToPlayer * (enemy.attackRange);

            enemy.ChasePlayer(targetPosition);
        }
        else
        {
            Debug.Log("Player not found!");
        }
    }

    // 플레이어로부터 도망
    public void FleeFromPlayer()
    {
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;

        float fleeDistance = enemy.sightRange + 10f;
        Vector3 fleePosition = transform.position + directionAwayFromPlayer * fleeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleePosition, out hit, fleeDistance, NavMesh.AllAreas))
        {
            enemy.FleeFromPlayer(hit.position);
            hasFledOnce = true;
            SwitchState(new IdleState(this, 5f));
        }
        else
        {
            SwitchToChaseOrAttackState();
        }
    }

    private void SwitchToChaseOrAttackState()
    {
        if (PlayerInAttackRange())
        {
            SwitchState(new AttackState(this));
        }
        else if (PlayerInSightRange())
        {
            SwitchState(new ChasingState(this));
        }
        else
        {
            SwitchState(new PatrollingState(this, patrolType));
        }
    }

    public bool PlayerMovedFar()
    {
        return Vector3.Distance(transform.position, player.position) > enemy.sightRange;
    }

    public bool PlayerInSightRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= enemy.sightRange)
        {
            return true;
        }

        return false;
    }

    public bool PlayerInAttackRange()
    {
        return enemy.PlayerInAttackRange();
    }
}