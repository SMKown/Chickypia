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

    private bool isRightClick = false;
    private bool isDragging = false;

    private void Start()
    {
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

        isRightClick = eventData.button == PointerEventData.InputButton.Right;
        isDragging = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        countText.raycastTarget = true;

        transform.SetParent(parentAfterDrag);

        LeftClickDrag();

        isDragging = false;
    }

    private void LeftClickDrag() { }
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
            count -= 1;
            ItemCount();

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
                    if (itemInSlot != null && itemInSlot.item == item)  // 같은 아이템이 슬롯에 있으면 개수 증가
                    {
                        itemInSlot.count += 1;
                        itemInSlot.ItemCount();
                        itemInSlot.image.raycastTarget = true;
                        itemInSlot.countText.raycastTarget = true;
                    }
                    else // 슬롯에 다른 아이템이 있으면 새 아이템 생성
                    {
                        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
                        InventoryItem newItem = inventoryManager.SpawnNewItem(item, slot, 1);
                        newItem.image.raycastTarget = true;
                        newItem.countText.raycastTarget = true;
                    }
                    return;
                }
            }
            // 슬롯을 찾지 못하면 원래 부모로 돌아감
            InventoryManager fallbackInventoryManager = FindObjectOfType<InventoryManager>();
            InventoryItem fallbackItem = fallbackInventoryManager.SpawnNewItem(item, parentAfterDrag.GetComponent<InventorySlot>(), 1);
            fallbackItem.image.raycastTarget = true;
            fallbackItem.countText.raycastTarget = true;
        }
    }
    public ItemData GetItemData()
    {
        return item;
    }
}