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

    private InventoryItem resultItemSlot;
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
    private Button cookButton;
    private Transform recipeTransform;

    private bool isCooking = true;

    public FoodRecipeData currentRecipe;


    private void Awake()
    {
        choicePopup = transform.GetChild(0);
        makePopup = transform.GetChild(1);
        foodRecipeParent = transform.GetChild(0);
        ingredientParent = transform.GetChild(1).GetChild(1).GetChild(0);


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
                isCooking = true;
                StartCook();
            }
        }
    }

    void StartCook()
    {
        choicePopup.gameObject.SetActive(true);

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

                if (foodRecipeIcon != null) { foodRecipeIcon.sprite = foodRecipe.foodIcon; }
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
        choicePopup.gameObject.SetActive(false);
        makePopup.gameObject.SetActive(false);
        isCooking = false;
    }

    public void ChoiceRecipe(FoodRecipeData recipe)
    {
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

        float initialX = 0f;
        float xOffset = 200f;

        foreach (var ingredient in recipe.ingredients)
        {
            if (ingredient != null)
            {
                GameObject newIngredient = Instantiate(ingredientPrefab, ingredientParent);
                RectTransform rectTransform = newIngredient.GetComponent<RectTransform>();

                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(initialX, rectTransform.anchoredPosition.y);
                    initialX += xOffset;
                }

                TextMeshProUGUI ingredientName = newIngredient.GetComponentInChildren<TextMeshProUGUI>();
                Image ingredientIcon = newIngredient.transform.GetChild(0).GetComponent<Image>();

                if (ingredientName != null) { ingredientName.text = ingredient.itemName; }
                if (ingredientIcon != null) { ingredientIcon.sprite = ingredient.itemIcon; }
            }
        }
    }

    public void Cooking()
    {

    }
}
