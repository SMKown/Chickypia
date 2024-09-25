using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ItemToolTipUI : MonoBehaviour
{
    public GameObject tooltipUI;
    public Text ItemName;
    public Text tooltipText;

    public static ItemToolTipUI Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        tooltipUI.SetActive(false);
    }

    public void ShowToolTip(string itemDec, string itemName)
    {
        ItemName.text = itemName;
        tooltipText.text = itemDec;
        tooltipUI.SetActive(true);
    }
    public void HideToolTip() 
    { 
        tooltipUI.SetActive(false);
    }
}
