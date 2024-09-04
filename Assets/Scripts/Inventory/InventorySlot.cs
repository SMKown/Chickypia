using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image image;
    public Color selectedColor, notSelectedColor;

    private void Awake()
    {
        DeSelect();
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void DeSelect()
    {
        image.color = notSelectedColor;
    }

    // 드래그된 아이템이 이 슬롯에 드롭되었을 때 호출되는 함수
    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem draggedItem = eventData.pointerDrag?.GetComponent<InventoryItem>();
        if (draggedItem == null) return;

        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null)
        {
            inventoryManager.OnItemSwapped(draggedItem, this);
        }
    }
}
