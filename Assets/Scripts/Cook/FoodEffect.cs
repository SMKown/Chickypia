using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodEffect : MonoBehaviour
{
    public PlayerMovement playerMovement; 
    private ItemData itemData;

    public void GetFood(InventoryItem selectedItem)
    {
        itemData = selectedItem.item;

        StartCoroutine(Eat(itemData));
    }

    IEnumerator Eat(ItemData itemData)
    {
        yield return new WaitForSeconds(5f); // �ִϸ��̼����� ���� ����

        //playerAbility.
    }
}
