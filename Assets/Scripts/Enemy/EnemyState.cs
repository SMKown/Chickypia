using UnityEngine;

public abstract class EnemyState
{
    protected EnemyAI enemyAI;

    public EnemyState(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract EnemyState CheckStateTransitions();

    public virtual void ExitState()
    {
    }
    protected bool PlayerInChaseRange()
    {
        return enemyAI.PlayerInSightRange() && !PlayerOutPatrolRange();
    }

    protected bool PlayerOutPatrolRange()
    {
        Vector3[] patrolPoints = enemyAI.GetEnemy().patrolPoints;
        foreach (var point in patrolPoints)
        {
            if (Vector3.Distance(point, enemyAI.player.position) <= enemyAI.GetEnemy().sightRange)
            {
                return false; // 플레이어가 순찰 지점 중 하나에 가까운 경우
            }
        }
        return true;
    }
}

// 적의 대기 상태
public class IdleState : EnemyState
{
    private float idleDuration;
    private float idleTimer;

    public IdleState(EnemyAI enemyAI, float duration) : base(enemyAI)
    {
        idleDuration = duration;
    }

    public override void EnterState()
    {
        enemyAI.GetEnemy().SetAnimationState("Idle", true);
        idleTimer = idleDuration;
    }

    public override void UpdateState()
    {
        idleTimer -= Time.deltaTime;

        if (enemyAI.PlayerInSightRange())
        {
            enemyAI.SwitchState(new ChasingState(enemyAI));
        }
        else if (idleTimer <= 0f)
        {
            enemyAI.SwitchState(new PatrollingState(enemyAI));
        }
    }

    public override EnemyState CheckStateTransitions()
    {
        return this;
    }

    public override void ExitState()
    {
        enemyAI.GetEnemy().SetAnimationState("Idle", false);
    }
}

// 순찰 상태
public class PatrollingState : EnemyState
{
    private Vector3 patrolStartPosition;
    private Vector3 patrolEndPosition;
    private bool movingToEnd = true;
    private bool waitingBeforeMove = false;

    public PatrollingState(EnemyAI enemyAI) : base(enemyAI) { }

    public override void EnterState()
    {
        enemyAI.GetEnemy().SetAnimationState("Move", true);
        SetNextPatrolDestination();
    }

    public override void UpdateState()
    {
        if (!waitingBeforeMove && !enemyAI.agent.pathPending && enemyAI.agent.remainingDistance < 0.5f)
        {
            waitingBeforeMove = true;
            enemyAI.SwitchState(new IdleState(enemyAI, 2f)); // Idle 상태로 전환하여 2초 대기
        }
    }

    public override EnemyState CheckStateTransitions()
    {
        if (PlayerInChaseRange())
        {
            switch (enemyAI.enemyLevel)
            {
                case EnemyLevel.Level1:
                    return new FleeingState(enemyAI);
                case EnemyLevel.Level2:
                case EnemyLevel.Level3:
                    return new ChasingState(enemyAI);
            }
        }
        return this;
    }

    private void SetNextPatrolDestination()
    {
        var enemy = enemyAI.GetEnemy();
        switch (enemy.patrolType)
        {
            case PatrolType.Patrol1:
                if (movingToEnd)
                    SetDestination(enemy.patrolPoints[1]);
                else
                    SetDestination(enemy.patrolPoints[0]);

                movingToEnd = !movingToEnd;
                break;

            case PatrolType.Patrol2:
                int currentIndex = GetClosestPatrolPointIndex();
                var possiblePoints = GetPossiblePoints(currentIndex);
                int nextPoint = possiblePoints[Random.Range(0, possiblePoints.Length)];
                SetDestination(enemy.patrolPoints[nextPoint]);
                break;

            default:
                SetDestination(enemy.initialPosition);
                break;
        }
    }

