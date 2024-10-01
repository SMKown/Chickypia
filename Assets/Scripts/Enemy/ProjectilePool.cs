using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance { get; private set; }

    public GameObject projectilePrefab;
    private Queue<GameObject> projectilePool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetProjectile()
    {
        if (projectilePool.Count > 0)
        {
            GameObject projectile = projectilePool.Dequeue();
            projectile.SetActive(true);
            return projectile;
        }
        else
        {
            return Instantiate(projectilePrefab);
        }
    }

    public void ReturnProjectile(GameObject projectile)
    {
        projectile.transform.position = Vector3.zero;
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        projectile.SetActive(false);
        projectilePool.Enqueue(projectile);
    }
}