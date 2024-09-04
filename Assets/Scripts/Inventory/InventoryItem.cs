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
    public Item item;

    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    private bool isRightClick = false;
    private bool isDragging = false;

    private void Start()
    {
        InitialiseItem(item);
    }

    // 아이템을 초기화하는 함수
    public void InitialiseItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.image;
        ItemCount();
    }

    // 아이템의 개수를 업데이트하고 텍스트 활성화/비활성화
    public void ItemCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    // 드래그 시작 시 호출되는 함수
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 중 이미지와 개수 텍스트의 레이캐스트를 비활성화
        image.raycastTarget = false;
        countText.raycastTarget = false;

        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);

        isRightClick = eventData.button == PointerEventData.InputButton.Right;
        isDragging = true;
    }

    // 드래그 중 호출되는 함수
    public void OnDrag(PointerEventData eventData)
    {
        // 마우스 위치에 아이템을 이동
        transform.position = Input.mousePosition;
    }

    // 드래그 종료 시 호출되는 함수
    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 중 이미지와 개수 텍스트의 레이캐스트를 재활성화
        image.raycastTarget = true;
        countText.raycastTarget = true;

        transform.SetParent(parentAfterDrag);

        LeftClickDrag();

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

    private void LeftClickDrag() { }

    // 아이템 하나를 새로운 슬롯에 배치하는 함수
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
}
