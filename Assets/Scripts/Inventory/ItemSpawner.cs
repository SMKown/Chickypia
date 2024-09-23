using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public ItemData[] itemsToPickup;

    //public GameObject itemPrefab;
    //public int numberOfItems = 10;
    //public Vector3 spawnAreaMin;
    //public Vector3 spawnAreaMax;
    //public ItemData[] itemDataArray;


    //private void Start()
    //{
    //    SpawnItems();
    //}

    //private void SpawnItems()
    //{
    //    for (int i = 0; i < numberOfItems; i++)
    //    {
    //        Vector3 spawnPosition = new Vector3(
    //            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
    //            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
    //            Random.Range(spawnAreaMin.z, spawnAreaMax.z));

    //        ItemData selectedItemData = itemDataArray[Random.Range(0, itemDataArray.Length)];

    //        GameObject itemObject = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);

    //        Item itemComponent = itemObject.GetComponent<Item>();
    //        if (itemComponent != null)
    //        {
    //            itemComponent.itemData = selectedItemData;
    //        }
    //    }
    //}

    public void PickupItemTest(int id)
    {
        inventoryManager.AddItem(itemsToPickup[id]);
    }

}
