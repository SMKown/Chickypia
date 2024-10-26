using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Build.Content;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public FoodEffect foodEffect;
    public int maxStack = 10;
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    public Transform player;
    public ItemDatabase itemDatabase;
    public CookingSystem cookingSystem;
    private string saveFilePath;
    public CompendiumManager compendiumManager;

    private void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/inventoryData.json";
        LoadInventory();
    }
    private void Start()
    {
    }

    private void Update()
    {
        if (cookingSystem != null && cookingSystem.IsCookingActive())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            List<(ItemData item, int count)> items = GetInventoryItems();
            foreach (var itemData in items)
            {
                Debug.Log($"Item: {itemData.item.name}, Count: {itemData.count}");
            }
        }
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < maxStack)
            {
                int addableAmount = Mathf.Min(amount, maxStack - itemInSlot.count);
                itemInSlot.count += addableAmount;
                itemInSlot.ItemCount();
                amount -= addableAmount;

                if (!item.isCollected)
                {
                    item.isCollected = true;
                    compendiumManager.CollectItem(item);
                }
                slot.UpdateSlotBackground();

                if (amount <= 0)
                    return true;
            }
        }

        while (amount > 0)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                if (itemInSlot == null)
                {
                    int addableAmount = Mathf.Min(amount, maxStack);
                    InventoryItem newItem = SpawnNewItem(item, slot, addableAmount);
                    amount -= addableAmount;

                    if (!item.isCollected)
                    {
                        item.isCollected = true;
                        compendiumManager.CollectItem(item);
                    }
                    slot.UpdateSlotBackground();

                    if (amount <= 0)
                        return true;
                }
            }

            if (amount > 0)
            {
                Debug.LogWarning("Inventory is full, unable to add remaining items.");
                return false;
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

    public void OnItemSwapped(InventoryItem draggedItem, InventorySlot newSlot)
    {
        if (draggedItem == null || newSlot == null) return;
        InventorySlot oldSlot = draggedItem.parentAfterDrag.GetComponent<InventorySlot>();
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
        newSlot.UpdateSlotBackground(); 
        if (oldSlot != null)
        {
            oldSlot.UpdateSlotBackground();
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

    public ItemData GetItemDataByName(string itemName)
    {
        return itemDatabase.GetItemDataByName(itemName);
    }

    public void ResetInventory()
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            foreach (Transform child in slot.transform)
            {
                DestroyImmediate(child.gameObject);
            }
            slot.UpdateSlotBackground();
        }

        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        Debug.Log("Inventory has been reset.");
    }


    public void SaveInventory()
    {
        List<InventoryItemData> inventoryDataList = new List<InventoryItemData>();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null)
            {
                InventoryItemData data = new InventoryItemData
                {
                    itemName = itemInSlot.item.itemName,
                    quantity = itemInSlot.count,
                    slotIndex = i
                };
                inventoryDataList.Add(data);
            }
        }
        if (inventoryDataList.Count > 0)
        {
            string json = JsonUtility.ToJson(new InventoryDataWrapper(inventoryDataList), true);
            System.IO.File.WriteAllText(saveFilePath, json);
        }
    }

    public void LoadInventory()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            InventoryDataWrapper dataWrapper = JsonUtility.FromJson<InventoryDataWrapper>(json);
            List<InventoryItemData> inventoryDataList = dataWrapper.items;
            string inventoryLog = "Loaded Inventory Slots:\n";

            foreach (InventorySlot slot in inventorySlots)
            {
                foreach (Transform child in slot.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            foreach (InventoryItemData data in inventoryDataList)
            {
                ItemData itemData = GetItemDataByName(data.itemName);
                if (itemData != null)
                {
                    InventorySlot slot = inventorySlots[data.slotIndex];
                    InventoryItem newItem = SpawnNewItem(itemData, slot, data.quantity);

                    inventoryLog += $"Slot {data.slotIndex} : {data.itemName} {data.quantity}°³\n";
                }
            }
        }
    }
}

[System.Serializable]
public class InventoryItemData
{
    public string itemName;
    public int quantity;
    public int slotIndex;
}

[System.Serializable]
public class InventoryDataWrapper
{
    public List<InventoryItemData> items;

    public InventoryDataWrapper(List<InventoryItemData> items)
    {
        this.items = items;
    }
}