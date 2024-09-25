using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public ItemData[] itemsToPickup;

    public void PickupItemTest(int id)
    {
        inventoryManager.AddItem(itemsToPickup[id]);
    }

}
