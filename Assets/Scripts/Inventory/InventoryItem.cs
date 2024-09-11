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

            // ���� ���콺 ��ġ���� ����ĳ��Ʈ�� �����Ͽ� ������ ã��
            PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                InventoryItem slot = result.gameObject.GetComponent<InventoryItem>();
                if (slot != null)
                {
                    //InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                    if (slot.item == item)  // ���� �������� ���Կ� ������ ���� ����
                    {
                        slot.count += 1;
                        slot.ItemCount();
                    }
                    else // ���Կ� �ٸ� �������� ������ �� ������ ����
                    {
                        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
                        //inventoryManager.SpawnNewItem(item, slot, 1);
                    }
                    return;
                }
            }
            // ������ ã�� ���ϸ� ���� �θ�� ���ư�
            InventoryManager fallbackInventoryManager = FindObjectOfType<InventoryManager>();
            InventoryItem fallbackItem = fallbackInventoryManager.SpawnNewItem(item, parentAfterDrag.GetComponent<InventorySlot>(), 1);
        }
    }

    public ItemData GetItemData()
    {
        return item;
    }
}
