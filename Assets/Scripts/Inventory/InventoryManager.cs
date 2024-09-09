using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    public GameObject MainInventory;
    public GameObject HotbarInventory;
    public int maxItem = 10;
    public InventorySlot[] inventorySlots; // �κ��丮 ���� �迭
    public InventorySlot[] hotbarSlots; // �ֹ� ���� �迭
    public GameObject inventoryItemPrefab;
    public GameObject itemPrefab; // ����� ������ ������
    public Transform player; // �÷��̾��� Transform

    private bool isInventoryOpen = false;
    private int selectedSlot = -1;

    private void Start()
    {
        ChangeSelectedSlot(33); // �ʱ� ���� ������ �ֹ��� ù ��° �������� ����
        MainInventory.SetActive(false); // ���� ���� �� �κ��丮�� ���� ���·� ����
        HotbarInventory.SetActive(true); // �ֹٴ� ���� ���·� ����
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

        // �κ��丮�� �ִ� ��� �����۰� �� ���� ���
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
                ChangeSelectedSlot(selectedSlot - 33); // �ֹٿ��� �κ��丮�� ��ȯ ��
            }
            else
            {
                ChangeSelectedSlot(selectedSlot + 33); // �κ��丮���� �ֹٷ� ��ȯ ��
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

    // �������� �κ��丮�� �߰��ϴ� �Լ�
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

        // �� ������ ã�� ���ο� �������� ������
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

    // ���ο� �������� �����Ͽ� ������ ���Կ� ��ġ�ϴ� �Լ�
    public InventoryItem SpawnNewItem(ItemData item, InventorySlot slot, int count = 1)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
        inventoryItem.count = count;
        inventoryItem.ItemCount();
        return inventoryItem;
    }

    // ���� ���õ� ������ �������� ��ȯ�ϴ� �Լ�
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

    // �巡���� �������� ���ο� �������� �̵���Ű�� �Լ�
    public void OnItemSwapped(InventoryItem draggedItem, InventorySlot newSlot)
    {
        if (draggedItem == null || newSlot == null) return;

        // ���ο� ���Կ� �̹� �������� �ִ� ��� ó��
        InventoryItem existingItem = newSlot.GetComponentInChildren<InventoryItem>();
        if (existingItem != null)
        {
            // ���� �������� ��� ��ħ
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
                    // ��ĥ �� �ִ� ��ŭ ��ġ�� ���� �������� �״�� ����
                    int remaining = combinedCount - maxItem;
                    existingItem.count = maxItem;
                    existingItem.ItemCount();
                    draggedItem.count = remaining;
                    draggedItem.ItemCount();
                    draggedItem.transform.SetParent(draggedItem.parentAfterDrag); // �巡�� ���� �� �θ�� ���ư�
                    draggedItem.transform.localPosition = Vector3.zero;
                }
            }
            else
            {
                // �ٸ� �������� ��� ��ȯ
                existingItem.transform.SetParent(draggedItem.parentAfterDrag);
                existingItem.transform.localPosition = Vector3.zero;
                existingItem.ItemCount();

                // �巡���� �������� ���ο� �������� �̵�
                draggedItem.transform.SetParent(newSlot.transform);
                draggedItem.transform.localPosition = Vector3.zero;
                draggedItem.parentAfterDrag = newSlot.transform;
            }
        }
        else
        {
            // ������ ��������� �巡���� �������� �̵�
            draggedItem.transform.SetParent(newSlot.transform);
            draggedItem.transform.localPosition = Vector3.zero;
            draggedItem.parentAfterDrag = newSlot.transform;
        }
    }

    // �κ��丮 �ȿ� �ִ� ������ ������ ����Ʈ
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

    // �������� ������ �Լ�
    public void DropItem(ItemData item)
    {
        Vector3 dropPosition = player.transform.position + player.transform.forward;
        GameObject droppedItem = Instantiate(item.itemModel, dropPosition, Quaternion.identity);
    }

    // ���Կ��� �������� �����ϴ� �Լ�
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
