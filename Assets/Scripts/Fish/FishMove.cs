using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMove : MonoBehaviour
{
    [SerializeField] private float rad = 0.8F;
    [SerializeField] private float speed = 1.4F;

    private float angle;
    private Vector3 center;

    void Start()
    {
        center = transform.position;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        angle += speed * Time.deltaTime;
        float x = Mathf.Sin(angle) * rad;
        float z = Mathf.Cos(angle) * rad; 
        Vector3 newPos = center + new Vector3(x, 0, z);

        Vector3 direction = newPos - transform.position;
        if (direction != Vector3.zero)
        {
            float rotangle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg; // 방향 각도 계산
            transform.rotation = Quaternion.Euler(0F, rotangle, 0F);
        }

        transform.position = newPos;
    }
}
