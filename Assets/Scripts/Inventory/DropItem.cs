using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public Sprite itemSprite;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(new Vector3(Random.Range(-5f, 5f), 7f, Random.Range(-5f, 5f)), ForceMode.Impulse);
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up, 50f * Time.deltaTime, Space.World);
    }
}
