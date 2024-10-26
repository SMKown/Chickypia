using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("# Main Info")]
    public ItemType itemType;
    public EffectType effectType;
    public int itemId;
    public string itemName;
    public string itemDesc;

    public bool stackable = true;

    public Sprite itemIcon;
    public GameObject itemModel;

    [Header("# Effect")]
    public float time; 
    public int hp;
    public int hpMax;
    public float moveSpeed;    
    public int attackDamage;
   

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

public enum EffectType
{
    Hp,
    MaxHp,
    MoveSpeed,
    AttackDamage,
}

public enum ItemCategory
{
    Gathering,
    Cooking,
    Fishing,
    MonsterDrop
}