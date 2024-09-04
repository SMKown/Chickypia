using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class CookingSystem : MonoBehaviour
{
    public GameObject cookingSpot;    

    private InventorySlot mResultItemSlot;
    private Transform choicePopup;
    private Transform makePopup;
    private Button mCookButton;
    private GameObject mDisableImage;

    private bool isCooking = false;

    private void Awake()
    {
        choicePopup = transform.GetChild(1);
        makePopup = transform.GetChild(1).GetChild(1);


        if (choicePopup == null)
        {
            Debug.LogError("choicePopup is not assigned.");
        }
        if (makePopup == null)
        {
            Debug.LogError("makePopup is not assigned.");
        }
    }

    private void Update()
    {
        if (CookingSpot.canCooking && Input.GetKeyDown(KeyCode.Q))
        {
            if(isCooking)
            {
                StopCook();
            }
            else
            {
                isCooking = true;
                Cook();
            }
        }
    }

    void Cook()
    {
        choicePopup.gameObject.SetActive(true);
    }

    void StopCook()
    {
        choicePopup.gameObject.SetActive(false);
        isCooking = false;
    }

    public void ChoiceRecipe()
    {
        if (!makePopup.gameObject.activeSelf)
        {
            makePopup.gameObject.SetActive(true);
        }
    }
}
