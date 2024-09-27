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
            HandleImpact();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandleImpact();
        }
    }

    private void HandleImpact()
    { 
        // 둘중하나 사용

        //PlayerInfo playerInfo = target.GetComponent<PlayerInfo>();
        //if (playerInfo != null)
        //{
        //    playerInfo.TakeDamage(damage);
        //}

        //PlayerInfo playerInfo = collision.gameObject.GetComponent<PlayerInfo>();
        //    if (playerInfo != null)
        //    {
        //        playerInfo.TakeDamage(damage);
        //    }

        ProjectilePool pool = FindObjectOfType<ProjectilePool>();
        if (pool != null)
        {
            pool.ReturnProjectile(gameObject);
        }
    }
}