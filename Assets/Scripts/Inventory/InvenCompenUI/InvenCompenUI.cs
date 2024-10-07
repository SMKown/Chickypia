using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvenCompenUI : MonoBehaviour
{
    public GameObject MainInventory;
    public GameObject CompendiumUI;
    public GameObject InventoryOpenButton;
    public GameObject CompendiumOpenButton;

    [HideInInspector] public bool isInventoryOpen = false;

    private void Start()
    {
        MainInventory.SetActive(false);
        CompendiumUI.SetActive(false);
        InventoryOpenButton.SetActive(false);
        CompendiumOpenButton.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        SetInventoryActive(isInventoryOpen);
        InventoryOpenButton.SetActive(isInventoryOpen);
        CompendiumOpenButton.SetActive(isInventoryOpen);
        CompendiumUI.SetActive(false);
    }
    public void OpenCompendium()
    {
        MainInventory.SetActive(false);
        CompendiumUI.SetActive(true);
    }
    public void CloseCompendium()
    {
        CompendiumUI.SetActive(false);
        MainInventory.SetActive(true);
    }

    public void SetInventoryActive(bool isActive)
    {
        MainInventory.SetActive(isActive);
        isInventoryOpen = isActive;
    }
}
