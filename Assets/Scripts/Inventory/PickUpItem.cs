using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpItem : MonoBehaviour
{
    public GameObject pickupImage;
    public float pickupRange = 2f; // �������� �ֿ� �� �ִ� ����
    private GameObject itemInRange; // ���� ���� �ִ� ������
    public InventoryManager inventoryManager; // �κ��丮 �Ŵ���

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
                pickupImage.gameObject.SetActive(true); // 'E' �ؽ�Ʈ�� Ȱ��ȭ
                return;
            }
        }
        pickupImage.gameObject.SetActive(false); // ���� ���� �������� ������ ��Ȱ��ȭ
    }

    private void PickupItem()
    {
        InventoryItem item = itemInRange.GetComponent<InventoryItem>();
        if (item != null)
        {
            // �κ��丮�� ������ �߰�
            inventoryManager.AddItem(item.GetItemData());
            // ������ ����
            Destroy(itemInRange);
            itemInRange = null;
            pickupImage.gameObject.SetActive(false); // �ؽ�Ʈ ��Ȱ��ȭ
        }
    }
}
