using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 10f; // ����ü �ӵ�
    public int damage = 10; // ����ü ���ط�
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
