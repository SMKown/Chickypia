using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ItemData itemData;
    private Rigidbody rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            rb.AddForce(new Vector3(Random.Range(-10f, 10f), 15f, Random.Range(-10f, 10f)), ForceMode.Impulse);
        }

        if (itemData != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = itemData.itemIcon;
        }
    }
    void Update()
    { 
        transform.Rotate(Vector3.up, 50f * Time.deltaTime, Space.World);
    }
}
