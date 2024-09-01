using UnityEngine;

[System.Serializable]
public class Item
{
    public Sprite image;

    public Item(Sprite itemimage)
    {
        image = itemimage;
    }
}