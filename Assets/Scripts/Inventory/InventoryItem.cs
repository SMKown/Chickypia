using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image image;
    public Text countText;
    public ItemData item;

    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    private bool isDragging = false;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        InitialiseItem(item);
    }

    public void InitialiseItem(ItemData newItem)
    {
        item = newItem;
        image.sprite = newItem.itemIcon;
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
        transform.SetParent(transform.root);

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

        isDragging = false;
    }

    private void Update()
    {
        // 드래그 중이고 오른쪽 마우스 버튼을 클릭하면 아이템 하나 배치
        if (isDragging && Input.GetMouseButtonDown(1))
        {
            PlaceOneItem();
        }
    }

    private void PlaceOneItem()
    {
        if (count > 1)
        {
            // 현재 마우스 위치에서 레이캐스트를 수행하여 슬롯을 찾음
            PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();
                if (slot != null)
                {
                    InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                    if (itemInSlot == null) // 슬롯에 아이템이 없는 경우 새 아이템 생성
                    {
                        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
                        InventoryItem newItem = inventoryManager.SpawnNewItem(item, slot, 1);
                        newItem.image.raycastTarget = true;
                        newItem.countText.raycastTarget = true;
                        count -= 1; // 아이템을 놓을 때만 개수 감소
                        ItemCount();
                        return;
                    }
                    else if (itemInSlot.item == item) // 슬롯에 같은 아이템이 있는 경우 개수 증가
                    {
                        itemInSlot.count += 1;
                        itemInSlot.ItemCount();
                        count -= 1; // 아이템을 놓을 때만 개수 감소
                        ItemCount();
                        return;
                    }
                    // 슬롯에 다른 아이템이 있는 경우 아무 작업도 하지 않음
                    return;
                }
            }
            // 슬롯을 찾지 못하면 아무 작업도 하지 않음
        }
    }

    public ItemData GetItemData()
    {
        return item;
    }
}
