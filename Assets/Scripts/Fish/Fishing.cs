using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Fishing : MonoBehaviour
{
    public GameObject Rod;
    public GameObject Bobber;
    public GameObject FxShine;
    public GameObject ExImage;
    public Transform originPos;
    public Transform[] fishPos;
    public Image fishImage;
    private Image biteImage;

    private Animator animator;
    private Animator BobberAnim;
    private NavMeshAgent navMeshAgent;

    private Fish fishtype;
    private bool nibble = false;
    private bool emoAnim = false;

    public InventoryManager inventoryManager;
    public InvenCompenUI invenCompenUI;
    private QuestManager questManager;

    private KeyCode fishingKey = KeyCode.E;
    private float maxCastDistance = 5F;

    private Vector3 initialPos;
    private Vector3 targetPos;
    private float startTime;
    private float lerpTime = 0.8F;

    private Coroutine fishingCoroutine;

    private void Start()
    {
        animator = GetComponent<Animator>();
        BobberAnim = Bobber.GetComponent<Animator>();
        biteImage = ExImage.GetComponent<Image>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        questManager = FindObjectOfType<QuestManager>();
    }

    private void Update()
    {
        HandleInput();
        HandleFishingInput();
    }

    private void HandleInput()
    {
        if (PlayerInfo.Instance.moving) return;

        if (invenCompenUI.isInventoryOpen == false)
        {
            if (Input.GetMouseButtonDown(0) && !PlayerInfo.Instance.casting && !PlayerInfo.Instance.fishing)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    MeshRenderer meshRenderer = hit.collider.GetComponent<MeshRenderer>();

                    if (meshRenderer != null && meshRenderer.material.name.Contains("Water"))
                    {
                        TryCastLine(hit.point);
                    }
                }
            }
        }        
    }

    private void TryCastLine(Vector3 hitPoint)
    {
        float distanceToHit = Vector3.Distance(originPos.position, hitPoint);
        if (distanceToHit <= maxCastDistance)
        {
            StartCasting(hitPoint);
        }
    }

    private void StartCasting(Vector3 hitPoint)
    {
        if (Bobber != null)
        {
            animator.SetBool("Break", false);
            animator.SetBool("Nice", false);
            animator.SetBool("isFishing", true);
            FxShine.SetActive(false);

            PlayerInfo.Instance.casting = true;

            targetPos = hitPoint;
            startTime = Time.time;

            RotatePlayerToTarget(hitPoint);

            navMeshAgent.enabled = false;
        }
    }

    private void RotatePlayerToTarget(Vector3 targetPoint)
    {
        Vector3 dir = targetPoint - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Euler(0, targetRot.eulerAngles.y, 0);
    }

    private void StartUpdateBobberPosition()
    {
        StartCoroutine(UpdateBobberCoroutine());
    }

    private IEnumerator UpdateBobberCoroutine()
    {
        float t = 0;
        
        initialPos = Bobber.transform.position;
        Bobber.SetActive(true);
        Bobber.transform.SetParent(null);

        while (t < 1f)
        {
            t += Time.deltaTime / lerpTime;
            Bobber.transform.position = Vector3.Slerp(initialPos, targetPos, t);
            Bobber.transform.position += new Vector3(0, Mathf.Sin(t * Mathf.PI) * 1F, 0);
            yield return null;
        }

        Bobber.transform.position = targetPos;
        CastLine();
    }

    private void CastLine()
    {
        PlayerInfo.Instance.casting = false;
        PlayerInfo.Instance.fishing = true;
        fishingCoroutine = StartCoroutine(WaitForNibble(10));
    }

    private void HandleFishingInput()
    {
        if (Input.GetKeyDown(fishingKey) && PlayerInfo.Instance.fishing)
        {
            if (PlayerInfo.Instance.casting || emoAnim) return;

            ResetBobber();

            animator.SetBool("isFishing", false);
            if (!nibble) PlayerInfo.Instance.fishing = false;
            else CatchFish();
        }
    }

    private void ResetBobber()
    {
        if (fishingCoroutine != null)
        {
            StopCoroutine(fishingCoroutine);
            fishingCoroutine = null;
        }

        Bobber.SetActive(false);
        Bobber.transform.position = originPos.position;
        Bobber.transform.SetParent(originPos);
        BobberAnim.SetBool("Bite", false);
        
        navMeshAgent.enabled = true;
    }

    private IEnumerator WaitForNibble(float maxWaitTime)
    {
        yield return new WaitForSeconds(Random.Range(maxWaitTime * 0.25F, maxWaitTime));

        biteImage.transform.position = Camera.main.WorldToScreenPoint(Bobber.transform.position);
        ExImage.SetActive(true);

        nibble = true;
        BobberAnim.SetBool("Bite", true);
        fishingCoroutine = StartCoroutine(LineBreak(1.5F));
    }


    private IEnumerator LineBreak(float lineBreakTime)
    {
        yield return new WaitForSeconds(lineBreakTime);
        nibble = false;

        ExImage.SetActive(false);

        ResetBobber();
        animator.SetBool("isFishing", false);
        animator.SetBool("Break", true);
        emoAnim = true;
    }

    private void CatchFish()
    {
        nibble = false;

        RotatePlayerToTarget(Camera.main.transform.position);

        ExImage.SetActive(false);

        animator.SetBool("Nice", true);
        emoAnim = true;

        fishtype = FishManager.GetRandomFish();

        if (transform.position.x <= -5F) FxShine.transform.position = fishPos[0].position;
        else FxShine.transform.position = fishPos[1].position;

        FxShine.SetActive(true);

        fishImage.sprite = fishtype.sprite;
        UIInteraction.Instance.ImageOn(fishImage, FxShine.transform);
        AddFish(fishtype);
        StartCoroutine(FishImageOff(1.3F));
    }

    public void AddFish(Fish fish)
    {
        ItemData fishItemData = inventoryManager.GetItemDataByName(fish.name);

        if (fishItemData != null)
        {
            inventoryManager.AddItem(fishItemData, 1);

            if (questManager != null)
            {
                foreach (var quest in questManager.questList)
                {
                    if (quest.itemId == fishItemData.itemId)
                    {
                        quest.UpdateItemCount(1);
                    }
                }
            }
        }
    }

    private IEnumerator FishImageOff(float t)
    {
        yield return new WaitForSeconds(t);
        UIInteraction.Instance.ImageOff(fishImage);
    }

    private void EmoAnimEnd()
    {
        PlayerInfo.Instance.fishing = false;
        emoAnim = false;

        navMeshAgent.enabled = true;
    }
}
