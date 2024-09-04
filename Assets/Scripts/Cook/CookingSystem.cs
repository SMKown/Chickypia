using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CookingSystem : MonoBehaviour
{    
    public ParticleSystem making;

    private InventorySlot mResultItemSlot;

    private Transform choicePopup;
    private Transform makePopup;

    private Button mCookButton;


    GameObject mDisableImage;

    private void Awake()
    {
        choicePopup = transform.GetChild(1);
        makePopup = transform.GetChild(1).GetChild(0).GetChild(1);

        if (choicePopup == null)
        {
            Debug.LogError("choicePopup is not assigned.");
        }
        if (makePopup == null)
        {
            Debug.LogError("makePopup is not assigned.");
        }
    }    

    void Cook(Collider other)
    {
        
    }
}
