using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class CompendiumManager : MonoBehaviour
{
    public ItemDatabase itemDatabase;
    public GameObject compendiumItemPrefab;

    public GameObject gatheringUI;
    public GameObject cookingUI;
    public GameObject fishingUI;
    public GameObject monsterDropUI;

    public Transform gatheringContent;
    public Transform cookingContent;
    public Transform fishingContent;
    public Transform monsterDropContent;

    private string compendiumFilePath;
    private bool isCompendiumLoaded = false;

    private void Awake()
    {
        compendiumFilePath = Application.persistentDataPath + "/compendiumData.json";
        LoadCompendium();
    }
    private void Start()
    {
        if (!isCompendiumLoaded)
        {
            OpenCompendium(ItemCategory.Gathering);
        }
    }

    public void OpenCompendium(ItemCategory category)
    {
        Transform contentParent = null;
        GameObject activeUI = null;

        switch(category)
        {
            case ItemCategory.Gathering:
                contentParent = gatheringContent;
                activeUI = gatheringUI;
                break;
            case ItemCategory.Cooking:
                contentParent = cookingContent;
                activeUI = cookingUI;
                break;
            case ItemCategory.Fishing:
                contentParent = fishingContent;
                activeUI = fishingUI;
                break;
            case ItemCategory.MonsterDrop:
                contentParent = monsterDropContent;
                activeUI = monsterDropUI;
                break;
        }
        ActivateCategory(activeUI);
        PopulateCompendium(contentParent, category);
    }

    public void PopulateCompendium(Transform contentParent, ItemCategory? categoryFilter = null)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemData item in itemDatabase.allItems)
        {
            if (categoryFilter != null && item.category == categoryFilter)
            {
                GameObject newItemEntry = Instantiate(compendiumItemPrefab, contentParent);
                CompendiumItemUI itemUI = newItemEntry.GetComponent<CompendiumItemUI>();
                itemUI.Setup(item, item.isCollected);
            }
        }
    }

    private Transform GetCurrentContent()
    {
        if (gatheringUI.activeSelf) return gatheringContent;
        if (cookingUI.activeSelf) return cookingContent;
        if (fishingUI.activeSelf) return fishingContent;
        if (monsterDropUI.activeSelf) return monsterDropContent;
        return null;
    }

    private void ActivateCategory(GameObject activeUI)
    {
        gatheringUI.SetActive(false);
        cookingUI.SetActive(false);
        fishingUI.SetActive(false);
        monsterDropUI.SetActive(false);

        activeUI.SetActive(true);
    }

    public void CollectItem(ItemData collectedItem)
    {
        Transform currentContent = GetCurrentContent();
        if (currentContent == null) return;

        foreach (Transform item in currentContent)
        {
            CompendiumItemUI itemUI = item.GetComponent<CompendiumItemUI>();
            if (itemUI.itemData == collectedItem)
            {
                itemUI.SetCollected();
                break;
            }
        }
    }

    public void ResetCompendium()
    {
        foreach (ItemData item in itemDatabase.allItems)
        {
            item.isCollected = false;
        }

        ResetCategoryContent(gatheringContent, ItemCategory.Gathering);
        ResetCategoryContent(cookingContent, ItemCategory.Cooking);
        ResetCategoryContent(fishingContent, ItemCategory.Fishing);
        ResetCategoryContent(monsterDropContent, ItemCategory.MonsterDrop);

        if (File.Exists(compendiumFilePath))
        {
            File.Delete(compendiumFilePath);
        }
        SaveCompendium();
        LoadCompendium();
        Debug.Log("Compendium has been reset.");
    }
    private void ResetCategoryContent(Transform contentParent, ItemCategory category)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        PopulateCompendium(contentParent, category);
    }

    public void SaveCompendium()
    {
        List<CompendiumItemData> compendiumDataList = new List<CompendiumItemData>();

        foreach (ItemData item in itemDatabase.allItems)
        {
            compendiumDataList.Add(new CompendiumItemData
            {
                itemName = item.itemName,
                isCollected = item.isCollected
            });
        }

        string json = JsonUtility.ToJson(new CompendiumDataWrapper(compendiumDataList), true);
        File.WriteAllText(compendiumFilePath, json);
    }

    public void LoadCompendium()
    {
        string filePath = Application.persistentDataPath + "/compendiumData.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(compendiumFilePath);
            CompendiumDataWrapper dataWrapper = JsonUtility.FromJson<CompendiumDataWrapper>(json);

            ResetCategoryContent(gatheringContent, ItemCategory.Gathering);
            ResetCategoryContent(cookingContent, ItemCategory.Cooking);
            ResetCategoryContent(fishingContent, ItemCategory.Fishing);
            ResetCategoryContent(monsterDropContent, ItemCategory.MonsterDrop);

            foreach (CompendiumItemData data in dataWrapper.items)
            {
                ItemData item = itemDatabase.GetItemDataByName(data.itemName);
                if (item != null)
                {
                    item.isCollected = data.isCollected;
                }
            }
            PopulateCompendium(gatheringContent, ItemCategory.Gathering);
            PopulateCompendium(cookingContent, ItemCategory.Cooking);
            PopulateCompendium(fishingContent, ItemCategory.Fishing);
            PopulateCompendium(monsterDropContent, ItemCategory.MonsterDrop);
            isCompendiumLoaded = true;
        }
        else
        {
            PopulateCompendium(gatheringContent, ItemCategory.Gathering);
            PopulateCompendium(cookingContent, ItemCategory.Cooking);
            PopulateCompendium(fishingContent, ItemCategory.Fishing);
            PopulateCompendium(monsterDropContent, ItemCategory.MonsterDrop);
        }
    }
}

[System.Serializable]
public class CompendiumItemData
{
    public string itemName;
    public bool isCollected;
}

[System.Serializable]
public class CompendiumDataWrapper
{
    public List<CompendiumItemData> items;

    public CompendiumDataWrapper(List<CompendiumItemData> items)
    {
        this.items = items;
    }
}