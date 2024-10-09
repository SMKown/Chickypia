using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class IngredientSlot : MonoBehaviour, IDropHandler
{
    public Image ingredientImage;
    public TextMeshProUGUI ingredientName;
    public TextMeshProUGUI ingredientCount;

    public ItemData currentItemData;
    public int currentCount;
    private CookingSystem cookingSystem;

    private Color originalColor;

    private void Awake()
    {
        cookingSystem = FindObjectOfType<CookingSystem>();
        originalColor = ingredientImage.color;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem draggedItem = eventData.pointerDrag?.GetComponent<InventoryItem>();
        if (draggedItem == null)
        {
            Debug.LogError("draggedItem is null in OnDrop");
            return;
        }

        if (currentItemData != null && currentItemData == draggedItem.item)
        {
            int remainingAmountNeeded = cookingSystem.currentRecipe.GetIngredientCount(currentItemData) - currentCount;
            int amountToAdd = Mathf.Min(draggedItem.count, remainingAmountNeeded);

            currentCount += amountToAdd;
            ingredientCount.text = currentCount.ToString();
            draggedItem.count -= amountToAdd;
            if (draggedItem.count <= 0)
            {
                Destroy(draggedItem.gameObject);
            }
            else
            {
                draggedItem.ItemCount();
            }

            cookingSystem.CheckAllIngredients();
            return;
        }

        if (currentItemData != null)
        {
            Debug.Log("이미 재료가 추가되어 있습니다.");
            return;
        }

        if (!IsCorrectIngredient(draggedItem.item))
        {
            StartCoroutine(FlashSlot(Color.red, 0.5f));
            draggedItem.transform.SetParent(draggedItem.parentAfterDrag);
            draggedItem.transform.localPosition = Vector3.zero;
            return;
        }
        AddIngredient(draggedItem);
    }

    private bool IsCorrectIngredient(ItemData draggedItem)
    {
        var currentRecipe = cookingSystem.currentRecipe;
        foreach (var ingredient in currentRecipe.ingredients)
        {
            if (ingredient.item == draggedItem)
            {
                return true;
            }
        }
        return false;
    }

    private void AddIngredient(InventoryItem draggedItem)
    {
        currentItemData = draggedItem.item;

        int requiredAmount = cookingSystem.currentRecipe.GetIngredientCount(currentItemData);
        currentCount = Mathf.Min(draggedItem.count, requiredAmount);
        ingredientImage.sprite = draggedItem.item.itemIcon;
        ingredientImage.color = Color.white;
        //ingredientName.text = draggedItem.item.itemName;
        ingredientCount.text = currentCount.ToString();

        draggedItem.count -= currentCount;
        if (draggedItem.count <= 0)
        {
            Destroy(draggedItem.gameObject);
        }
        else
        {
            draggedItem.ItemCount();
        }

        draggedItem.transform.SetParent(this.transform);
        draggedItem.transform.localPosition = Vector3.zero;

        cookingSystem.CheckAllIngredients();
    }

    public void ResetSlot()
    {
        currentItemData = null;
        currentCount = 0;

        ingredientImage.sprite = null;
        ingredientImage.color = new Color(1, 1, 1, 0.5f);
        ingredientName.text = "";
        ingredientCount.text = "";
    }

    private IEnumerator FlashSlot(Color color, float duration)
    {
        Color originalColor = ingredientImage.color;
        ingredientImage.color = color;
        yield return new WaitForSeconds(duration);
        ingredientImage.color = originalColor;
    }
}
