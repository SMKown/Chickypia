using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab; // 아이템 프리팹
    public ItemData[] itemDatas; // 아이템 데이터 배열
    public int numberOfItems = 10; // 생성할 아이템 수
    public float spawnRange = 10f; // 아이템을 생성할 범위

    private void Start()
    {
        SpawnItems();
    }

    private void SpawnItems()
    {
        for (int i = 0; i < numberOfItems; i++)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0.5f, // 바닥 위에 놓이도록 약간의 높이 설정
                Random.Range(-spawnRange, spawnRange)
            );

            // 랜덤하게 아이템 데이터 선택
            ItemData selectedItemData = itemDatas[Random.Range(0, itemDatas.Length)];
            GameObject newItem = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);

            // InventoryItem 컴포넌트 가져오기
            InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
            if (inventoryItem != null)
            {
                inventoryItem.InitialiseItem(selectedItemData);
                // 기본 아이템 수 설정
                inventoryItem.count = 1;
                inventoryItem.ItemCount();
            }
        }
    }
}
