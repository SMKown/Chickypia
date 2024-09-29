using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject particle;
    private ParticleSystem particleSystem;

    private CharacterController cc;
    private Animator animator;
    
    [SerializeField] private float moveSpeed;

    private float gravity;
    private bool Jumping = false;

    public InventoryManager inventoryManager;
    private InventoryItem inventoryItem;
    private GatherableItem gatherableItem;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (particle != null)
            particleSystem = particle.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        Move();
        Jump();
        Attack();

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

            PlayerInfo.Instance.moving = true;
            animator.SetBool("isWalk", true);
        }
        else
        {
            PlayerInfo.Instance.moving = false;
            animator.SetBool("isWalk", false);
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (PlayerInfo.Instance.UnableMove()) return;

            if (!Jumping)
            {
                Jumping = true;
                animator.SetTrigger("Jump");
            }
        }
    }

    private void Attack()
    {
        if (!PlayerInfo.Instance.attackMode || PlayerInfo.Instance.moving || PlayerInfo.Instance.attacking) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayerInfo.Instance.attacking = true;
            animator.SetTrigger("Attack");
        }
    }

    private void Particle()
    {
        particleSystem.Play();
    }

    private void AnimEnd()
    {
        if (particleSystem != null)
            particleSystem.Clear();

        Jumping = false;
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Jump");
        PlayerInfo.Instance.attacking = false;
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
