using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 10f; // 투사체 속도
    public int damage = 10; // 투사체 피해량
    private Vector3 targetPosition;

    public void Initialize(Vector3 target)
    {
        targetPosition = target;
        Destroy(gameObject, 5f); 
    }

    void Update()
    {
        
    }
}
