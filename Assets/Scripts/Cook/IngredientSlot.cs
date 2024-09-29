using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour, IDropHandler
{
    public ItemData requiredItem;
    private ItemData currentItem;
    private Image ingredientIcon;
    private CookingSystem cookingSystem;

    private void Awake()
    {
        ingredientIcon = GetComponent<Image>();
    }

    public void SetIngredient(ItemData item, CookingSystem system)
    {
        requiredItem = item;
        cookingSystem = system;

        if (ingredientIcon != null && requiredItem != null)
        {
            ingredientIcon.sprite = requiredItem.itemIcon;
            ingredientIcon.color = Color.gray;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem draggedItem = eventData.pointerDrag?.GetComponent<InventoryItem>();

        if (draggedItem != null && draggedItem.item == requiredItem)
        {
            currentItem = draggedItem.item;
            ingredientIcon.sprite = draggedItem.item.itemIcon;
            ingredientIcon.color = Color.white;

            draggedItem.count -= 1;
            draggedItem.ItemCount();

            if (draggedItem.count == 0)
            {
                Destroy(draggedItem.gameObject);
            }

            cookingSystem.CheckAllIngredients();
        }
    }

    public bool IsFilled()
    {
        return currentItem == requiredItem;
    }
}
