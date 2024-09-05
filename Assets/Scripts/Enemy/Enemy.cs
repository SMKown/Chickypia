using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    MeleeType1,
    MeleeType2,
    MeleeType3,
    RangedType1,
    RangedType2,
    RangedType3
}

public abstract class Enemy : MonoBehaviour
{
    public int health;
    public int attackPower;
    public Animator animator;
    public EnemyType enemyType;
    protected Transform player;

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Damage");
        }
    }

    protected virtual void Die()
    {
        animator.SetTrigger("Die");
        Destroy(gameObject, 2f);

        DropItem();
    }

    void DropItem()
    {
        // 아이템 드롭 로직 추가
        Debug.Log("아이템 드롭");
    }

    protected abstract void Attack();
}