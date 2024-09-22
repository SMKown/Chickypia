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
        float distanceFromInitialPosition = Vector3.Distance(enemyAI.GetEnemy().initialPosition, enemyAI.player.position);
        return distanceFromInitialPosition > enemyAI.GetEnemy().patrolRange;
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

// 적의 순찰 상태
public class PatrollingState : EnemyState
{
    public PatrollingState(EnemyAI enemyAI) : base(enemyAI) { }

    public override void EnterState()
    {
        SetNextDestination();
    }

    public override void UpdateState()
    {
        PatrolBehavior();
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

    private void SetNextDestination()
    {
        enemyAI.GetEnemy().SetAnimationState("Move", true);
        enemyAI.SetDestinationRandomPoint();
    }

    private void PatrolBehavior()
    {
        if (!enemyAI.agent.pathPending && (enemyAI.agent.remainingDistance <= enemyAI.agent.stoppingDistance || enemyAI.agent.velocity.sqrMagnitude == 0f))
        {
            enemyAI.SwitchState(new IdleState(enemyAI, 2f));
        }
    }


    public override void ExitState()
    {
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