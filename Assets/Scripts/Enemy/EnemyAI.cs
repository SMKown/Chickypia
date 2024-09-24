using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask Player;
    public EnemyLevel enemyLevel;                 // ���� ����

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
        if (!hasFledOnce && PlayerInSightRange()) // ���� ������ ���� ���� �÷��̾ �þ� ���� �ȿ� ���� ��
        {
            Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;

            float fleeDistance = enemy.sightRange + 10f; // ���� �Ÿ� ���
            Vector3 fleePosition = transform.position + directionAwayFromPlayer * fleeDistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(fleePosition, out hit, fleeDistance, NavMesh.AllAreas))
            {
                enemy.FleeFromPlayer(hit.position);
                hasFledOnce = true; // ���� ��� ����
            }
            else
            {
                SwitchToChaseOrAttackState();
            }
        }
        else
        {
            SwitchToChaseOrAttackState(); // �̹� ������ ���� ���� �� �߰� �Ǵ� ���� ���·� ��ȯ
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
        // �÷��̾ ���� �þ� ���� �ٱ��� �ִ� ��� true ��ȯ
        return Vector3.Distance(transform.position, player.position) > enemy.sightRange;
    }

    public bool PlayerInSightRange()
    {
        // �÷��̾ �þ� ���� ���� �ִ��� Ȯ��
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