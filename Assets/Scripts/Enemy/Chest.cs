using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator animator;
    public GameObject Star;
    public PlayerMovement playerMovement;
    public Transform chestCamTransform;
    public ItemData itemData;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (EnemyAllDead())
        {
            animator.SetTrigger("ChestSpawn");
        }
    }

    private bool EnemyAllDead()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Length == 0;
    }                                                                                   

    public void OpenChest()
    {
        animator.SetTrigger("ChestOpen");
    }

    public void SpawnStar()
    {
        Star.SetActive(true);
    }
}
