using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ProjectilePool pool = FindObjectOfType<ProjectilePool>();
            if (pool != null)
            {
                pool.ReturnProjectile(gameObject);
            }
        }
        else
        {
            ProjectilePool pool = FindObjectOfType<ProjectilePool>();
            if (pool != null)
            {
                pool.ReturnProjectile(gameObject);
            }
        }
    }
}
