using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController cc;
    private Animator animator;
    
    private ThirdCam thirdCam;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    
    private float gravity;
    private bool tryJump = false;
    private bool jumpQueued = false;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        thirdCam = Camera.main.GetComponent<ThirdCam>();
    }

    private void Update()
    {
        Move();
        CheckGrounded();
        TryJump();
    }

    private void Move()
    {
        if (!PlayerInfo.Instance.shouldTurn)
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
                cc.Move(dir * moveSpeed * Time.deltaTime);
                animator.SetBool("isWalk", true);
            }
            else
                animator.SetBool("isWalk", false);
        }
    }

    private void CheckGrounded()
    {
        if (cc.isGrounded)
        {
            PlayerInfo.Instance.isGround = true;
            tryJump = false;
            gravity = 0;
            animator.ResetTrigger("Jump");

            if (jumpQueued)
            {
                jumpQueued = false;
                tryJump = true;
            }
        }
        else
        {
            PlayerInfo.Instance.isGround = false;
        }
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (PlayerInfo.Instance.isGround && !PlayerInfo.Instance.shouldAttack && !tryJump)
            {
                tryJump = true;
                gravity = jumpForce;
                animator.SetTrigger("Jump");
            }
            else if (!PlayerInfo.Instance.isGround)
            {
                jumpQueued = true;
            }
        }

        if (tryJump)
        {
            if (PlayerInfo.Instance.isGround)
            {
                cc.Move(Vector3.up * jumpForce * Time.deltaTime);
                tryJump = false;
                gravity = 0;
            }
        }
    }
}
