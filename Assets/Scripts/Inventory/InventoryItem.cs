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

    // �������� �ʱ�ȭ�ϴ� �Լ�
    public void InitialiseItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.image;
        ItemCount();
    }

    // �������� ������ ������Ʈ�ϰ� �ؽ�Ʈ Ȱ��ȭ/��Ȱ��ȭ
    public void ItemCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    // �巡�� ���� �� ȣ��Ǵ� �Լ�
    public void OnBeginDrag(PointerEventData eventData)
    {
        // �巡�� �� �̹����� ���� �ؽ�Ʈ�� ����ĳ��Ʈ�� ��Ȱ��ȭ
        image.raycastTarget = false;
        countText.raycastTarget = false;

        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);

        isRightClick = eventData.button == PointerEventData.InputButton.Right;
        isDragging = true;
    }

    // �巡�� �� ȣ��Ǵ� �Լ�
    public void OnDrag(PointerEventData eventData)
    {
        // ���콺 ��ġ�� �������� �̵�
        transform.position = Input.mousePosition;
    }

    // �巡�� ���� �� ȣ��Ǵ� �Լ�
    public void OnEndDrag(PointerEventData eventData)
    {
        // �巡�� �� �̹����� ���� �ؽ�Ʈ�� ����ĳ��Ʈ�� ��Ȱ��ȭ
        image.raycastTarget = true;
        countText.raycastTarget = true;

        transform.SetParent(parentAfterDrag);

        LeftClickDrag();

        isDragging = false;
    }

    private void Update()
    {
        // �巡�� ���̰� ������ ���콺 ��ư�� Ŭ���ϸ� ������ �ϳ� ��ġ
        if (isDragging && Input.GetMouseButtonDown(1))
        {
            PlaceOneItem();
        }
    }

    private void LeftClickDrag() { }

    // ������ �ϳ��� ���ο� ���Կ� ��ġ�ϴ� �Լ�
    private void PlaceOneItem()
    {
        if (count > 1)
        {
            count -= 1;
            ItemCount();

            // ���� ���콺 ��ġ���� ����ĳ��Ʈ�� �����Ͽ� ������ ã��
            PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();
                if (slot != null)
                {
                    InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                    if (itemInSlot != null && itemInSlot.item == item)  // ���� �������� ���Կ� ������ ���� ����
                    {
                        itemInSlot.count += 1;
                        itemInSlot.ItemCount();
                        itemInSlot.image.raycastTarget = true;
                        itemInSlot.countText.raycastTarget = true;
                    }
                    else // ���Կ� �ٸ� �������� ������ �� ������ ����
                    {
                        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
                        InventoryItem newItem = inventoryManager.SpawnNewItem(item, slot, 1);
                        newItem.image.raycastTarget = true;
                        newItem.countText.raycastTarget = true;
                    }
                    return;
                }
            }
            // ������ ã�� ���ϸ� ���� �θ�� ���ư�
            InventoryManager fallbackInventoryManager = FindObjectOfType<InventoryManager>();
            InventoryItem fallbackItem = fallbackInventoryManager.SpawnNewItem(item, parentAfterDrag.GetComponent<InventorySlot>(), 1);
            fallbackItem.image.raycastTarget = true;
            fallbackItem.countText.raycastTarget = true;
        }
    }
}
