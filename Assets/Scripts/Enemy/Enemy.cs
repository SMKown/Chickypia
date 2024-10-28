using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { Runtype, FightType }
public enum AnimationState { Idle, Move, Attack, Damage, Die }
public abstract class Enemy : MonoBehaviour
{
    public int health;
    [Header("시야 및 공격 범위")]
    [Space(3)]public float sightRange;
    public float attackRange;
    [Header("공격 쿨타임")]
    public float attackCooldown;

    protected Animator anim;
    protected Transform player;
    public bool isAttacking = false;
    [HideInInspector]public NavMeshAgent agent;
    [Header("순찰 포인트")]
    public Vector3[] patrolPoints;
    [Header("도망 포인트")]
    public Vector3[] FleePoints;

    public GameObject FlashTrans;

    public ItemData dropItemData;
    public GameObject dropItemPrefab;

    public AudioSource damageSound;

    protected virtual void Awake()
    {
        player = GameObject.Find("PlayerAttackMode").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public virtual void TakeDamage(int damage, Vector3 playerForwardDirection, float knockbackForce)
    {
        if (health <= 0) return;

        health -= damage;

        if (damageSound != null)
        {
            damageSound.Play();
        }

        Knockback(playerForwardDirection, knockbackForce);
        StartCoroutine(FlashTR());
        SetAnimationState(AnimationState.Damage);

        if (health > 0)
        {
            EnemyAI enemyAI = GetComponent<EnemyAI>();
            if (enemyAI.enemyType == EnemyType.Runtype || isAttacking)
            {
                if (!enemyAI.hasFledOnce)
                {
                    isAttacking = false;
                    enemyAI.SwitchState(new FleeingState(enemyAI));
                }
            }
        }
        if (health <= 0) Die();
    }

    private void Knockback(Vector3 playerForwardDirection, float force)
    {
        Debug.Log("넉백");
        if (health > 0)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 knockbackDirection = playerForwardDirection.normalized;

                agent.isStopped = true;
                rb.AddForce(knockbackDirection * force, ForceMode.Impulse);
                StartCoroutine(ReEnableNavMeshAgent());
            }
        }
    }

    private IEnumerator ReEnableNavMeshAgent()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        yield return new WaitForSeconds(1.6f);
        rb.isKinematic = false;
        agent.isStopped = false;
    }

    private IEnumerator FlashTR()
    {
        int flashCount = 8;
        for (int i = 0; i < flashCount; i++)
        {
            FlashTrans.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            FlashTrans.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void OnDamageAnimationEnd()
    {
        if (health > 0)
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);

            if (PlayerInAttackRange())
            {
                SetAnimationState(AnimationState.Move);
                SetAnimationState(AnimationState.Attack);
                Attack();
            }
            else if (distanceToPlayer <= sightRange)
            {
                SetAnimationState(AnimationState.Move);
                ChasePlayer(player.position);
            }
            else
            {
                SetAnimationState(AnimationState.Move);
                SetAnimationState(AnimationState.Idle);
            }
        }
    }

    protected void Die()
    {
        SetAnimationState(AnimationState.Die);
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        DropItem();
        Destroy(gameObject, 1.5f);
    }

    private void DropItem()
    {
        Vector3 dropPosition = transform.position + new Vector3(0, 1f, 0);
        GameObject droppedItem = Instantiate(dropItemPrefab, dropPosition, Quaternion.identity);

        DroppedItem droppedItemScript = droppedItem.GetComponent<DroppedItem>();
        if (droppedItemScript != null)
        {
            droppedItemScript.itemData = dropItemData;
            droppedItemScript.itemSprite = dropItemData.itemIcon;
        }
    }

    public virtual void Attack(){}

    public void SetAnimationState(AnimationState state)
    {
        if (anim != null)
        {
            anim.SetBool("Idle", state == AnimationState.Idle);
            anim.SetBool("Move", state == AnimationState.Move);

            if (state == AnimationState.Attack)
            {
                anim.SetTrigger("Attack");
            }
            if (state == AnimationState.Damage)
            {
                anim.SetTrigger("Damage");
            }
            if (state == AnimationState.Die)
            {
                anim.SetTrigger("Die");
            }
        }
    }

    public void ResetAnimationState()
    {
        if (anim != null)
        {
            anim.SetBool("Move", false);
            anim.SetBool("Idle", false);
            anim.ResetTrigger("Attack");
            anim.ResetTrigger("Damage");
            anim.ResetTrigger("Die");
        }
    }

    public void ChasePlayer(Vector3 targetPosition)
    {
        if (agent != null)
        {
            agent.SetDestination(targetPosition);
            SetAnimationState(AnimationState.Move);
        }
    }

    public void FleeFromPlayer()
    {
        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI.enemyType != EnemyType.Runtype)
        {
            Debug.Log("싸움타입적");
            return;
        }

        if (FleePoints == null || FleePoints.Length == 0)
        {
            Debug.LogError("도망 포인트 필요");
            return;
        }
        Vector3 furthestFleePoint = FleePoints[0];
        float maxDistance = Vector3.Distance(transform.position, player.position);

        foreach (var fleePoint in FleePoints)
        {
            float distance = Vector3.Distance(fleePoint, player.position);
            if (distance > maxDistance)
            {
                furthestFleePoint = fleePoint;
                maxDistance = distance;
            }
        }

        agent.SetDestination(furthestFleePoint);
        SetAnimationState(AnimationState.Move);
    }

    public bool PlayerInAttackRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var collider in colliders)
        {
            if (collider.transform.IsChildOf(player))
            {
                return true;
            }
        }
        return false;
    }

    public void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 시야 범위 (파란색)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // 공격 범위 (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 순찰 포인트 (초록색)
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Gizmos.DrawSphere(patrolPoints[i], 0.5f);

                if (i + 1 < patrolPoints.Length)
                {
                    Gizmos.DrawLine(patrolPoints[i], patrolPoints[i + 1]);
                }
                else if (i == patrolPoints.Length - 1)
                {
                    Gizmos.DrawLine(patrolPoints[i], patrolPoints[0]);
                }
            }
        }

        if (FleePoints != null && FleePoints.Length > 0)
        {
            Gizmos.color = Color.yellow;

            for (int i = 0; i < FleePoints.Length; i++)
            {
                Gizmos.DrawSphere(FleePoints[i], 0.5f);
            }
        }
    }
}