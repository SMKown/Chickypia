using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI")]
    public Image image;
    public Text countText;
    public ItemData item;
    public ItemToolTipUI tooltipUI;
    public InventoryManager inventoryManager;

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
        if (parentAfterDrag == null)
        {
            parentAfterDrag = transform.parent;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ShowCheckUI();
        }
    }

    private void ShowCheckUI()
    {
        InvenCompenUI invenUI = FindObjectOfType<InvenCompenUI>();
        if (invenUI != null)
        {
            invenUI.SetCurrentItem(this);
            invenUI.ShowItemOptions(item.itemType);
        }
    }

    public void UseItemButton()
    {
        StartCoroutine(UseItem());
    }

    public void DiscardItemButton()
    {
        if (count > 1)
        {
            count -= 1;
            if (count <= 0)
            {
                InventorySlot parentSlot = transform.parent?.GetComponent<InventorySlot>();
                if (parentSlot != null)
                {
                    parentSlot.slotBackgroundImage.color = new Color(1, 1, 1, 1);
                    parentSlot.slotBackgroundImage.SetAllDirty();
                }
                if (tooltipUI != null)
                {
                    tooltipUI.HideToolTip();
                }
                Destroy(gameObject);
            }
            else
            {
                ItemCount();
            }
        }
        else
        {
            InventorySlot parentSlot = transform.parent?.GetComponent<InventorySlot>();
            if (parentSlot != null)
            {
                parentSlot.slotBackgroundImage.color = new Color(1, 1, 1, 1);
                parentSlot.slotBackgroundImage.SetAllDirty();
            }
            if (tooltipUI != null)
            {
                tooltipUI.HideToolTip();
            }
            Destroy(gameObject);
        }
    }

    IEnumerator UseItem()
    {
        if (PlayerStats.Instance.useItem == false && item.itemType == ItemType.Food)
        {
            PlayerStats.Instance.useItem = true;
            FoodEffect.instance.GetFood(this);

            count -= 1;
            if (count <= 0)
            {
                InventorySlot parentSlot = transform.parent?.GetComponent<InventorySlot>();
                if (parentSlot != null)
                {
                    parentSlot.slotBackgroundImage.color = new Color(1, 1, 1, 1);
                    parentSlot.slotBackgroundImage.SetAllDirty();
                }
                if (tooltipUI != null)
                {
                    tooltipUI.HideToolTip();
                }
                PlayerStats.Instance.useItem = false;
                Destroy(gameObject);
            }
            else
            {
                ItemCount();
            }    
        }
        yield return new WaitForSeconds(0.1f);
        PlayerStats.Instance.useItem = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            return;
        }

        image.raycastTarget = false;
        countText.raycastTarget = false;

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
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (isDragging)
        {
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    rectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                rectTransform.position = globalMousePos;
            }
        }
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            //transform.SetParent(parentAfterDrag);
            //transform.localPosition = Vector3.zero;
            return;
        }

        image.raycastTarget = true;
        countText.raycastTarget = true;

        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;

        InventorySlot targetSlot = parentAfterDrag.GetComponent<InventorySlot>();
        if (targetSlot != null)
        {
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
        Vector3 offset = new Vector3(slotRectTransform.rect.width / 3, -slotRectTransform.rect.height / 3, 0);

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

