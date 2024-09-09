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
    public RectTransform inventoryUIRectTransform; // 인벤토리 UI RectTransform 추가

    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    private GameObject itemModelInstance; // 3D 모델 인스턴스
    private bool isDragging = false; // 드래그 상태를 나타내는 변수 추가

    private void Start()
    {
        InitialiseItem(item);
    }

    public void InitialiseItem(ItemData newItem)
    {
        item = newItem;
        image.sprite = newItem.itemIcon;
        ItemCount();

        // 3D 모델 인스턴스화
        if (itemModelInstance != null)
        {
            Destroy(itemModelInstance);
        }
        itemModelInstance = Instantiate(newItem.itemModel, transform);
        itemModelInstance.SetActive(false); // 인벤토리에서는 보이지 않도록 설정
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

        if (itemModelInstance != null)
        {
            itemModelInstance.SetActive(true); // 드래그 중에는 3D 모델을 보이도록 설정
        }

        isDragging = true; // 드래그 시작
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        countText.raycastTarget = true;

        if (RectTransformUtility.RectangleContainsScreenPoint(inventoryUIRectTransform, Input.mousePosition))
        {
            transform.SetParent(parentAfterDrag);
            transform.localPosition = Vector3.zero;
        }
        else
        {
            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
            inventoryManager.DropItem(item);
            Destroy(gameObject);
        }

        if (itemModelInstance != null)
        {
            itemModelInstance.SetActive(false); // 드래그가 끝나면 3D 모델을 숨김
        }

        isDragging = false; // 드래그 종료
    }

    private void Update()
    {
        if (isDragging && Input.GetMouseButtonDown(1))
        {
            PlaceOneItem();
        }
    }

    private void LeftClickDrag() { }

    private void PlaceOneItem()
    {
        if (count > 1)
        {
            count -= 1;
            ItemCount();

            PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();
                if (slot != null)
                {
                    InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                    if (itemInSlot != null && itemInSlot.item == item)
                    {
                        itemInSlot.count += 1;
                        itemInSlot.ItemCount();
                        itemInSlot.image.raycastTarget = true;
                        itemInSlot.countText.raycastTarget = true;
                    }
                    else
                    {
                        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
                        InventoryItem newItem = inventoryManager.SpawnNewItem(item, slot, 1);
                        newItem.image.raycastTarget = true;
                        newItem.countText.raycastTarget = true;
                    }
                    return;
                }
            }

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
