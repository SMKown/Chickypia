using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private int damage;
    public float lifetime = 5f;

    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }

    public void SetSpeed(float projectileSpeed)
    {
        speed = projectileSpeed;
    }
    private void Start()
    {
        StartCoroutine(DestroyLifetime());
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("A"))
        {
            HandleImpact(other.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void HandleImpact(GameObject player)
    {
        if (player != null && player.CompareTag("A"))
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(damage);
                Debug.Log("Player hit, dealing damage: " + damage);
            }
        }
        Destroy(gameObject);
    }
    private IEnumerator DestroyLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
