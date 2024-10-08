using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    public Image image;
    public Text countText;
    public ItemData item;
    public ItemToolTipUI tooltipUI;

    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    private bool isDragging = false;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        //InitialiseItem(item);
        if (tooltipUI == null)
        {
            tooltipUI = ItemToolTipUI.Instance;
        }
    }

    public void InitialiseItem(ItemData newItem, ItemToolTipUI toolTipUIInstance)
    {
        item = newItem;
        image.sprite = newItem.itemIcon;
        tooltipUI = toolTipUIInstance;
        ItemCount();
    }

    public void ItemCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        countText.raycastTarget = false;

        parentAfterDrag = transform.parent;
        InventorySlot currentSlot = parentAfterDrag.GetComponent<InventorySlot>();
        if (currentSlot != null)
        {
            transform.SetParent(transform.root);
            currentSlot.UpdateSlotBackground();
        }

        //transform.SetParent(transform.root);
        tooltipUI.HideToolTip();
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                rectTransform.position = globalMousePos;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        countText.raycastTarget = true;

        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;

        InventorySlot targetSlot = parentAfterDrag.GetComponent<InventorySlot>();
        if (targetSlot != null)
        {
            transform.SetParent(parentAfterDrag);
            transform.localPosition = Vector3.zero;
            targetSlot.UpdateSlotBackground();
        }

        isDragging = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipUI == null || isDragging)
            return;

        SetTooltipPosition();

        tooltipUI.ShowToolTip(item.itemDesc, item.itemName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipUI == null || isDragging)
            return;

        tooltipUI.HideToolTip();
    }


    private void SetTooltipPosition()
    {
        RectTransform slotRectTransform = GetComponent<RectTransform>();
        RectTransform tooltipRectTransform = tooltipUI.GetComponent<RectTransform>();
        RectTransform canvasRectTransform = tooltipUI.transform.parent.GetComponent<RectTransform>();

        Vector3 slotPosition = slotRectTransform.position;
        Vector3 offset = new Vector3(slotRectTransform.rect.width / 4, -slotRectTransform.rect.height / 4, 0);

        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            slotPosition + offset,
            null,
            out anchoredPosition
            );
        tooltipRectTransform.anchoredPosition = anchoredPosition;
    }

    private void Update()
    {
        if (isDragging && Input.GetMouseButtonDown(1))
        {
            PlaceOneItem();
        }
    }

    private void PlaceOneItem()
    {
        if (count > 1)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();
                if (slot != null)
                {
                    InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                    if (itemInSlot == null)
                    {
                        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
                        InventoryItem newItem = inventoryManager.SpawnNewItem(item, slot, 1);

                        newItem.image.raycastTarget = true;
                        newItem.countText.raycastTarget = true;
                        count -= 1;
                        ItemCount();
                        slot.UpdateSlotBackground();

                        return;
                    }
                    else if (itemInSlot.item == item)
                    {
                        itemInSlot.count += 1;
                        itemInSlot.ItemCount();
                        count -= 1;
                        ItemCount();
                        slot.UpdateSlotBackground();
                        return;
                    }
                    return;
                }
            }
        }
    }

    public ItemData GetItemData()
    {
        return item;
    }
}
