using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab; // ������ ������
    public ItemData[] itemDatas; // ������ ������ �迭
    public int numberOfItems = 10; // ������ ������ ��
    public float spawnRange = 10f; // �������� ������ ����

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
                0.5f, // �ٴ� ���� ���̵��� �ణ�� ���� ����
                Random.Range(-spawnRange, spawnRange)
            );

            // �����ϰ� ������ ������ ����
            ItemData selectedItemData = itemDatas[Random.Range(0, itemDatas.Length)];
            GameObject newItem = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);

            // InventoryItem ������Ʈ ��������
            InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
            if (inventoryItem != null)
            {
                inventoryItem.InitialiseItem(selectedItemData);
                // �⺻ ������ �� ����
                inventoryItem.count = 1;
                inventoryItem.ItemCount();
            }
        }
    }
}
