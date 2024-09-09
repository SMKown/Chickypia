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
    public RectTransform inventoryUIRectTransform; // �κ��丮 UI RectTransform �߰�

    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    private GameObject itemModelInstance; // 3D �� �ν��Ͻ�
    private bool isDragging = false; // �巡�� ���¸� ��Ÿ���� ���� �߰�

    private void Start()
    {
        InitialiseItem(item);
    }

    public void InitialiseItem(ItemData newItem)
    {
        item = newItem;
        image.sprite = newItem.itemIcon;
        ItemCount();

        // 3D �� �ν��Ͻ�ȭ
        if (itemModelInstance != null)
        {
            Destroy(itemModelInstance);
        }
        itemModelInstance = Instantiate(newItem.itemModel, transform);
        itemModelInstance.SetActive(false); // �κ��丮������ ������ �ʵ��� ����
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
            itemModelInstance.SetActive(true); // �巡�� �߿��� 3D ���� ���̵��� ����
        }

        isDragging = true; // �巡�� ����
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
            itemModelInstance.SetActive(false); // �巡�װ� ������ 3D ���� ����
        }

        isDragging = false; // �巡�� ����
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
