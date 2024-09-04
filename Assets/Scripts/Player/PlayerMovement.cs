using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController cc;
    private bool isGround = true;
    private bool isJump = false;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    
    private float gravity;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
        CheckGrounded();
        TryJump();
    }

    private void Move()
    {
        if (!PlayerAbility.instance.shouldTurn)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            
            Vector3 dir = new Vector3(h, 0, v);
            gravity += Physics.gravity.y * Time.deltaTime;
            dir.y = gravity;

            if (h != 0 || v != 0)
            {
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0F, angle, 0F);
            }
            cc.Move(dir * moveSpeed * Time.deltaTime);
        }
    }

    private void CheckGrounded()
    {
        if (cc.isGrounded && !PlayerAbility.instance.shouldAttack)
        {
            isGround = true;
            isJump = false;
        }
        else isGround = false;
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround && !isJump)
                gravity = jumpForce;
    }
}
