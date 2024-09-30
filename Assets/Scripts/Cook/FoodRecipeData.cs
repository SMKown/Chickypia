
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cookingsystem
{
    [CreateAssetMenu(fileName = "FoodRecipe", menuName = "Scriptable Object/FoodRecipe")]
    public class FoodRecipeData : ScriptableObject
    {
        public enum FoodType { Soup, Barbecue, Ramen }

        [Header("# Main Info")]
        public FoodType foodType;

        [SerializeField]
        public int foodRecipeId;
        public string foodName;
        public string foodDesc;
        public Sprite foodIcon;

        [Header("# Ingredients")]
        [SerializeField]
        public IngredientsInfo[] ingredients;

        [Header("# Result Item")]
        public ItemData resultItem;

        private void OnEnable()
        {
            if (resultItem && resultItem.itemIcon && resultItem.itemName != null)
            {
                foodIcon = resultItem.itemIcon;
                foodName = resultItem.itemName;
                foodRecipeId = resultItem.itemId;
            }
        }

        public int GetIngredientCount(ItemData ingredient)
        {
            foreach (var ingredientInfo in ingredients)
            {
                if (ingredientInfo.item == ingredient)
                {
                    return ingredientInfo.count;
                }
            }
            return 0;
        }
    }

    [System.Serializable]
    public struct IngredientsInfo
    {
        public ItemData item;
        public int count;
    }
}
