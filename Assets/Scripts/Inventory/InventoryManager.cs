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


    // �������� �κ��丮�� �߰��ϴ� �Լ�
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

        // �� ������ ã�� ���ο� �������� ������
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

    // ���ο� �������� �����Ͽ� ������ ���Կ� ��ġ�ϴ� �Լ�
    public InventoryItem SpawnNewItem(Item item, InventorySlot slot, int count = 1)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
        inventoryItem.count = count;
        inventoryItem.ItemCount();
        return inventoryItem;
    }

    // ���� ���õ� ������ �������� ��ȯ�ϴ� �Լ�
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
}
