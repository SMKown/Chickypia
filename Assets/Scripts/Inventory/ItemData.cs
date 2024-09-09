using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptble Object/ItemData")]
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
}

public enum ItemType
{
    Food,
    Tool
}
