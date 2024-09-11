using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public FoodEffect foodEffect;

    public int maxStack = 10;
    public GameObject MainInventory;
    public GameObject HotBar;
    public InventorySlot[] inventorySlots;

    public GameObject inventoryItemPrefab;
    public Transform player;

    private bool isInventoryOpen = false;
    int selectedSlot = -1;
    public ItemData selectedItem;

    private void Start()
    {
        ChangeSelectedSlot(0);
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
            MainInventory.SetActive(isInventoryOpen);
            HotBar.SetActive(!isInventoryOpen);
            if (isInventoryOpen)
            {
                SyncHotBarToInventory();
            }
            else
            {
                SyncInventoryToHotBar();
            }
        }
    }

    private void SyncHotBarToInventory()
    {
        for (int i = 0; i < 5; i++)
        {
            InventorySlot hotBarSlot = HotBar.transform.GetChild(i).GetComponent<InventorySlot>();
            InventorySlot inventorySlot = inventorySlots[i + 5];

            InventoryItem hotBarItem = hotBarSlot.GetComponentInChildren<InventoryItem>();
            InventoryItem inventoryItem = inventorySlot.GetComponentInChildren<InventoryItem>();

            if (hotBarItem != null)
            {
                if (inventoryItem != null)
                {
                    // 아이템을 인벤토리 슬롯에 복사
                    inventoryItem.item = hotBarItem.item;
                    inventoryItem.count = hotBarItem.count;
                    inventoryItem.ItemCount();
                }
                else
                {
                    // 새로운 아이템을 인벤토리 슬롯에 생성
                    SpawnNewItem(hotBarItem.item, inventorySlot, hotBarItem.count);
                }
            }
            else if (inventoryItem != null)
            {
                // 핫바 슬롯에 아이템이 없으면 삭제
                Destroy(inventoryItem.gameObject);
            }
        }
    }

    private void SyncInventoryToHotBar()
    {
        for (int i = 0; i < 5; i++)
        {
            InventorySlot hotBarSlot = HotBar.transform.GetChild(i).GetComponent<InventorySlot>();
            InventorySlot inventorySlot = inventorySlots[i + 5];

            InventoryItem inventoryItem = inventorySlot.GetComponentInChildren<InventoryItem>();
            InventoryItem hotBarItem = hotBarSlot.GetComponentInChildren<InventoryItem>();

            if (inventoryItem != null)
            {
                if (hotBarItem != null)
                {
                    // 아이템을 핫바 슬롯에 복사
                    hotBarItem.item = inventoryItem.item;
                    hotBarItem.count = inventoryItem.count;
                    hotBarItem.ItemCount();
                }
                else
                {
                    // 새로운 아이템을 핫바 슬롯에 생성
                    SpawnNewItem(inventoryItem.item, hotBarSlot, inventoryItem.count);
                }
            }
            else if (hotBarItem != null)
            {
                // 인벤토리 슬롯에 아이템이 없으면 삭제
                Destroy(hotBarItem.gameObject);
            }
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
                    int remaining = combinedCount - maxStack;
                    existingItem.count = maxStack;
                    existingItem.ItemCount();
                    draggedItem.count = remaining;
                    draggedItem.ItemCount();
                    draggedItem.transform.SetParent(draggedItem.parentAfterDrag);
                    draggedItem.transform.localPosition = Vector3.zero;
                }
            }
            else //다른 아이템일 경우 교환
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
            // 슬롯이 비어있으면 드래그한 아이템을 이동
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
