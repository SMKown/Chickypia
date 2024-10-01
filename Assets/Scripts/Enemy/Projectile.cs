using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;
    private int damage;

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }
    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }
    public void SetSpeed(float projectileSpeed)
    {
        speed = projectileSpeed;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            HandleImpact(null);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            HandleImpact(collision.collider);
        }
    }

    private void HandleImpact(Collider collider)
    {
        if (collider != null && collider.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(damage);
            }
        }
        ProjectilePool.Instance.ReturnProjectile(gameObject);
    }
}