using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public FoodEffect foodEffect;

    public int maxStack = 10;
    public GameObject MainInventiroy;
    public InventorySlot[] inventorySlots;

    public GameObject inventoryItemPrefab;
    public Transform player;

    private bool isInventoryOpen = false;
    int selectedSlot = -1;
    public ItemData selectedItem;

    private void Start()
    {
        ChangeSelectedSlot(0);
        MainInventiroy.SetActive(false);
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

        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < 6)
            {
                ChangeSelectedSlot(number - 1);
            }
        }
    }

    public void OnInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isInventoryOpen = !isInventoryOpen;
            MainInventiroy.SetActive(isInventoryOpen);
        }
    }
    void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].DeSelect();
        }
        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddItem(ItemData item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < maxStack)
            {
                itemInSlot.count++;
                itemInSlot.ItemCount();
                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot, 1);
                return true;
            }
        }
        return false;
    }

    public InventoryItem SpawnNewItem(ItemData item, InventorySlot slot, int count)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem invItem = newItemGo.GetComponent<InventoryItem>();
        invItem.InitialiseItem(item);
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

        // 새로운 슬롯에 이미 아이템이 있는 경우 처리
        InventoryItem existingItem = newSlot.GetComponentInChildren<InventoryItem>();
        if (existingItem != null)
        {

            if (existingItem.item == draggedItem.item)
            {
                // 같은 아이템일 경우 병합
                int combinedCount = existingItem.count + draggedItem.count;
                if (combinedCount <= maxStack)
                {
                    existingItem.count = combinedCount;
                    existingItem.ItemCount();
                    Destroy(draggedItem.gameObject);
                }
                else
                {
                    // 최대 스택 수에 맞게 아이템 분할
                    existingItem.count = maxStack;
                    existingItem.ItemCount();
                    draggedItem.count = combinedCount - maxStack;
                    draggedItem.ItemCount();
                }
            }
            else //다른 아이템일 경우 교환
            {
                SwapItems(draggedItem, existingItem);
            }
        }
        else
        {
            // 슬롯이 비어있으면 드래그한 아이템을 이동
            draggedItem.transform.SetParent(newSlot.transform);
            draggedItem.transform.localPosition = Vector3.zero;
            draggedItem.parentAfterDrag = newSlot.transform;
        }
    }

    private void SwapItems(InventoryItem draggedItem, InventoryItem existingItem)
    {
        Transform draggedItemParent = draggedItem.transform.parent;
        Transform existingItemParent = existingItem.transform.parent;

        draggedItem.transform.SetParent(existingItemParent);
        draggedItem.transform.localPosition = Vector3.zero;

        existingItem.transform.SetParent(draggedItemParent);
        existingItem.transform.localPosition = Vector3.zero;

        draggedItem.parentAfterDrag = draggedItem.transform.parent;
        existingItem.parentAfterDrag = existingItem.transform.parent;
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
