using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
    public FoodRecipeData foodRecipeData;
    public ItemData itemData;
    public ItemData FoodData;

    Image icon;

    private void Awake()
    {
        icon = GetComponentInChildren<Image>();
        icon.sprite = foodRecipeData.foodRecipeIcon;
    }

    private void LateUpdate()
    {
        
    }

    public void OnClick()
    {
        switch (foodRecipeData.foodRecipeId)
        {
            case 0:
                // ¿ìÃø µ¦ Ç¥½Ã

            default:
                break;
        }
    }
}
