using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fishing : MonoBehaviour
{
    public GameObject Rod;
    public GameObject bobber;
    public Transform originPos;

    private Animator animator;
    private Animator rodAnimator;

    private Fish fishtype;
    private bool nibble = false;

    private KeyCode fishingKey = KeyCode.E;
    private float maxCastDistance = 5F;

    private Vector3 initialPos;
    private Vector3 targetPos;
    private float startTime;
    private float lerpTime = 0.8F;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rodAnimator = Rod.GetComponent<Animator>();
    }

    private void Update()
    {
        HandleInput();
        HandleFishingInput();
    }

    private void HandleInput()
    {
        if (PlayerInfo.Instance.moving || !PlayerInfo.Instance.isGround) return;

        if (Input.GetMouseButtonDown(0) && !PlayerInfo.Instance.casting && !PlayerInfo.Instance.fishing)
        {
            animator.SetBool("Nice", false);
            rodAnimator.SetBool("Nice", false);

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
        Debug.Log("물 인식");

        float distanceToHit = Vector3.Distance(originPos.position, hitPoint);
        if (distanceToHit <= maxCastDistance)
        {
            StartCasting(hitPoint);
        }
        else
            Debug.Log("범위 초과");
    }

    private void StartCasting(Vector3 hitPoint)
    {
        if (bobber != null)
        {
            PlayerInfo.Instance.casting = true;

            targetPos = hitPoint;
            startTime = Time.time;

            RotatePlayerToTarget(hitPoint);
            animator.SetBool("isFishing", true);
            rodAnimator.SetBool("Cast", true);
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
        
        initialPos = bobber.transform.position;
        bobber.SetActive(true);
        bobber.transform.SetParent(null);

        while (t < 1f)
        {
            t += Time.deltaTime / lerpTime;
            bobber.transform.position = Vector3.Slerp(initialPos, targetPos, t);
            bobber.transform.position += new Vector3(0, Mathf.Sin(t * Mathf.PI) * 1F, 0);
            yield return null;
        }

        bobber.transform.position = targetPos;
        CastLine();
    }

    private void CastLine()
    {
        PlayerInfo.Instance.casting = false;
        PlayerInfo.Instance.fishing = true;
        StartCoroutine(WaitForNibble(10));
    }

    private void HandleFishingInput()
    {
        if (Input.GetKeyDown(fishingKey) && PlayerInfo.Instance.fishing)
        {
            if (PlayerInfo.Instance.casting) return;
            ResetBobber();

            if (!nibble) PlayerInfo.Instance.fishing = false;
            else CatchFish();
        }
    }

    private void ResetBobber()
    {
        StopAllCoroutines();
        bobber.SetActive(false);
        bobber.transform.position = originPos.position;
        bobber.transform.SetParent(originPos);
        animator.SetBool("isFishing", false);
        rodAnimator.SetBool("Cast", false);
    }

    private IEnumerator WaitForNibble(float maxWaitTime)
    {
        yield return new WaitForSeconds(Random.Range(maxWaitTime * 0.25F, maxWaitTime));
        UIInteraction.Instance.ImageOn(UIInteraction.Instance.exclamation, bobber.transform);
        nibble = true;
        StartCoroutine(LineBreak(2));
    }

    private IEnumerator LineBreak(float lineBreakTime)
    {
        yield return new WaitForSeconds(lineBreakTime);
        Debug.Log("물고기 놓침");
        UIInteraction.Instance.ImageOff(UIInteraction.Instance.exclamation);

        nibble = false;
        ResetBobber();
        PlayerInfo.Instance.fishing = false;
    }

    private void CatchFish()
    {
        nibble = false;

        fishtype = FishManager.GetRandomFish();
        Debug.Log($"잡은 물고기 : {fishtype.name}");

        RotatePlayerToTarget(Camera.main.transform.position);

        UIInteraction.Instance.ImageOff(UIInteraction.Instance.exclamation);
        animator.SetBool("Nice", true);
        rodAnimator.SetBool("Nice", true);
    }

    private void NiceAnimEnd()
    {
        PlayerInfo.Instance.fishing = false;
    }
}
