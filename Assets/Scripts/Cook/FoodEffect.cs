using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodEffect : MonoBehaviour
{
    public void GetFood(ItemData selectedItem)
    {
        Debug.Log(selectedItem.name);
    }
}
