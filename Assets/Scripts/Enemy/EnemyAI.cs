using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask Player;
    public float minpatrolRange = 5f;             // 순찰 최소 범위
    public EnemyLevel enemyLevel;                 // 적의 레벨

    private Vector3 patrolPoint;                  // 순찰 지점
    private bool patrolPointSet;

    public EnemyState currentState;
    private bool isTransitioningState;

    private Enemy enemy;

    public Enemy GetEnemy()
    {
        return enemy;
    }

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        currentState = new PatrollingState(this);
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
        if (!isTransitioningState)
        {
            isTransitioningState = true;

            currentState.ExitState();
            currentState = newState;
            currentState.EnterState();

            isTransitioningState = false;
        }
    }

    // 플레이어 추적
    public void ChasePlayer()
    {
        Vector3 playerVelocity = player.GetComponent<Rigidbody>().velocity;
        Vector3 predictedPosition = player.position + playerVelocity * 0.5f;
        enemy.ChasePlayer(predictedPosition);
    }

    // 플레이어로부터 도망
    public void FleeFromPlayer()
    {
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        Vector3 fleePosition = transform.position + directionAwayFromPlayer * enemy.patrolRange;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleePosition, out hit, enemy.patrolRange, NavMesh.AllAreas))
        {
            enemy.FleeFromPlayer(hit.position);
        }
        else
        {
            SwitchState(new PatrollingState(this));
        }
    }

    // 플레이어와 일정 거리 이상 떨어졌는지 확인
    public bool PlayerMovedFar()
    {
        return Vector3.Distance(transform.position, player.position) > enemy.fleeDistance;
    }

    // 플레이어가 시야 범위에 있는지 확인
    public bool PlayerInSightRange()
    {
        return Physics.CheckSphere(transform.position, enemy.sightRange, Player);
    }

    // 플레이어가 공격 범위에 있는지 확인
    public bool PlayerInAttackRange()
    {
        return Physics.CheckSphere(transform.position, enemy.attackRange, Player);
    }

    //public void SetDestinationRandomPoint()
    //{
    //    int maxAttempts = 30;
    //    patrolPointSet = false;

    //    for (int i = 0; i < maxAttempts; i++)
    //    {
    //        SearchPatrolPoint();

    //        if (Vector3.Distance(enemy.initialPosition, patrolPoint) <= enemy.patrolRange && Vector3.Distance(transform.position, patrolPoint) >= minpatrolRange)
    //        {
    //            patrolPointSet = true;
    //            agent.SetDestination(patrolPoint);
    //            enemy.SetAnimationState("Move", true);
    //            break;
    //        }
    //    }
    //}

    //// 순찰할 랜덤한 지점을 찾기
    //private void SearchPatrolPoint()
    //{
    //    float randomZ = Random.Range(-patrolRange, patrolRange);
    //    float randomX = Random.Range(-patrolRange, patrolRange);

    //    Vector3 randomPoint = new Vector3(enemy.initialPosition.x + randomX, enemy.initialPosition.y, enemy.initialPosition.z + randomZ);

    //    NavMeshHit hit;
    //    if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
    //    {
    //        patrolPoint = hit.position;
    //    }
    //}

    // 공격 상태로 전환
    public void SwitchToAttackState()
    {
        switch (enemyLevel)
        {
            case EnemyLevel.Level2:
                SwitchState(new AttackState(this));
                break;
            case EnemyLevel.Level3:
                SwitchState(new RangedAttackState(this));
                break;
        }
    }
}