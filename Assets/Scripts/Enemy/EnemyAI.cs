using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask Player;
    public EnemyLevel enemyLevel;                 // 적의 레벨

    public EnemyState currentState;
    private bool isTransitioningState;

    private Enemy enemy;
    private bool hasFledOnce = false;
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
        if (!hasFledOnce && PlayerInSightRange()) // 아직 도망간 적이 없고 플레이어가 시야 범위 안에 있을 때
        {
            Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;

            float fleeDistance = enemy.sightRange + 10f; // 도망 거리 계산
            Vector3 fleePosition = transform.position + directionAwayFromPlayer * fleeDistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(fleePosition, out hit, fleeDistance, NavMesh.AllAreas))
            {
                enemy.FleeFromPlayer(hit.position);
                hasFledOnce = true; // 도망 기록 설정
            }
            else
            {
                SwitchToChaseOrAttackState();
            }
        }
        else
        {
            SwitchToChaseOrAttackState(); // 이미 도망간 적이 있을 때 추격 또는 공격 상태로 전환
        }
    }

    private void SwitchToChaseOrAttackState()
    {
        if (PlayerInSightRange() && !PlayerInAttackRange())
        {
            SwitchState(new ChasingState(this)); 
        }
        else if (PlayerInAttackRange())
        {
            SwitchToAttackState(); 
        }
        else
        {
            SwitchState(new PatrollingState(this)); 
        }
    }

    public bool PlayerMovedFar()
    {
        // 플레이어가 적의 시야 범위 바깥에 있는 경우 true 반환
        return Vector3.Distance(transform.position, player.position) > enemy.sightRange;
    }

    public bool PlayerInSightRange()
    {
        // 플레이어가 시야 범위 내에 있는지 확인
        if (Vector3.Distance(transform.position, player.position) <= enemy.sightRange)
        {
            RaycastHit hit;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, directionToPlayer, out hit, enemy.sightRange))
            {
                if (hit.transform == player)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool PlayerInAttackRange()
    {
        return Physics.CheckSphere(transform.position, enemy.attackRange, Player);
    }

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