using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventoryslots;

    public void AddItem(Item item)
    {
        for (int i = 0; i < inventoryslots.Length; i++)
        {
            InventorySlot slot = inventoryslots[i];
            InventoryItem itemInSlot = slot.GetComponent<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawNewItem(item, slot);
                return;
            }
        }
    }

    void SpawNewItem(Item item, InventorySlot slot)
    {

    }
}
