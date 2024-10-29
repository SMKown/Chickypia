using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InvenCompenUI : MonoBehaviour
{
    public GameObject MainInventory;
    public GameObject CompendiumUI;

    public GameObject UseUI;
    public GameObject DelUI;

    public GameObject gatheringUI;
    public GameObject cookingUI;
    public GameObject fishingUI;
    public GameObject monsterDropUI;

    public GameObject InventoryOpenButton;
    public GameObject CompendiumOpenButton;

    [HideInInspector] public bool isInventoryOpen = false;

    private ItemToolTipUI tooltipUI;
    public CompendiumManager compendiumManager;

    private InventoryItem currentItem;

    private void Start()
    {
        MainInventory.SetActive(false);
        CompendiumUI.SetActive(false);
        InventoryOpenButton.SetActive(false);
        CompendiumOpenButton.SetActive(false);
        UseUI.SetActive(false);
        DelUI.SetActive(false);
        tooltipUI = ItemToolTipUI.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!FindObjectOfType<CookingSystem>().IsCookingActive() || PlayerInfo.Instance.attackMode)
            {
                ToggleInventory();
            }
        }

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            CloseCheckUI();
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

    public void SetCurrentItem(InventoryItem item)
    {
        currentItem = item;
    }

    public void CloseCheckUI()
    {
        UseUI.SetActive(false);
        DelUI.SetActive(false);
    }

    public void ConfirmUseItem()
    {
        if (currentItem != null)
        {
            currentItem.UseItemButton();
            CloseCheckUI();
        }
    }

    public void ConfirmDiscardItem()
    {
        if (currentItem != null)
        {
            currentItem.DiscardItemButton();
            CloseCheckUI();
        }
    }

    public void ShowItemOptions(ItemType itemType)
    {
        if (itemType == ItemType.Food)
        {
            UseUI.SetActive(true);
            DelUI.SetActive(true);
        }
        else
        {
            UseUI.SetActive(false);
            DelUI.SetActive(true);
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        foreach (var result in results)
        {
            if (result.gameObject == UseUI || result.gameObject == DelUI)
            {
                return true;
            }
        }
        return false;
    }
}