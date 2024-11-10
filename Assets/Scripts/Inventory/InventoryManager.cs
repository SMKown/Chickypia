using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

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
    public Text FullSlotText; 
    private AudioSource audioSource;

    private void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/inventoryData.json";
        LoadInventory();
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
        audioSource.Play();

        if (!HasEmptySlot(item))
        {
            Debug.Log("인벤토리가 가득 찼습니다!");
            FullText();
            return false;
        }

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
                return false;
            }
        }

        return false;
    }

    public bool HasEmptySlot(ItemData item)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null || (itemInSlot.item == item && itemInSlot.count < maxStack))
            {
                return true;
            }
        }
        return false;
    }

    public void FullText()
    {
        if (FullSlotText != null)
        {
            FullSlotText.gameObject.SetActive(true);
            StartCoroutine(ShowEmptySlotText());
        }
    }

    private IEnumerator ShowEmptySlotText()
    {
        yield return new WaitForSeconds(3f);
        if (FullSlotText != null)
        {
            FullSlotText.gameObject.SetActive(false);
        }
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

        if (Input.GetMouseButton(1))
        {
            return;
        }

        InventorySlot oldSlot = draggedItem.parentAfterDrag.GetComponent<InventorySlot>();
        InventoryItem existingItem = newSlot.GetComponentInChildren<InventoryItem>();

        if (existingItem != null)
        {
            if (existingItem.item == draggedItem.item)
            {
                audioSource.Play();
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
                    draggedItem.transform.SetParent(oldSlot.transform);
                    draggedItem.transform.localPosition = Vector3.zero;
                    draggedItem.parentAfterDrag = oldSlot.transform;
                }
            }
            else
            {
                audioSource.Play();
                existingItem.transform.SetParent(oldSlot.transform);
                existingItem.transform.localPosition = Vector3.zero;
                existingItem.parentAfterDrag = oldSlot.transform;

                draggedItem.transform.SetParent(newSlot.transform);
                draggedItem.transform.localPosition = Vector3.zero;
                draggedItem.parentAfterDrag = newSlot.transform;
            }
        }
        else
        {
            audioSource.Play();
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

                    inventoryLog += $"Slot {data.slotIndex} : {data.itemName} {data.quantity}개\n";
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