using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask Player;
    public float minpatrolRange = 5f;             // ���� �ּ� ����
    public EnemyLevel enemyLevel;                 // ���� ����

    private Vector3 patrolPoint;                  // ���� ����
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

    // ���� ��ȯ
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

    // �÷��̾� ����
    public void ChasePlayer()
    {
        Vector3 playerVelocity = player.GetComponent<Rigidbody>().velocity;
        Vector3 predictedPosition = player.position + playerVelocity * 0.5f;
        enemy.ChasePlayer(predictedPosition);
    }

    // �÷��̾�κ��� ����
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

    // �÷��̾�� ���� �Ÿ� �̻� ���������� Ȯ��
    public bool PlayerMovedFar()
    {
        return Vector3.Distance(transform.position, player.position) > enemy.fleeDistance;
    }

    // �÷��̾ �þ� ������ �ִ��� Ȯ��
    public bool PlayerInSightRange()
    {
        return Physics.CheckSphere(transform.position, enemy.sightRange, Player);
    }

    // �÷��̾ ���� ������ �ִ��� Ȯ��
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

    //// ������ ������ ������ ã��
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

    // ���� ���·� ��ȯ
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