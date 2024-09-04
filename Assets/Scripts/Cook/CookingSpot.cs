using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingSpot : MonoBehaviour
{
    private static bool mcanCooking = false;
    public static bool canCooking
    {
        get { return mcanCooking; }
    }    

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mcanCooking = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mcanCooking = false;
        }
    }
}
