
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
        public int foodRecipeId;
        public string foodName;
        public string foodDesc;
        public Sprite foodIcon;

        [Header("# Ingredients")]
        public ItemData[] ingredients;

        [Header("# Result Item")]
        public ItemData resultItem;

        private void OnEnable()
        {
            if (resultItem != null && resultItem.itemIcon != null)
            {
                foodIcon = resultItem.itemIcon;
            }
            else
            {
                Debug.LogWarning("itemIcon is not assigned.");
            }
        }
    }

    [System.Serializable]
    public struct IngredientsInfo
    {
        [SerializeField] public ItemData item;
        [SerializeField] public int count;
    }
}
