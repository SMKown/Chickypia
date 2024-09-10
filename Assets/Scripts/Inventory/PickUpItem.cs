using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpItem : MonoBehaviour
{
    public GameObject pickupImage;
    public float pickupRange = 2f; // 아이템을 주울 수 있는 범위
    private GameObject itemInRange; // 범위 내에 있는 아이템
    public InventoryManager inventoryManager; // 인벤토리 매니저

    void Update()
    {
        CheckForItemInRange();

        if (Input.GetKeyDown(KeyCode.E) && itemInRange != null)
        {
            PickupItem();
        }
    }

    private void CheckForItemInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange);

        itemInRange = null;
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Item"))
            {
                itemInRange = collider.gameObject;
                pickupImage.gameObject.SetActive(true); // 'E' 텍스트를 활성화
                return;
            }
        }
        pickupImage.gameObject.SetActive(false); // 범위 내에 아이템이 없으면 비활성화
    }

    private void PickupItem()
    {
        InventoryItem item = itemInRange.GetComponent<InventoryItem>();
        if (item != null)
        {
            // 인벤토리에 아이템 추가
            inventoryManager.AddItem(item.GetItemData());
            // 아이템 제거
            Destroy(itemInRange);
            itemInRange = null;
            pickupImage.gameObject.SetActive(false); // 텍스트 비활성화
        }
    }
}
