using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public InventoryManager inventoryManager; // �κ��丮 �Ŵ���

    private CharacterController cc;
    private Animator animator;

    private ThirdCam thirdCam;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    public float pickupRange = 2f; // �������� �ֿ� �� �ִ� ����
    private float gravity;
    private bool tryJump = false;

    private GameObject itemInRange; // ���� ���� �ִ� ������

    public GameObject pickupImage;


    private void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        thirdCam = Camera.main.GetComponent<ThirdCam>();
    }

    private void Update()
    {
        Move();
        CheckGrounded();
        TryJump();
        CheckForItemInRange();
        if (Input.GetKeyDown(KeyCode.E) && itemInRange != null)
        {
            PickupItem();
        }

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
            animator.ResetTrigger("Jump");
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
        }

        if (tryJump)
        {
            if (PlayerInfo.Instance.isGround)
            {
                tryJump = false;
                gravity = 0;
            }
        }
    }

    private void CheckForItemInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange);

        itemInRange = null;
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Item"))
            {
                itemInRange = collider.gameObject;
                return;
            }
        }
    }

    private void PickupItem()
    {
        InventoryItem item = itemInRange.GetComponent<InventoryItem>();
        if (item != null)
        {
            // �κ��丮�� ������ �߰�
            inventoryManager.AddItem(item.GetItemData());
            // ������ ����
            Destroy(itemInRange);
            itemInRange = null;
        }
    }
}
