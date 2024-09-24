using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fishing : MonoBehaviour
{
    public Transform bobber;
    public Transform originPos;

    private Animator animator;

    private Fish fishtype;
    private bool nibble = false;
    [SerializeField] private GameObject thoughtBubbles;

    private KeyCode fishingKey = KeyCode.E;
    private float maxCastDistance = 5F;

    private Vector3 initialPos;
    private Vector3 targetPos;
    private float startTime;
    private float lerpTime = 0.8F;
    private bool isCasting = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        HandleInput();
        UpdateBobberPosition();
        HandleFishingInput();
    }

    private void HandleInput()
    {
        if (PlayerInfo.Instance.moving || !PlayerInfo.Instance.isGround) return;

        if (Input.GetMouseButtonDown(0) && !PlayerInfo.Instance.fishing)
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

    private void TryCastLine(Vector3 hitPoint)
    {
        Debug.Log("물 인식!");

        float distanceToHit = Vector3.Distance(originPos.position, hitPoint);
        if (distanceToHit <= maxCastDistance)
        {
            StartCasting(hitPoint);
        }
        else
        {
            Debug.Log("범위 초과");
        }
    }

    private void StartCasting(Vector3 hitPoint)
    {
        if (bobber != null)
        {
            bobber.SetParent(null);
            initialPos = bobber.position;
            targetPos = hitPoint;
            startTime = Time.time;
            isCasting = true;

            // 플레이어 회전
            RotatePlayerToTarget(hitPoint);
            animator.SetBool("isFishing", true);
        }
    }

    private void RotatePlayerToTarget(Vector3 targetPoint)
    {
        Vector3 dir = targetPoint - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Euler(0, targetRot.eulerAngles.y, 0);
    }

    private void UpdateBobberPosition()
    {
        if (isCasting)
        {
            float t = (Time.time - startTime) / lerpTime;
            if (t > 1F)
            {
                t = 1F;
                isCasting = false;
                CastLine();
            }

            bobber.position = Vector3.Slerp(initialPos, targetPos, t);
            bobber.position += new Vector3(0, Mathf.Sin(t * Mathf.PI) * 1F, 0);
        }
    }

    private void HandleFishingInput()
    {
        if (Input.GetKeyDown(fishingKey) && PlayerInfo.Instance.fishing)
        {
            if (!nibble) // 물기 전 회수
            {
                RetractLine();
            }
            else // 물고 잡음
            {
                CatchFish();
            }
        }
    }

    private void RetractLine()
    {
        StopAllCoroutines();
        PlayerInfo.Instance.fishing = false;

        thoughtBubbles.GetComponent<Animator>().SetTrigger("Reset");
        thoughtBubbles.SetActive(false);

        bobber.position = originPos.position;
        bobber.SetParent(originPos);
        animator.SetBool("isFishing", false);
    }

    private void CatchFish()
    {
        StopAllCoroutines();
        FishCaught();

        bobber.position = originPos.position;
        bobber.SetParent(originPos);
        animator.SetBool("isFishing", false);
    }

    private void CastLine()
    {
        PlayerInfo.Instance.fishing = true;
        thoughtBubbles.SetActive(true);
        StartCoroutine(WaitForNibble(10));
    }

    private IEnumerator WaitForNibble(float maxWaitTime)
    {
        yield return new WaitForSeconds(Random.Range(maxWaitTime * 0.25F, maxWaitTime));
        thoughtBubbles.GetComponent<Animator>().SetTrigger("Alert");
        nibble = true;
        StartCoroutine(LineBreak(2));
    }

    private IEnumerator LineBreak(float lineBreakTime)
    {
        yield return new WaitForSeconds(lineBreakTime);
        Debug.Log("물고기 놓침");


        thoughtBubbles.GetComponent<Animator>().SetTrigger("Reset");
        thoughtBubbles.SetActive(false);

        PlayerInfo.Instance.fishing = false;
        nibble = false;

        bobber.position = originPos.position;
        bobber.SetParent(originPos);
        animator.SetBool("isFishing", false);
    }

    public void FishCaught()
    {
        PlayerInfo.Instance.fishing = false;
        nibble = false;

        fishtype = FishManager.GetRandomFish();
        Debug.Log($"잡은 물고기 : {fishtype.name}");
        thoughtBubbles.SetActive(false);
        thoughtBubbles.GetComponent<Animator>().SetTrigger("Reset");
    }
}
