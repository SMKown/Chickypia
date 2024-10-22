using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    public virtual void ExitState(){}
    protected bool PlayerInChaseRange()
    {
        return enemyAI.PlayerInSightRange() && !PlayerOutPatrolRange();
    }

    protected bool PlayerOutPatrolRange()
    {
        Vector3[] patrolPoints = enemyAI.GetEnemy().patrolPoints;
        float patrolRadius = 20f;
        float sightRange = enemyAI.GetEnemy().sightRange;  // 적의 시야 범위

        foreach (var point in patrolPoints)
        {
            if (Vector3.Distance(point, enemyAI.player.position) <= patrolRadius)
            {
                return false;
            }
        }
        if (Vector3.Distance(enemyAI.player.position, enemyAI.transform.position) > sightRange)
        {
            return true;
        }
        return false;
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
        enemyAI.GetEnemy().SetAnimationState(AnimationState.Idle);
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
            if (!(enemyAI.CurrentState is PatrollingState))
            {
                enemyAI.SwitchState(new PatrollingState(enemyAI, enemyAI.patrolType));
            }
        }
    }

    public override EnemyState CheckStateTransitions()
    {
        return this;
    }

    public override void ExitState()
    {
        enemyAI.GetEnemy().SetAnimationState(AnimationState.Idle);
    }
}

// 순찰 상태
public class PatrollingState : EnemyState
{
    private PatrolType patrolType;
    private bool movingToPointA;
    private float twoPtWaitTime = 1f;
    private float waitTimer;

    public PatrollingState(EnemyAI enemyAI, PatrolType patrolType) : base(enemyAI) 
    {
        this.patrolType = patrolType;
        this.movingToPointA = false;
    }

    public override void EnterState()
    {
        enemyAI.GetEnemy().SetAnimationState(AnimationState.Move);
        if (patrolType == PatrolType.FourPt)
        {
            SetNextFourPatrolDestination();
        }
        else if (patrolType == PatrolType.TwoPt)
        {
            SetNextTwoPatrolDestination();
        }
    }

    public override void UpdateState()
    {
        if (patrolType == PatrolType.FourPt)
        {
            if(!enemyAI.agent.pathPending && enemyAI.agent.remainingDistance <= enemyAI.agent.stoppingDistance + 0.2f)
            {
                enemyAI.SwitchState(new IdleState(enemyAI, 2f));
            }
        }
        else if (patrolType == PatrolType.TwoPt)
        {
            if (!enemyAI.agent.pathPending && enemyAI.agent.remainingDistance <= enemyAI.agent.stoppingDistance + 0.2f)
            {
                if (waitTimer <= 0f)
                {
                    SetNextTwoPatrolDestination();
                    waitTimer = twoPtWaitTime;
                }
                else
                {
                    waitTimer -= Time.deltaTime;
                    enemyAI.GetEnemy().SetAnimationState(AnimationState.Idle);
                }
            }
            else
            {
                enemyAI.GetEnemy().SetAnimationState(AnimationState.Move);
            }
        }
    }

    public override EnemyState CheckStateTransitions()
    {
        if (PlayerInChaseRange())
        {
            if (enemyAI.enemyType == EnemyType.FightType)
            {
                return new ChasingState(enemyAI);
            }
            else if (enemyAI.enemyType == EnemyType.Runtype)
            {
                return new ChasingState(enemyAI);
            }
        }
        return this;
    }

    private void SetNextFourPatrolDestination()
    {
        var enemy = enemyAI.GetEnemy();
        int currentIndex = GetPatrolPointIndex();
        var possiblePoints = GetPossiblePoints(currentIndex);
        int nextPoint = possiblePoints[Random.Range(0, possiblePoints.Length)];
        SetDestination(enemy.patrolPoints[nextPoint]);
    }
    private void SetNextTwoPatrolDestination()
    {
        var enemy = enemyAI.GetEnemy();
        if (enemy.patrolPoints.Length >= 2)
        {
            if (movingToPointA)
            {
                SetDestination(enemy.patrolPoints[0]);
            }
            else
            {
                SetDestination(enemy.patrolPoints[1]);
            }
            movingToPointA = !movingToPointA;
        }
    }
    private void SetDestination(Vector3 destination)
    {
        enemyAI.agent.SetDestination(destination);
    }
    
    private int GetPatrolPointIndex()
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

    public override void ExitState()
    {
        enemyAI.GetEnemy().SetAnimationState(AnimationState.Move);
    }
}

// 적의 추적 상태
public class ChasingState : EnemyState
{
    public ChasingState(EnemyAI enemyAI) : base(enemyAI) { }

    public override void EnterState()
    {
        enemyAI.GetEnemy().SetAnimationState(AnimationState.Move);
        enemyAI.ChasePlayer();
    }

    public override void UpdateState()
    {
        if (enemyAI.PlayerInAttackRange())
        {
            enemyAI.SwitchState(new AttackState(enemyAI));
        }
        else if (enemyAI.PlayerMovedFar())
        {
            enemyAI.SwitchState(new PatrollingState(enemyAI, enemyAI.patrolType));
        }
        else
        {
            enemyAI.ChasePlayer();
        }
    }

    public override EnemyState CheckStateTransitions()
    {
        if (enemyAI.PlayerInAttackRange()) 
        {
            return new AttackState(enemyAI);
        }
        else if (PlayerOutPatrolRange())
        {
            return new PatrollingState(enemyAI, enemyAI.patrolType);
        }
        else if (!PlayerInChaseRange())
        {
            return new PatrollingState(enemyAI, enemyAI.patrolType);
        }
        return this;
    }

    public override void ExitState()
    {
        enemyAI.GetEnemy().SetAnimationState(AnimationState.Move);
    }
}

// 적의 공격 상태
public class AttackState : EnemyState
{
    public AttackState(EnemyAI enemyAI) : base(enemyAI) {}

    public override void EnterState()
    {
        Enemy enemy = enemyAI.GetEnemy();
        if (!enemy.isAttacking)
        {
            Vector3 directionToPlayer = (enemyAI.player.position - enemy.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
            enemy.transform.rotation = lookRotation;
            enemy.Attack();
        }
    }

    public override void UpdateState() 
    {
        if (!enemyAI.PlayerInAttackRange())
        {
            enemyAI.SwitchState(new IdleState(enemyAI, 2f));
        }
    }

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
    private float OutTime = 0f;
    private float maxOutTime = 6f;
    private float MinDistance = 5f;

    public FleeingState(EnemyAI enemyAI) : base(enemyAI) { }

    public override void EnterState()
    {
        Debug.Log("도망상태");
        enemyAI.GetEnemy().ResetAnimationState();
        enemyAI.FleeFromPlayer();
        enemyAI.hasFledOnce = true;
        OutTime = 0f;
    }

    public override void UpdateState() 
    {
        if (enemyAI.PlayerMovedFar() || Vector3.Distance(enemyAI.transform.position, enemyAI.player.position) > MinDistance)
        {
            OutTime += Time.deltaTime;
            if (OutTime >= maxOutTime)
            {
                enemyAI.SwitchState(new PatrollingState(enemyAI, enemyAI.patrolType));
            }
        }
    }

    public override EnemyState CheckStateTransitions()
    {
        return this;
    }

    public override void ExitState()
    {
        enemyAI.GetEnemy().ResetAnimationState();
    }
}