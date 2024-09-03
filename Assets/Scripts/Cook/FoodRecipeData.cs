using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodRecipe", menuName = "Scriptble Object/FoodRecipe")]
public class FoodRecipeData : ScriptableObject
{
    public enum FoodType { Soup, Barbecue, Ramen}

    [Header("# Main Info")]
    public FoodType foodType;
    public int foodRecipeId;
    public string foodRecipeName;
    public string foodRecipeDesc;
    public Sprite foodRecipeIcon;

    [Header("# Ingredients")]
    public ItemData[] ingredients;

    [Header("# Result Item")]
    public ItemData resultItem;
}
