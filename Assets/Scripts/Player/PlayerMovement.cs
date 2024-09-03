using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController cc;
    private Camera cam;

    [SerializeField] private float moveSpeed;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (!PlayerAbility.instance.shouldTurn)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            
            if (h != 0 || v != 0)
            {
                Vector3 dir = new Vector3(h, 0, v).normalized;
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            
                transform.rotation = Quaternion.Euler(0F, angle, 0F);
                cc.Move(dir * moveSpeed * Time.deltaTime);
            }
        }
    }
}
