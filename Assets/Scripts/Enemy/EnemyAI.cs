using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum PatrolType { FourPt, TwoPt }
public class EnemyAI : MonoBehaviour
{
    [Header("�� �Ӽ�")]
    public NavMeshAgent agent;
    public Transform player;
    [Header("���� ����")]
    public EnemyType enemyType;
    public PatrolType patrolType;

    public EnemyState currentState;
    private bool isTransitioningState;
    [Header("���� ����")]
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

            if (currentState is IdleState && PlayerInAttackRange())
            {
                SwitchState(new AttackState(this));
            }

            EnemyState nextState = currentState.CheckStateTransitions();
            if (nextState != currentState)
            {
                SwitchState(nextState);
            }
        }
    }

    // ���� ��ȯ
    public void SwitchState(EnemyState newState)
    {
        if (isTransitioningState) return;
        isTransitioningState = true;

        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();

        isTransitioningState = false;
    }

    // �÷��̾� ����
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

    // �÷��̾�κ��� ����
    public void FleeFromPlayer()
    {
        enemy.FleeFromPlayer();
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
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return enemy.PlayerInAttackRange();
    }
}