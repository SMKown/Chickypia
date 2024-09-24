using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController cc;
    private Animator animator;
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    private float gravity;
    private bool tryJump = false;

    public InventoryManager inventoryManager;
    private InventoryItem inventoryItem;
    private GatherableItem gatherableItem;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Move();
        CheckGround();
        TryJump();

        GetItem();
    }

    private void Move()
    {
        if (PlayerInfo.Instance.UnableMove())
        {
            animator.SetBool("isWalk", false);
            return;
        }

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

    private void CheckGround()
    {
        PlayerInfo.Instance.isGround = cc.isGrounded;
        if (cc.isGrounded) animator.ResetTrigger("Jump");
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (PlayerInfo.Instance.UnableMove()) return;

            if (PlayerInfo.Instance.isGround && !tryJump)
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

    private void GetItem()
    {
        if (UIInteraction.Instance.interactableObj != null)
        {
            if (UIInteraction.Instance.interactableObj.CompareTag("Collectible"))
                Collection(UIInteraction.Instance.interactableObj);
            else if (UIInteraction.Instance.interactableObj.CompareTag("Gatherable"))
                Gathering(UIInteraction.Instance.interactableObj);
        }
    }

    private void Collection(GameObject item)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryItem = item.GetComponent<InventoryItem>();
            
            if (inventoryItem != null && inventoryManager != null)
            {
                bool added = inventoryManager.AddItem(inventoryItem.GetItemData());
                if (added)
                {
                    Destroy(item);
                    UIInteraction.Instance.ImageOff(UIInteraction.Instance.collection);
                }
            }
        }
    }

    public void Gathering(GameObject item)
    {
        gatherableItem = item.GetComponent<GatherableItem>();

        if (gatherableItem != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                gatherableItem.StartGathering(inventoryManager);
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                gatherableItem.StopGathering();
            }
        }
    }
}
