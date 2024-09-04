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

public abstract class EnemyBase : MonoBehaviour
{
    public int health;
    public int attackPower;
    public Animator animator;
    public EnemyType enemyType;

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("hit");
        }
    }

    protected virtual void Die()
    {
        animator.SetTrigger("die");
        Destroy(gameObject, 2f);

        DropItem(); // ������ ���
    }

    // ���� �׾��� �� �������� ����ϴ� �޼���
    void DropItem()
    {
        
    }
    protected abstract void Attack();
}