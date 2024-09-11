using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public ItemData[] itemsToPickup;

    //public GameObject itemPrefab;
    //public int numberOfItems = 10;
    //public Vector3 spawnAreaMin;
    //public Vector3 spawnAreaMax;
    //public ItemData[] itemDataArray;  // 아이템 데이터 배열 추가


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

    //        // 아이템 데이터 무작위 선택
    //        ItemData selectedItemData = itemDataArray[Random.Range(0, itemDataArray.Length)];

    //        // 아이템 생성
    //        GameObject itemObject = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);

    //        // Item 컴포넌트를 찾아서 아이템 데이터 설정
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
