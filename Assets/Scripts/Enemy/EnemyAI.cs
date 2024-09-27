using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public EnemyType enemyType;

    public EnemyState currentState;
    private bool isTransitioningState;

    private Enemy enemy;
    private bool hasFledOnce = false;
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
        if (player != null)
        {
            Debug.Log("player found");
            Vector3 playerVelocity = player.GetComponent<CharacterController>().velocity;
            Vector3 predictedPosition = player.position + playerVelocity;
            enemy.ChasePlayer(predictedPosition);
        }
        else
        {
            // 플레이어를 찾지 못했을 때의 처리
            Debug.Log("Player not found!");
        }
    }

    // 플레이어로부터 도망
    public void FleeFromPlayer()
    {
        if (!hasFledOnce && PlayerInSightRange())
        {
            Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;

            float fleeDistance = enemy.sightRange + 10f;
            Vector3 fleePosition = transform.position + directionAwayFromPlayer * fleeDistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(fleePosition, out hit, fleeDistance, NavMesh.AllAreas))
            {
                enemy.FleeFromPlayer(hit.position);
                hasFledOnce = true;
            }
            else
            {
                SwitchToChaseOrAttackState();
            }
        }
        else
        {
            SwitchToChaseOrAttackState();
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
            SwitchState(new AttackState(this));
        }
        else
        {
            SwitchState(new PatrollingState(this)); 
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