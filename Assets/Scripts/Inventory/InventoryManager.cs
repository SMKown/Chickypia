using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public FoodEffect foodEffect;

    public int maxStack = 10;
    public GameObject MainInventory;
    public InventorySlot[] inventorySlots;

    public GameObject inventoryItemPrefab;
    public Transform player;

    private bool isInventoryOpen = false;
    int selectedSlot = -1;
    public ItemData selectedItem;

    private void Start()
    {
        MainInventory.SetActive(false);
    }

    private void Update()
    {
        OnInventory();

        if (Input.GetKeyDown(KeyCode.T))
        {
            List<(ItemData item, int count)> items = GetInventoryItems();
            foreach (var itemData in items)
            {
                Debug.Log($"Item: {itemData.item.name}, Count: {itemData.count}");
            }
        }
    }

    public void OnInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isInventoryOpen = !isInventoryOpen;
            MainInventory.SetActive(isInventoryOpen);
        }
    }

    public bool AddItem(ItemData item)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < maxStack)
            {
                itemInSlot.count++;
                itemInSlot.ItemCount();
                return true;
            }
        }

        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }
        return false;
    }

    public InventoryItem SpawnNewItem(ItemData item, InventorySlot slot, int count = 1)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem invItem = newItemGo.GetComponent<InventoryItem>();
        invItem.InitialiseItem(item, ItemToolTipUI.Instance);
        invItem.count = count;
        invItem.ItemCount();
        return invItem;
    }

    public ItemData GetSelectedItem()
    {
        if (selectedSlot >= 0 && selectedSlot < inventorySlots.Length)
        {
            InventoryItem itemInSlot = inventorySlots[selectedSlot].GetComponentInChildren<InventoryItem>();
            return itemInSlot != null ? itemInSlot.item : null;
        }
        return null;
    }

    public void OnItemSwapped(InventoryItem draggedItem, InventorySlot newSlot)
    {
        if (draggedItem == null || newSlot == null) return;

        InventoryItem existingItem = newSlot.GetComponentInChildren<InventoryItem>();
        if (existingItem != null)
        {
            if (existingItem.item == draggedItem.item)
            {
                int combinedCount = existingItem.count + draggedItem.count;
                if (combinedCount <= maxStack)
                {
                    existingItem.count = combinedCount;
                    existingItem.ItemCount();
                    Destroy(draggedItem.gameObject);
                }
                else
                {
                    int remaining = combinedCount - maxStack;
                    existingItem.count = maxStack;
                    existingItem.ItemCount();
                    draggedItem.count = remaining;
                    draggedItem.ItemCount();
                    draggedItem.transform.SetParent(draggedItem.parentAfterDrag);
                    draggedItem.transform.localPosition = Vector3.zero;
                }
            }
            else
            {
                existingItem.transform.SetParent(draggedItem.parentAfterDrag);
                existingItem.transform.localPosition = Vector3.zero;
                existingItem.ItemCount();

                draggedItem.transform.SetParent(newSlot.transform);
                draggedItem.transform.localPosition = Vector3.zero;
                draggedItem.parentAfterDrag = newSlot.transform;
            }
        }
        else
        {
            draggedItem.transform.SetParent(newSlot.transform);
            draggedItem.transform.localPosition = Vector3.zero;
            draggedItem.parentAfterDrag = newSlot.transform;
        }
    }

    public List<(ItemData item, int count)> GetInventoryItems()
    {
        List<(ItemData item, int count)> items = new List<(ItemData item, int count)>();

        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null)
            {
                items.Add((itemInSlot.item, itemInSlot.count));
            }
        }
        return items;
    }
}