    private int GetClosestPatrolPointIndex()
    {
        var enemy = enemyAI.GetEnemy();
        int closestIndex = 0;
        float closestDistance = Vector3.Distance(enemy.transform.position, enemy.patrolPoints[0]);

        for (int i = 1; i < enemy.patrolPoints.Length; i++)
        {
            float distance = Vector3.Distance(enemy.transform.position, enemy.patrolPoints[i]);
            if (distance < closestDistance)
            {
                closestIndex = i;
                closestDistance = distance;
            }
        }

        return closestIndex;
    }

    private int[] GetPossiblePoints(int currentIndex)
    {
        switch (currentIndex)
        {
            case 0:
                return new int[] { 1, 2 };
            case 1:
                return new int[] { 0, 3 };
            case 2:
                return new int[] { 0, 3 };
            case 3:
                return new int[] { 1, 2 };
            default:
                return new int[] { 0 };
        }
    }

    private void SetDestination(Vector3 destination)
    {
        enemyAI.agent.SetDestination(destination);
    }

    public override void ExitState()
    {
        waitingBeforeMove = false; // 상태 종료 시 대기 상태 초기화
        enemyAI.GetEnemy().SetAnimationState("Move", false);
    }
}

// 적의 추적 상태
public class ChasingState : EnemyState
{
    public ChasingState(EnemyAI enemyAI) : base(enemyAI) { }

    public override void EnterState()
    {
        enemyAI.GetEnemy().SetAnimationState("Move", true);
        enemyAI.ChasePlayer();
    }

    public override void UpdateState()
    {
        if (PlayerOutPatrolRange())
        {
            enemyAI.SwitchState(new PatrollingState(enemyAI));
        }
        else
        {
            enemyAI.ChasePlayer();
        }
    }

    public override EnemyState CheckStateTransitions()
    {
        if (PlayerOutPatrolRange())
        {
            return new PatrollingState(enemyAI);
        }
        else if (!PlayerInChaseRange())
        {
            return new PatrollingState(enemyAI);
        }
        else if (enemyAI.PlayerInAttackRange())
        {
            return new AttackState(enemyAI);
        }
        return this;
    }

    public override void ExitState()
    {
        enemyAI.GetEnemy().SetAnimationState("Move", false);
    }
}

// 적의 근접 공격 상태
public class AttackState : EnemyState
{
    public AttackState(EnemyAI enemyAI) : base(enemyAI) { }

    public override void EnterState()
    {
        Enemy enemy = enemyAI.GetEnemy();
        enemy.SetAnimationTrigger("Attack");
        enemyAI.GetComponent<EnemyMelee>().Attack();
    }

    public override void UpdateState() { }

    public override EnemyState CheckStateTransitions()
    {
        if (!enemyAI.PlayerInAttackRange())
        {
            return new ChasingState(enemyAI);
        }
        return this;
    }

    public override void ExitState()
    {
        enemyAI.GetEnemy().ResetAnimationState();
    }
}

// 적의 원거리 공격 상태
public class RangedAttackState : EnemyState
{
    public RangedAttackState(EnemyAI enemyAI) : base(enemyAI) { }

    public override void EnterState()
    {
        Enemy enemy = enemyAI.GetEnemy();
        enemy.SetAnimationTrigger("Attack");
        enemyAI.GetComponent<EnemyRange>().Attack();
    }

    public override void UpdateState() { }

    public override EnemyState CheckStateTransitions()
    {
        if (!enemyAI.PlayerInAttackRange())
        {
            return new ChasingState(enemyAI);
        }
        return this;
    }

    public override void ExitState()
    {
        enemyAI.GetEnemy().ResetAnimationState();
    }
}

// 적의 도망 상태
public class FleeingState : EnemyState
{
    public FleeingState(EnemyAI enemyAI) : base(enemyAI) { }

    public override void EnterState()
    {
        enemyAI.FleeFromPlayer();
    }

    public override void UpdateState() { }

    public override EnemyState CheckStateTransitions()
    {
        if (enemyAI.PlayerMovedFar())
        {
            return new PatrollingState(enemyAI);
        }
        return this;
    }

    public override void ExitState()
    {
        enemyAI.GetEnemy().ResetAnimationState();
    }
}
