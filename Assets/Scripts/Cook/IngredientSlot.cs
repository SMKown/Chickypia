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

    // 드롭된 아이템을 받는 부분
    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem draggedItem = eventData.pointerDrag?.GetComponent<InventoryItem>();

        if (draggedItem == null || draggedItem.item == null) return;

        // 재료 슬롯에 아이템 배치
        currentItemData = draggedItem.item;
        currentCount = draggedItem.count;

        ingredientImage.sprite = draggedItem.item.itemIcon;
        ingredientImage.color = Color.white;
        ingredientName.text = draggedItem.item.itemName;
        ingredientCount.text = currentCount.ToString();

        // 우클릭으로 하나씩 놓기
        draggedItem.count -= 1;
        draggedItem.ItemCount();

        if (draggedItem.count <= 0)
        {
            Destroy(draggedItem.gameObject); // 아이템 전부 소진 시 제거
        }
    }

    // 재료 슬롯 초기화 (레시피 변경 시 실행)
    public void ResetSlot()
    {
        currentItemData = null;
        currentCount = 0;

        ingredientImage.sprite = null;
        ingredientImage.color = new Color(0, 0, 0, 0); // 투명하게 변경
        ingredientName.text = "";
        ingredientCount.text = "";
    }
}
