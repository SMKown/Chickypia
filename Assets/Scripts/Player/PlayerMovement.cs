using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public InventoryManager inventoryManager;

    private CharacterController cc;
    private Animator animator;

    private ThirdCam thirdCam;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    public float pickupRange = 2F;
    private float gravity;
    private bool tryJump = false;

    private GameObject gatherableObject;
    private GameObject pickupObject;
    private GatherableObject currentGatherScript;

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
        CheckForObjectsInRange();
        PickUpItem();
        GatherableItem();
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

    private void CheckForObjectsInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange);

        gatherableObject = null;
        pickupObject = null;
        currentGatherScript = null;

        foreach(Collider collider in colliders)
        {
            if(collider.CompareTag("Gatherable"))
            {
                gatherableObject = collider.gameObject;
                currentGatherScript = gatherableObject.GetComponent<GatherableObject>();
            }
            else if( collider.CompareTag("Item"))
            {
                pickupObject = collider.gameObject;
            }
        }
    }

    private void PickUpItem()
    {
        if (Input.GetKeyDown(KeyCode.E) && pickupObject != null)
        {
            InventoryItem itemSlot = pickupObject.GetComponent<InventoryItem>();
            if (itemSlot != null)
            {
                if (inventoryManager != null)
                {
                    inventoryManager.AddItem(itemSlot.GetItemData());
                    Destroy(pickupObject);
                    UIInteraction.Instance.ImageOff(UIInteraction.Instance.itemImage);
                }
                pickupObject = null;
            }
        }
    }
    public void GatherableItem()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentGatherScript != null)
        {
            currentGatherScript.StartGathering(this);
        }
        else if (Input.GetKeyUp(KeyCode.E) && currentGatherScript != null)
        {
            currentGatherScript.StopGathering();
        }
    }

public void CollectItem(ItemData item, int amount)
    {
        if (item != null && inventoryManager != null)
        {
            for (int i = 0; i < amount; i++)
            {
                if (!inventoryManager.AddItem(item))
                {
                    break;
                }
            }
        }
    }
}
