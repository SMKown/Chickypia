using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int maxItem = 10;
    public InventorySlot[] inventoryslots;
    public GameObject inventoryItemPrefab;
    public GameObject Hotbar;

    int selectedSlot = -1;

    private void Start()
    {
        ChangeSelectedSlot(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(Hotbar == true)
                ChangeSelectedSlot(0);  
            else
                ChangeSelectedSlot(33);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (Hotbar == true)
                ChangeSelectedSlot(1);
            else
                ChangeSelectedSlot(34);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeSelectedSlot(2);
            ChangeSelectedSlot(35);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeSelectedSlot(3);
            ChangeSelectedSlot(36);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeSelectedSlot(4);
            ChangeSelectedSlot(37);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ChangeSelectedSlot(5);
            ChangeSelectedSlot(38);
        }
    }

    void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            inventoryslots[selectedSlot].DeSelect();
        }
        inventoryslots[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddItem(Item item)
    {
        for (int i = 0; i < inventoryslots.Length; i++)
        {
            InventorySlot slot = inventoryslots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < maxItem)
            {
                itemInSlot.count++;
                itemInSlot.ItemCount();
                return true;
            }
        }

        for (int i = 0; i < inventoryslots.Length; i++)
        {
            InventorySlot slot = inventoryslots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawNewItem(item, slot);
                return true;
            }
        }
        return false;
    }

    void SpawNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGet = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGet.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
    }

    public Item GetSelectedItem()
    {
        InventorySlot slot = inventoryslots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            return itemInSlot.item;
        }
        return null;
    }
}
