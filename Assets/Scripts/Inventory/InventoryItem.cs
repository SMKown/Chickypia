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
            Debug.Log("Right click detected on item: " + item.itemName);
            UseItem();
        }
    }

    public void UseItem()
    {
        if (item.itemType == ItemType.Food)
        {
            ApplyFoodEffect();

            count -= 1;
            if (count <= 0)
            {
                InventorySlot parentSlot = transform.parent?.GetComponent<InventorySlot>();
                if (parentSlot != null)
                {
                    Debug.Log("슬롯 색상 변경을 시도합니다.");
                    parentSlot.slotBackgroundImage.color = new Color(1, 1, 1, 1); // 흰색 강제 설정
                    parentSlot.slotBackgroundImage.SetAllDirty();
                }
                else
                {
                    Debug.Log("슬롯을 찾을 수 없습니다.");
                }
                if (tooltipUI != null)
                {
                    Debug.Log("툴팁숨김");
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
            Debug.Log("This item cannot be consumed.");
        }
    }

    private void ApplyFoodEffect()
    {
        PlayerStats.Instance.ChangeMaxHealth(item.hpMax);
        PlayerStats.Instance.ChangeHealHealth(item.hp);
        PlayerStats.Instance.ChangeAttackDamage(item.attackDamage);
        PlayerStats.Instance.ChangeMoveSpeed(item.moveSpeed);

        Debug.Log($"Used {item.itemName}: MaxHP +{item.hpMax}, CurrentHP +{item.hp}, Attack +{item.attackDamage}, MoveSpeed +{item.moveSpeed}");
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
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
        Vector3 offset = new Vector3(slotRectTransform.rect.width / 4, -slotRectTransform.rect.height / 5, 0);

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
