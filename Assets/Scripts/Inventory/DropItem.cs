using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public Sprite itemSprite;
    public ItemData itemData;
    private SpriteRenderer spriteRenderer;

    private InventoryItem inventoryItem;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemSprite;
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        inventoryItem = GetComponent<InventoryItem>();
        if (inventoryItem != null)
        {
            inventoryItem.InitialiseItem(itemData, ItemToolTipUI.Instance);
        }
        else
        {
            Debug.LogWarning("InventoryItem 컴포넌트를 찾을 수 없습니다!");
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * 50f * Time.deltaTime);
    }
}