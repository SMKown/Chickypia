using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour, IDropHandler
{
    public Image ingredientImage;
    public Text ingredientName;
    public Text ingredientCount;

    public ItemData currentItemData;
    public int currentCount;

    // ��ӵ� �������� �޴� �κ�
    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem draggedItem = eventData.pointerDrag?.GetComponent<InventoryItem>();

        if (draggedItem == null || draggedItem.item == null) return;

        // ��� ���Կ� ������ ��ġ
        currentItemData = draggedItem.item;
        currentCount = draggedItem.count;

        ingredientImage.sprite = draggedItem.item.itemIcon;
        ingredientImage.color = Color.white;
        ingredientName.text = draggedItem.item.itemName;
        ingredientCount.text = currentCount.ToString();

        // ��Ŭ������ �ϳ��� ����
        draggedItem.count -= 1;
        draggedItem.ItemCount();

        if (draggedItem.count <= 0)
        {
            Destroy(draggedItem.gameObject); // ������ ���� ���� �� ����
        }
    }

    // ��� ���� �ʱ�ȭ (������ ���� �� ����)
    public void ResetSlot()
    {
        currentItemData = null;
        currentCount = 0;

        ingredientImage.sprite = null;
        ingredientImage.color = new Color(0, 0, 0, 0); // �����ϰ� ����
        ingredientName.text = "";
        ingredientCount.text = "";
    }
}
