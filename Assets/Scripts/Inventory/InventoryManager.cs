using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    public GameObject MainInventory;
    public GameObject HotbarInventory;
    public int maxItem = 10;
    public InventorySlot[] inventorySlots; // 인벤토리 슬롯 배열
    public InventorySlot[] hotbarSlots; // 핫바 슬롯 배열
    public GameObject inventoryItemPrefab;
    public GameObject itemPrefab; // 드롭할 아이템 프리팹
    public Transform player; // 플레이어의 Transform

    private bool isInventoryOpen = false;
    private int selectedSlot = -1;

    private void Start()
    {
        ChangeSelectedSlot(33); // 초기 선택 슬롯을 핫바의 첫 번째 슬롯으로 설정
        MainInventory.SetActive(false); // 게임 시작 시 인벤토리를 꺼진 상태로 설정
        HotbarInventory.SetActive(true); // 핫바는 켜진 상태로 설정
    }

    private void Update()
    {
        OnInventory();

        if (Input.GetKeyDown(KeyCode.G) && !isInventoryOpen)
        {
            ItemData selectedItem = GetSelectedItem();
            if (selectedItem != null)
            {
                DropItem(selectedItem);
                RemoveItemFromSlot(selectedSlot);
            }
        }

        // 인벤토리에 있는 모든 아이템과 그 개수 출력
        if (Input.GetKeyDown(KeyCode.T))
        {
            List<(ItemData item, int count)> items = GetInventoryItems();
            foreach (var itemData in items)
            {
                Debug.Log($"Item: {itemData.item.name}, Count: {itemData.count}");
            }
        }

        for (int i = 0; i < 6; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ChangeSelectedSlot(i + (isInventoryOpen ? 0 : 33));
                break;
            }
        }
    }

    public void OnInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isInventoryOpen = !isInventoryOpen;
            MainInventory.SetActive(isInventoryOpen);
            HotbarInventory.SetActive(!isInventoryOpen);

            if (isInventoryOpen)
            {
                ChangeSelectedSlot(selectedSlot - 33); // 핫바에서 인벤토리로 전환 시
            }
            else
            {
                ChangeSelectedSlot(selectedSlot + 33); // 인벤토리에서 핫바로 전환 시
            }
        }
    }

    private void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0 && selectedSlot < inventorySlots.Length)
        {
            inventorySlots[selectedSlot].DeSelect();
        }
        else if (selectedSlot >= 33 && selectedSlot < 33 + hotbarSlots.Length)
        {
            hotbarSlots[selectedSlot - 33].DeSelect();
        }

        if (newValue >= 0 && newValue < inventorySlots.Length)
        {
            inventorySlots[newValue].Select();
            selectedSlot = newValue;
        }
        else if (newValue >= 33 && newValue < 33 + hotbarSlots.Length)
        {
            hotbarSlots[newValue - 33].Select();
            selectedSlot = newValue;
        }
    }

    // 아이템을 인벤토리에 추가하는 함수
    public bool AddItem(ItemData item)
    {
        foreach (InventorySlot slot in inventorySlots)
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

    // 새로운 아이템을 생성하여 지정된 슬롯에 배치하는 함수
    public InventoryItem SpawnNewItem(ItemData item, InventorySlot slot, int count = 1)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
        inventoryItem.count = count;
        inventoryItem.ItemCount();
        return inventoryItem;
    }

    // 현재 선택된 슬롯의 아이템을 반환하는 함수
    public ItemData GetSelectedItem()
    {
        if (selectedSlot >= 0 && selectedSlot < inventorySlots.Length)
        {
            InventorySlot slot = inventorySlots[selectedSlot];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null) return itemInSlot.item;
        }
        else if (selectedSlot >= 33 && selectedSlot < 33 + hotbarSlots.Length)
        {
            InventorySlot slot = hotbarSlots[selectedSlot - 33];
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

    // 인벤토리 안에 있는 아이템 데이터 리스트
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

        foreach (InventorySlot slot in hotbarSlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null)
            {
                items.Add((itemInSlot.item, itemInSlot.count));
            }
        }

        return items;
    }

    // 아이템을 버리는 함수
    public void DropItem(ItemData item)
    {
        Vector3 dropPosition = player.transform.position + player.transform.forward;
        GameObject droppedItem = Instantiate(item.itemModel, dropPosition, Quaternion.identity);
    }

    // 슬롯에서 아이템을 제거하는 함수
    public void RemoveItemFromSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inventorySlots.Length)
        {
            InventorySlot slot = inventorySlots[slotIndex];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null)
            {
                Destroy(itemInSlot.gameObject);
            }
        }
        else if (slotIndex >= 33 && slotIndex < 33 + hotbarSlots.Length)
        {
            InventorySlot slot = hotbarSlots[slotIndex - 33];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null)
            {
                Destroy(itemInSlot.gameObject);
            }
        }
    }
}
