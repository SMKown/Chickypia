using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("UI")]
    public Image image;
    public Text countText;

    public ItemData item;
    [HideInInspector] public int count = 1;

    [HideInInspector] public Transform parentAfterDrag;
    private bool isRightClick = false;
    private bool isDragging = false;

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color notSelectedColor;

    private void Start()
    {
        InitialiseItem(item);
        DeSelect();  // �ʱ� ���´� ���õ��� ���� ����
    }

    public void InitialiseItem(ItemData newItem)
    {
        item = newItem;
        image.sprite = newItem?.itemIcon;
        image.gameObject.SetActive(newItem != null);
        ItemCount();
    }

    public void ItemCount()
    {
        countText.text = count.ToString();
        countText.gameObject.SetActive(count > 1);
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void DeSelect()
    {
        image.color = notSelectedColor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        countText.raycastTarget = false;

        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);  // �巡�� ���� �� �ֻ����� �̵�

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

        transform.SetParent(parentAfterDrag);  // �巡�װ� ������ ���� �θ�� ����
        transform.localPosition = Vector3.zero;  // ���� �ڸ��� ����

        if (isRightClick)
        {
            PlaceOneItem();
        }

        isDragging = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot draggedItemSlot = eventData.pointerDrag?.GetComponent<InventorySlot>();
        if (draggedItemSlot == null) return;

        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null)
        {
            inventoryManager.OnItemSwapped(draggedItemSlot, this);  // �巡�׵� �����۰� ���� ������ �������� ��ȯ
        }
    }

    public ItemData GetItemData()
    {
        return item;
    }

    private void PlaceOneItem()
    {
        if (count <= 1) return;

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
                if (slot.item == item)
                {
                    slot.count += 1;
                    slot.ItemCount();
                }
                else
                {
                    InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
                    inventoryManager.SpawnNewItem(item, slot, 1);
                }
                return;
            }
        }

        InventoryManager fallbackInventoryManager = FindObjectOfType<InventoryManager>();
        fallbackInventoryManager.SpawnNewItem(item, this, 1);
    }
}
