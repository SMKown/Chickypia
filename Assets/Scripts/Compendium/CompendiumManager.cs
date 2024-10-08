using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class CompendiumManager : MonoBehaviour
{
    public ItemDatabase itemDatabase;
    public GameObject compendiumItemPrefab;
    public Transform compendiumContent;
    private string compendiumFilePath;

    private void Awake()
    {
        compendiumFilePath = Application.persistentDataPath + "/compendiumData.json";
        LoadCompendium();
    }
    private void Start()
    {
        PopulateCompendium();
    }

    public void PopulateCompendium()
    {
        foreach (ItemData item in itemDatabase.allItems)
        {
            GameObject newItemEntry = Instantiate(compendiumItemPrefab, compendiumContent);
            CompendiumItemUI itemUI = newItemEntry.GetComponent<CompendiumItemUI>();

            itemUI.Setup(item, item.isCollected);
        }
    }

    public void CollectItem(ItemData collectedItem)
    {
        foreach (Transform item in compendiumContent)
        {
            CompendiumItemUI itemUI = item.GetComponent<CompendiumItemUI>();
            if (itemUI.itemData == collectedItem)
            {
                itemUI.SetCollected();
                break;
            }
        }
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
            string json = File.ReadAllText(filePath);
            CompendiumDataWrapper dataWrapper = JsonUtility.FromJson<CompendiumDataWrapper>(json);

            foreach (Transform child in compendiumContent)
            {
                Destroy(child.gameObject);
            }

            foreach (CompendiumItemData data in dataWrapper.items)
            {
                ItemData item = itemDatabase.GetItemDataByName(data.itemName);
                if (item != null)
                {
                    item.isCollected = data.isCollected;
                }
            }
        }
        else
        {
            PopulateCompendium();
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