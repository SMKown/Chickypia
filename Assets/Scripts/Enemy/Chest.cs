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

        if(itemData.isCollected)
        {
            CleardChest();
        }
    }

    private void Update()
    {
        if (SpawnChest())
        {
            animator.SetTrigger("ChestSpawn");
        }
        else if(itemData.isCollected)
        {
            animator.SetTrigger("ChestSpawn");
        }
    }

    private bool SpawnChest()
    {
        bool enemiesDead = EnemyAllDead();
        bool questInProgress = QuestManager.Instance.GetQuestData(5)?.status == QuestStatus.InProgress;
        return enemiesDead && questInProgress;
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

    public void CleardChest()
    {
        UIInteraction.Instance.interactableObj.name = "Village";
        UIInteraction.Instance.interactableObj.tag = "MoveScene";
        animator.SetTrigger("ChestClear");
    }
}
