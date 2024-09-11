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
        countText.gameObject.SetActive(count > 1);
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
        transform.localPosition = Vector3.zero;

        if (isRightClick)
        {
            PlaceOneItem();
        }
        isDragging = false;
    }

    private void LeftClickDrag() { }

    private void PlaceOneItem()
    {
        if (count <= 1) return;
        {
            count -= 1;
            ItemCount();

            // 현재 마우스 위치에서 레이캐스트를 수행하여 슬롯을 찾음
            PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                InventoryItem slot = result.gameObject.GetComponent<InventoryItem>();
                if (slot != null)
                {
                    //InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                    if (slot.item == item)  // 같은 아이템이 슬롯에 있으면 개수 증가
                    {
                        slot.count += 1;
                        slot.ItemCount();
                    }
                    else // 슬롯에 다른 아이템이 있으면 새 아이템 생성
                    {
                        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
                        //inventoryManager.SpawnNewItem(item, slot, 1);
                    }
                    return;
                }
            }
            // 슬롯을 찾지 못하면 원래 부모로 돌아감
            InventoryManager fallbackInventoryManager = FindObjectOfType<InventoryManager>();
            InventoryItem fallbackItem = fallbackInventoryManager.SpawnNewItem(item, parentAfterDrag.GetComponent<InventorySlot>(), 1);
        }
    }

    public ItemData GetItemData()
    {
        return item;
    }
}
