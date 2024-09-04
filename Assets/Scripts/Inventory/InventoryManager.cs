using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject MainInvetory;
    public GameObject HotbarInventory;
    public int maxItem = 10;
    public InventorySlot[] inventoryslots;
    public GameObject inventoryItemPrefab;

    private bool isInventoryOpen = false;
    private int selectedSlot = -1;
    private int InvenSelectedSlot = -1;
    private int HotbarSelectedSlot = -1;

    private void Start()
    {
        ChangeSelectedSlot(33);
    }

    private void Update()
    {
        OnInventory();

        if (isInventoryOpen)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    InvenSelectedSlot = i;
                    ChangeSelectedSlot(InvenSelectedSlot);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    HotbarSelectedSlot = i + 33;
                    ChangeSelectedSlot(HotbarSelectedSlot);
                    break;
                }
            }
        }
    }

    public void OnInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isInventoryOpen = !isInventoryOpen;
            MainInvetory.SetActive(isInventoryOpen);
            HotbarInventory.SetActive(!isInventoryOpen);

            if (isInventoryOpen)
            {
                InvenSelectedSlot = HotbarSelectedSlot - 33;
                ChangeSelectedSlot(InvenSelectedSlot);
            }
            else
            {
                HotbarSelectedSlot = InvenSelectedSlot + 33;
                ChangeSelectedSlot(HotbarSelectedSlot);
            }
        }
    }

    private void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0 && selectedSlot < inventoryslots.Length)
        {
            inventoryslots[selectedSlot].DeSelect();
        }

        if (newValue >= 0 && newValue < inventoryslots.Length)
        {
            inventoryslots[newValue].Select();
            selectedSlot = newValue;
        }
    }


    // 아이템을 인벤토리에 추가하는 함수
    public bool AddItem(Item item)
    {
        foreach (InventorySlot slot in inventoryslots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < maxItem)
            {
                itemInSlot.count++;
                itemInSlot.ItemCount();
                return true;
            }
        }

        // 빈 슬롯을 찾아 새로운 아이템을 생성함
        foreach (InventorySlot slot in inventoryslots)
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

    // 새로운 아이템을 생성하여 지정된 슬롯에 배치하는 함수
    public InventoryItem SpawnNewItem(Item item, InventorySlot slot, int count = 1)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
        inventoryItem.count = count;
        inventoryItem.ItemCount();
        return inventoryItem;
    }

    // 현재 선택된 슬롯의 아이템을 반환하는 함수
    public Item GetSelectedItem()
    {
        if (selectedSlot >= 0 && selectedSlot < inventoryslots.Length)
        {
            InventorySlot slot = inventoryslots[selectedSlot];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null) return itemInSlot.item;
        }
        return null;
    }

    // 드래그한 아이템을 새로운 슬롯으로 이동시키는 함수
    public void OnItemSwapped(InventoryItem draggedItem, InventorySlot newSlot)
    {
        if (draggedItem == null || newSlot == null) return;

        // 새로운 슬롯에 이미 아이템이 있는 경우 처리
        InventoryItem existingItem = newSlot.GetComponentInChildren<InventoryItem>();
        if (existingItem != null)
        {
            // 같은 아이템일 경우 합침
            if (existingItem.item == draggedItem.item)
            {
                int combinedCount = existingItem.count + draggedItem.count;
                if (combinedCount <= maxItem)
                {
                    existingItem.count = combinedCount;
                    existingItem.ItemCount();
                    Destroy(draggedItem.gameObject);
                }
                else
                {
                    // 합칠 수 있는 만큼 합치고 남은 아이템은 그대로 유지
                    int remaining = combinedCount - maxItem;
                    existingItem.count = maxItem;
                    existingItem.ItemCount();
                    draggedItem.count = remaining;
                    draggedItem.ItemCount();
                    draggedItem.transform.SetParent(draggedItem.parentAfterDrag); // 드래그 시작 전 부모로 돌아감
                    draggedItem.transform.localPosition = Vector3.zero;
                }
            }
            else
            {
                // 다른 아이템일 경우 교환
                existingItem.transform.SetParent(draggedItem.parentAfterDrag);
                existingItem.transform.localPosition = Vector3.zero;
                existingItem.ItemCount();

                // 드래그한 아이템을 새로운 슬롯으로 이동
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
}
