using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image slotBackgroundImage;

    private void Start()
    {
        UpdateSlotBackground();
    }
    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem draggedItem = eventData.pointerDrag?.GetComponent<InventoryItem>();
        if (draggedItem == null) return;

        draggedItem.tooltipUI = ItemToolTipUI.Instance;

        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null)
        {
            inventoryManager.OnItemSwapped(draggedItem, this);
            UpdateSlotBackground();
        }
    }
    public void UpdateSlotBackground()
    {
        InventoryItem itemInSlot = GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            slotBackgroundImage.color = Color.clear;
        }
        else
        {
            slotBackgroundImage.color = new Color(1, 1, 1, 1);
        }
        Debug.Log("UpdateSlotBackground called. Slot color: " + slotBackgroundImage.color);
    }
}
