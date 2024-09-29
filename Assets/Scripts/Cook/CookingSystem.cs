using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cookingsystem;
using Unity.VisualScripting;

public class CookingSystem : MonoBehaviour
{
    // public GameObject cookingSpot;    

    public FoodRecipeData[] foodRecipeData;   

    private InventoryManager inventoryManager;
    private InventorySlot resultItemSlot;

    private Transform choicePopup;
    private Transform makePopup;

    private TextMeshProUGUI foodName;
    private TextMeshProUGUI ingredientsName;
    private Image foodImage;
    private GameObject disableImage;

    public GameObject foodRecipePrefab;
    public GameObject ingredientPrefab;
    private Transform foodRecipeParent;
    private Transform ingredientParent;

    private Button recipeButton;
    public Button cookButton;
    private Transform recipeTransform;

    private bool isCooking = false;

    public FoodRecipeData currentRecipe;


    private void Awake()
    {
        choicePopup = transform.GetChild(0);
        makePopup = transform.GetChild(1);
        foodRecipeParent = transform.GetChild(0);
        ingredientParent = transform.GetChild(1).GetChild(1).GetChild(0);
        cookButton = transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Button>();
        inventoryManager = FindObjectOfType<InventoryManager>();

        if (choicePopup == null) { Debug.LogError("choicePopup is not assigned."); }
        if (makePopup == null) { Debug.LogError("makePopup is not assigned."); }
        if (foodRecipeParent == null) { Debug.LogError("foodRecipeParent is not assigned."); }
        if (ingredientParent == null) { Debug.LogError("ingredientParent is not assigned."); }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isCooking)
            {
                StopCook();
            }
            else
            {
                StartCook();
            }
        }

        cookButton.onClick.AddListener(() => Cook(currentRecipe));
    }

    public bool IsCookingActive()
    {
        return isCooking;
    }


    void StartCook()
    {
        isCooking = true;
        choicePopup.gameObject.SetActive(true);

        //List<(ItemData item, int count)> inventoryItems = inventoryManager.GetInventoryItems();

        //foreach (var item in inventoryItems)
        //{
        //    Debug.Log($"Inventory Item: {item.item.name}, Count: {item.count}");
        //}
        inventoryManager.ShowInventory();
        foreach (Transform child in foodRecipeParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var foodRecipe in foodRecipeData)
        {
            if (foodRecipe != null)
            {
                GameObject newIngredient = Instantiate(foodRecipePrefab, foodRecipeParent);

                Image foodRecipeIcon = newIngredient.transform.GetChild(0).GetComponent<Image>();
                TextMeshProUGUI foodRecipeName = newIngredient.transform.GetChild(1).GetComponent< TextMeshProUGUI>();                

                if (foodRecipeIcon != null) 
                {                    
                    foodRecipeIcon.sprite = foodRecipe.foodIcon;
                    foodRecipeIcon.color = Color.gray;
                }
                if (foodRecipeName != null) { foodRecipeName.text = foodRecipe.foodName; }
                               

                recipeButton = newIngredient.transform.GetChild(0).GetComponent<Button>();
                if (recipeButton != null)
                {
                    recipeButton.onClick.AddListener(() => ChoiceRecipe(foodRecipe));
                }
            }
        }
    }

    void StopCook()
    {
        foreach (Transform child in ingredientParent)
        {
            IngredientSlot ingredientSlot = child.GetComponent<IngredientSlot>();
            if (ingredientSlot.currentItemData != null) 
            {
                inventoryManager.AddItem(ingredientSlot.currentItemData, ingredientSlot.currentCount);
                ingredientSlot.ResetSlot();
            }
        }

        choicePopup.gameObject.SetActive(false);
        makePopup.gameObject.SetActive(false);
        isCooking = false;
        inventoryManager.HideInventory();
    }

    public void ChoiceRecipe(FoodRecipeData recipe)
    {
        foreach (Transform child in ingredientParent)
        {
            IngredientSlot ingredientSlot = child.GetComponent<IngredientSlot>();
            if (ingredientSlot.currentItemData != null)
            {
                inventoryManager.AddItem(ingredientSlot.currentItemData, ingredientSlot.currentCount);
                ingredientSlot.ResetSlot();
            }
        }

        currentRecipe = recipe;

        if (!makePopup.gameObject.activeSelf)
        {
            makePopup.gameObject.SetActive(true);
        }

        UpdateUIRecipe(currentRecipe);
    }

    private void UpdateUIRecipe(FoodRecipeData recipe)
    {
        foodName = makePopup.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        foodImage = makePopup.GetChild(0).GetChild(0).GetComponent<Image>();

        cookButton.interactable = false;

        if (!string.IsNullOrEmpty(recipe.foodName))
        {
            foodName.text = recipe.foodName;
        }

        if (recipe.foodIcon != null)
        {
            foodImage.sprite = recipe.foodIcon;
        }

        foreach (Transform child in ingredientParent)
        {
            Destroy(child.gameObject);
        }
        bool allIngredients = true;
        float initialX = 0f;
        float xOffset = 200f;

        foreach (var ingredient in recipe.ingredients)
        {
            GameObject newIngredient = Instantiate(ingredientPrefab, ingredientParent);
            RectTransform rectTransform = newIngredient.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(initialX, rectTransform.anchoredPosition.y);
                initialX += xOffset;
            }

            TextMeshProUGUI ingredientName = newIngredient.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI ingrediencount = newIngredient.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            Image ingredientIcon = newIngredient.transform.GetChild(0).GetComponent<Image>();

            if (ingredientName != null) { ingredientName.text = ingredient.item.itemName; }
            if (ingredientIcon != null) { ingredientIcon.sprite = ingredient.item.itemIcon; }
            if (ingrediencount != null) { ingrediencount.text = ingredient.count.ToString(); }

            IngredientSlot ingredientSlot = FindIngredientSlot(ingredient.item);
            if (ingredientSlot == null || ingredientSlot.currentCount < ingredient.count)
            {
                allIngredients = false;
            }
        }

        cookButton.interactable = allIngredients;
    }

    public void Cook(FoodRecipeData recipe)
    {
        if(isCooking)
        {
            Debug.Log("Cooking...");
            choicePopup.gameObject.SetActive(false);
            makePopup.gameObject.SetActive(false);
            foreach (var ingredient in recipe.ingredients)
            {
                IngredientSlot ingredientSlot = FindIngredientSlot(ingredient.item);
                if (ingredientSlot != null && ingredientSlot.currentItemData != null)
                {
                    int usedAmount = ingredient.count;
                    int remainingAmount = ingredientSlot.currentCount - usedAmount;

                    if (remainingAmount > 0)
                    {
                        inventoryManager.AddItem(ingredientSlot.currentItemData, remainingAmount);
                    }

                    ingredientSlot.ResetSlot();
                }
            }

            StartCoroutine(Cooking(recipe));
        }
        else
        {
            Debug.Log("Please Choice Recipe!");
        }
    }

    private IngredientSlot FindIngredientSlot(ItemData ingredient)
    {
        foreach (Transform child in ingredientParent)
        {
            IngredientSlot slot = child.GetComponent<IngredientSlot>();
            if (slot != null && slot.currentItemData == ingredient)
            {
                return slot;
            }
        }
        return null;
    }

    IEnumerator Cooking(FoodRecipeData recipe)
    {
        yield return new WaitForSeconds(3f);

        choicePopup.gameObject.SetActive(true);
        makePopup.gameObject.SetActive(true);
    }
}
