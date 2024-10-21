using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    public string itemDesc;

    public bool stackable = true;

    public Sprite itemIcon;
    public GameObject itemModel;

    [Header("# Effect")]
    public float moveSpeed;
    public int attackDamage;
    public int hp;
    public int hpMax;

    [Header("# Category")]
    public ItemCategory category;

    [Header("# Compendium")]
    public bool isCollected = false;
}

public enum ItemType
{
    Food,
    Ingredient
}

public enum ItemCategory
{
    Gathering,
    Cooking,
    Fishing,
    MonsterDrop
}