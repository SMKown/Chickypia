using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public FoodEffect foodEffect;

    public GameObject MainInventory;
    public GameObject HotbarInventory;

    private const int InventorySlotCount = 30;
    private const int HotbarSlotCount = 5;

    public int maxItem = 10;
    public InventorySlot[] inventorySlots;
    public InventorySlot[] hotbarSlots;
    public Transform player;

    private bool isInventoryOpen = false;
    private int selectedSlot = -1;
    public ItemData selectedItem;

    private void Start()
    {
        ChangeSelectedSlot(0);
        MainInventory.SetActive(false);
        HotbarInventory.SetActive(true);
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

        for (int i = 0; i < HotbarSlotCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ChangeSelectedSlot(i);
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

            SynInvenSlotHotbar();
        }
    }

    private void SynInvenSlotHotbar()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            InventorySlot hotbarSlot = hotbarSlots[i];
            InventorySlot inventorySlot = inventorySlots[i];

            if (hotbarSlot.item != null)
            {
                if (inventorySlot.item == null)
                {
                    SpawnNewItem(hotbarSlot.item, inventorySlot, hotbarSlot.count);
                }
                else
                {
                    inventorySlot.InitialiseItem(hotbarSlot.item);
                    inventorySlot.count = hotbarSlot.count;
                    inventorySlot.ItemCount();
                }
            }
            else
            {
                if (inventorySlot.item != null)
                {
                    SpawnNewItem(inventorySlot.item, hotbarSlot, inventorySlot.count);
                    inventorySlot.InitialiseItem(null);
                }
            }
        }
    }

    private void ChangeSelectedSlot(int newValue)
    {
        if (newValue >= 0 && newValue < HotbarSlotCount)
        {
            if (selectedSlot >= 0 && selectedSlot < HotbarSlotCount)
            {
                hotbarSlots[selectedSlot].DeSelect();
            }
            hotbarSlots[newValue].Select();
            selectedSlot = newValue;
        }
    }

    public bool AddItem(ItemData item)
    {
        if (TryAddItemToSlot(hotbarSlots, item)) return true;
        if (TryAddItemToSlot(inventorySlots, item)) return true;
        return TryAddItemToEmptySlot(inventorySlots, item);
    }

    private bool TryAddItemToSlot(InventorySlot[] slots, ItemData item)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == item && slot.count < maxItem)
            {
                slot.count++;
                slot.ItemCount();
                return true;
            }
        }
        return false;
    }

    private bool TryAddItemToEmptySlot(InventorySlot[] slots, ItemData item)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }
        return false;
    }

    public InventorySlot SpawnNewItem(ItemData item, InventorySlot slot, int count = 1)
    {
        slot.InitialiseItem(item);
        slot.count = count;
        slot.ItemCount();
        return slot;
    }

    public List<(ItemData item, int count)> GetInventoryItems()
    {
        List<(ItemData item, int count)> items = new List<(ItemData item, int count)>();

        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.item != null)
            {
                items.Add((slot.item, slot.count));
            }
        }

        return items;
    }

    public void OnItemSwapped(InventorySlot draggedSlot, InventorySlot targetSlot)
    {
        if (targetSlot.item != null && targetSlot.item == draggedSlot.item)
        {
            targetSlot.count += draggedSlot.count;
            targetSlot.ItemCount();
        }
        else
        {
            ItemData tempItem = targetSlot.item;
            int tempCount = targetSlot.count;

            targetSlot.InitialiseItem(draggedSlot.item);
            targetSlot.count = draggedSlot.count;
            targetSlot.ItemCount();

            draggedSlot.InitialiseItem(tempItem);
            draggedSlot.count = tempCount;
            draggedSlot.ItemCount();
        }
    }
}
