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

    public bool isLetsCook = false;

    public FoodRecipeData[] foodRecipeData;   

    private InventoryManager inventoryManager;
    private InvenCompenUI invenCompenUI;
    private InventorySlot resultItemSlot;

    private Transform choicePopup;
    private Transform makePopup;

    private TextMeshProUGUI foodName;
    //private TextMeshProUGUI ingredientsName;
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
    private QuestManager questManager;
    private PlayerMovement PlayerMovement;

    private AudioSource audioSource;

    private void Awake()
    {
        choicePopup = transform.GetChild(0);
        makePopup = transform.GetChild(1);
        foodRecipeParent = transform.GetChild(0);
        ingredientParent = transform.GetChild(1).GetChild(1).GetChild(1);
        cookButton = transform.GetChild(1).GetChild(1).GetChild(2).GetChild(1).GetComponent<Button>();
        foodImage = makePopup.GetChild(0).GetChild(0).GetComponent<Image>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        invenCompenUI = FindObjectOfType<InvenCompenUI>();
        questManager = FindObjectOfType<QuestManager>();

        audioSource = GetComponent<AudioSource>();

        if (choicePopup == null) { Debug.LogError("choicePopup is not assigned."); }
        if (makePopup == null) { Debug.LogError("makePopup is not assigned."); }
        if (foodRecipeParent == null) { Debug.LogError("foodRecipeParent is not assigned."); }
        if (ingredientParent == null) { Debug.LogError("ingredientParent is not assigned."); }        
    }
    private void Start()
    {
        PlayerMovement = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        if (isLetsCook && !PlayerInfo.Instance.attackMode && !PlayerInfo.Instance.fishing && !GameManager.Instance.isOptionActive)
        {
            StartCook();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isCooking)
            {
                StopCook();
            }
            else if (!invenCompenUI.isInventoryOpen && !PlayerInfo.Instance.attackMode && !PlayerInfo.Instance.fishing && !GameManager.Instance.isOptionActive)
            {
                StartCook();
            }
        }
    }

    public bool IsCookingActive()
    {
        return isCooking;
    }


    void StartCook()
    {
        PlayerInfo.Instance.cooking = true;

        isCooking = true;
        choicePopup.gameObject.SetActive(true);
        makePopup.gameObject.SetActive(true);
        invenCompenUI.SetInventoryActive(true);
        foodImage.gameObject.SetActive(false);
        foreach (Transform child in foodRecipeParent)
        {
            Destroy(child.gameObject);
        }

        cookButton.onClick.RemoveAllListeners();
        cookButton.onClick.AddListener(() => Cook(currentRecipe, questManager));

        ClearIngredientSlots();

        foreach (var foodRecipe in foodRecipeData)
        {
            if (foodRecipe != null)
            {
                GameObject newIngredient = Instantiate(foodRecipePrefab, foodRecipeParent);

                Image foodRecipeIcon = newIngredient.transform.GetChild(0).GetComponent<Image>();
                //TextMeshProUGUI foodRecipeName = newIngredient.transform.GetChild(1).GetComponent< TextMeshProUGUI>();

                foodRecipeIcon.sprite = foodRecipe.foodIcon;                
                
                //if (foodRecipeName != null) { foodRecipeName.text = foodRecipe.foodName; }
                               

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
        PlayerInfo.Instance.cooking = false;

        ResetIngredientSlots();
        choicePopup.gameObject.SetActive(false);
        makePopup.gameObject.SetActive(false);
        isCooking = false;
        invenCompenUI.SetInventoryActive(false);
        if (foodName != null)
        {
            foodName.text = "";
        }

        if (foodImage != null)
        {
            foodImage.sprite = null;
            foodImage.gameObject.SetActive(false);
        }

        cookButton.interactable = false;
        currentRecipe = null;
    }

    public void ChoiceRecipe(FoodRecipeData recipe)
    {
        ResetIngredientSlots();

        currentRecipe = recipe;
        UpdateUIRecipe(currentRecipe);
    }

    private void UpdateUIRecipe(FoodRecipeData recipe)
    {
        foodName = makePopup.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();

        foodImage.gameObject.SetActive(true);

        cookButton.interactable = false;

        if (!string.IsNullOrEmpty(recipe.foodName))
        {
            foodName.text = recipe.foodName;
        }

        if (recipe.foodIcon != null)
        {
            foodImage.sprite = recipe.foodIcon;
        }

        ClearIngredientSlots();
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

            // TextMeshProUGUI ingredientName = newIngredient.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI ingrediencount = newIngredient.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            Image ingredientIcon = newIngredient.transform.GetChild(0).GetComponent<Image>();

            // ingredientName.text = ingredient.item.itemName;
            ingredientIcon.sprite = ingredient.item.itemIcon;
            ingredientIcon.color = new Color(1, 1, 1, 0.5f);
            ingrediencount.text = ingredient.count.ToString();

            IngredientSlot ingredientSlot = FindIngredientSlot(ingredient.item);
            if (ingredientSlot == null || ingredientSlot.currentCount < ingredient.count)
            {
                allIngredients = false;
            }

        }

        cookButton.interactable = allIngredients;
    }

    private void ResetIngredientSlots()
    {
        foreach (Transform child in ingredientParent)
        {
            IngredientSlot ingredientSlot = child.GetComponent<IngredientSlot>();
            if (ingredientSlot != null && ingredientSlot.currentItemData != null)
            {
                inventoryManager.AddItem(ingredientSlot.currentItemData, ingredientSlot.currentCount);
                ingredientSlot.ResetSlot();
            }
        }
    }

    private void ClearIngredientSlots()
    {
        foreach (Transform child in ingredientParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void Cook(FoodRecipeData recipe, QuestManager questManager)
    {
        if(isCooking)
        {
            choicePopup.gameObject.SetActive(false);
            makePopup.gameObject.SetActive(false);
            invenCompenUI.SetInventoryActive(false);
            foodImage.gameObject.SetActive(false);
            PlayerMovement.CookAniStart();
            cookButton.interactable = false;
            foreach (var ingredient in recipe.ingredients)
            {
                IngredientSlot ingredientSlot = FindIngredientSlot(ingredient.item);
                if (ingredientSlot != null && ingredientSlot.currentItemData != null)
                {
                    ingredientSlot.ResetSlot();
                }
            }

            StartCoroutine(Cooking(recipe, questManager));
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

    public void CheckAllIngredients()
    {
        bool allIngredientsMet = true;

        foreach (var ingredient in currentRecipe.ingredients)
        {
            IngredientSlot ingredientSlot = FindIngredientSlot(ingredient.item);
            if (ingredientSlot == null || ingredientSlot.currentCount < ingredient.count)
            {
                allIngredientsMet = false;
                break;
            }
        }

        cookButton.interactable = allIngredientsMet;
    }

    IEnumerator Cooking(FoodRecipeData recipe, QuestManager questManager)
    {
        audioSource.Play();
        yield return new WaitForSeconds(3f);
        audioSource.Stop();
        inventoryManager.AddItem(recipe.resultItem, 1);

        // 수집한 아이템의 ID를 퀘스트와 비교
        if (questManager != null)
        {
            foreach (var quest in questManager.questList)
            {
                if (quest.itemId == recipe.resultItem.itemId)
                {
                    quest.UpdateItemCount(1);
                }
            }
        }

        currentRecipe = null;
        foodName.text = "";
        foodImage.sprite = null;
        ClearIngredientSlots();

        choicePopup.gameObject.SetActive(true);
        makePopup.gameObject.SetActive(true);
        invenCompenUI.SetInventoryActive(true);
        PlayerMovement.CookAniEnd();
        cookButton.interactable = true;
    }
}
