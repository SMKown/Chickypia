using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public CinemachineVirtualCamera[] virtualCameras;
    private int currentCameraIndex = 0;

    public GameObject DialogBox;
    private Image dialogImage;

    public GameObject particle;
    public float attackRange;

    private Animator animator;
    private NavMeshAgent agent;
    private Vector3 moveDirection;

    private bool Jumping = false;

    public InventoryManager inventoryManager;
    private InventoryItem inventoryItem;
    private GatherableItem gatherableItem;

    private QuestManager questManager;
    private NPC currentNpc;

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        if (DialogBox != null)
            dialogImage = DialogBox.GetComponent<Image>();

        for (int i = 0; i < virtualCameras.Length; i++)
        {
            virtualCameras[i].gameObject.SetActive(i == 0);
        }
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
            if (agent.isActiveAndEnabled) agent.ResetPath();

            animator.SetBool("isWalk", false);
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);

        agent.speed = PlayerStats.Instance.moveSpeed;

        if (h != 0 || v != 0)
        {
            agent.Move(dir * Time.deltaTime * agent.speed);
            agent.SetDestination(transform.position + dir);

            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0F, angle, 0F);

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
            int attackDamage = PlayerStats.Instance.attackDamage;

            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange);
            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    Vector3 knockbackDirection = transform.forward;
                    float knockbackForce = 4F;
                    enemy.GetComponent<Enemy>().TakeDamage(attackDamage, knockbackDirection, knockbackForce);
                }
            }
        }
    }

    private void Particle()
    {
        particle.SetActive(true);
    }

    private void AnimEnd()
    {
        if (particle != null)
            particle.SetActive(false);

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
            else if (UIInteraction.Instance.interactableObj.CompareTag("Dialog"))
                Dialog();
        }
    }

    private void Dialog()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!PlayerInfo.Instance.interacting)
            {
                PlayerInfo.Instance.interacting = true;
                ChangeCamera(UIInteraction.Instance.interactableObj.transform);

                currentNpc = UIInteraction.Instance.interactableObj.GetComponent<NPC>();
            }

            if (currentNpc != null)
                currentNpc.Interact();
        }
    }

    private void ChangeCamera(Transform npcTransform)
    {
        currentCameraIndex = 1; 
        SetActiveCamera(currentCameraIndex);

        virtualCameras[currentCameraIndex].LookAt = npcTransform;
        virtualCameras[currentCameraIndex].Follow = npcTransform;

        DialogBox.SetActive(true);
    }

    public void ResetCamera()
    {
        currentCameraIndex = 0;
        SetActiveCamera(currentCameraIndex);

        virtualCameras[currentCameraIndex].LookAt = transform;
        virtualCameras[currentCameraIndex].Follow = transform;

        StartCoroutine(FadeOutDialogBox());
    }

    private IEnumerator FadeOutDialogBox()
    {
        Color imageColor = dialogImage.color;
        float fadeDuration = 0.5F;
        float startAlpha = imageColor.a;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            imageColor.a = Mathf.Lerp(startAlpha, 0, normalizedTime);
            dialogImage.color = imageColor;
            yield return null;
        }

        DialogBox.SetActive(false);
        PlayerInfo.Instance.interacting = false;
    }

    private void SetActiveCamera(int index)
    {
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            virtualCameras[i].gameObject.SetActive(i == index);
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

                    // 수집한 아이템의 ID를 퀘스트와 비교
                    foreach (var quest in questManager.questList)
                    {
                        if (quest.itemId == inventoryItem.GetItemData().itemId)
                        {
                            quest.UpdateItemCount(1);
                        }
                    }
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
                gatherableItem.StartGathering(inventoryManager, questManager);
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                gatherableItem.StopGathering();
            }
        }
    }
}
