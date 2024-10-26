using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvenCompenUI : MonoBehaviour
{
    public GameObject MainInventory;
    public GameObject CompendiumUI;

    public GameObject gatheringUI;
    public GameObject cookingUI;
    public GameObject fishingUI;
    public GameObject monsterDropUI;

    public GameObject InventoryOpenButton;
    public GameObject CompendiumOpenButton;

    [HideInInspector] public bool isInventoryOpen = false;

    private ItemToolTipUI tooltipUI;
    public CompendiumManager compendiumManager;

    private void Start()
    {
        MainInventory.SetActive(false);
        CompendiumUI.SetActive(false);
        InventoryOpenButton.SetActive(false);
        CompendiumOpenButton.SetActive(false);

        tooltipUI = ItemToolTipUI.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!FindObjectOfType<CookingSystem>().IsCookingActive())
            {
                ToggleInventory();
            }
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        SetInventoryActive(isInventoryOpen);
        InventoryOpenButton.SetActive(isInventoryOpen);
        CompendiumOpenButton.SetActive(isInventoryOpen);
        CompendiumUI.SetActive(false);

        if (!isInventoryOpen && tooltipUI != null)
        {
            tooltipUI.HideToolTip();
        }
    }
    public void OpenCompendium()
    {
        MainInventory.SetActive(false);
        CompendiumUI.SetActive(true);

        if (compendiumManager != null)
        {
            compendiumManager.OpenCompendium(ItemCategory.Gathering);
        }

        if (tooltipUI != null)
        {
            tooltipUI.HideToolTip();
        }
    }

    public void CloseCompendium()
    {
        CompendiumUI.SetActive(false);
        MainInventory.SetActive(true);

        if (tooltipUI != null)
        {
            tooltipUI.HideToolTip();
        }
    }

    public void GatheringButton()
    {
        compendiumManager.OpenCompendium(ItemCategory.Gathering);
    }

    public void CookingButton()
    {
        compendiumManager.OpenCompendium(ItemCategory.Cooking);
    }

    public void FishingButton()
    {
        compendiumManager.OpenCompendium(ItemCategory.Fishing);
    }

    public void MonsterDropButton()
    {
        compendiumManager.OpenCompendium(ItemCategory.MonsterDrop);
    }


    public void SetInventoryActive(bool isActive)
    {
        MainInventory.SetActive(isActive);
        isInventoryOpen = isActive;

        if (tooltipUI != null)
        {
            tooltipUI.HideToolTip();
        }
    }
}
