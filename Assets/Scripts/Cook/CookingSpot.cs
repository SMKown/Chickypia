using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingSpot : MonoBehaviour
{
    private static bool mIsCooking = false;
    public static bool isCooking
    {
        get { return mIsCooking; }
    }    

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mIsCooking = true;
            Debug.Log(mIsCooking);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mIsCooking = false;
            Debug.Log(mIsCooking);
        }
    }
}
