using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fishing : MonoBehaviour
{
    public GameObject Rod;
    public GameObject Bobber;
    public Transform originPos;

    private Animator animator;
    private Animator BobberAnim;

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
        BobberAnim = Bobber.GetComponent<Animator>();
    }

    private void Update()
    {
        HandleInput();
        HandleFishingInput();
    }

    private void HandleInput()
    {
        if (PlayerInfo.Instance.moving) return;

        if (Input.GetMouseButtonDown(0) && !PlayerInfo.Instance.casting && !PlayerInfo.Instance.fishing)
        {
            animator.SetBool("Nice", false);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                MeshRenderer meshRenderer = hit.collider.GetComponent<MeshRenderer>();

                if (meshRenderer != null && meshRenderer.material.name.Contains("Water") && !hit.collider.CompareTag("Plane"))
                {
                    TryCastLine(hit.point);
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
            PlayerInfo.Instance.casting = true;

            targetPos = hitPoint;
            startTime = Time.time;

            RotatePlayerToTarget(hitPoint);

            animator.SetBool("Break", false);
            animator.SetBool("isFishing", true);
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
        StartCoroutine(WaitForNibble(10));
    }

    private void HandleFishingInput()
    {
        if (Input.GetKeyDown(fishingKey) && PlayerInfo.Instance.fishing)
        {
            if (PlayerInfo.Instance.casting) return;
            
            ResetBobber();

            animator.SetBool("isFishing", false);
            if (!nibble) PlayerInfo.Instance.fishing = false;
            else CatchFish();
        }
    }

    private void ResetBobber()
    {
        StopAllCoroutines();
        Bobber.SetActive(false);
        Bobber.transform.position = originPos.position;
        Bobber.transform.SetParent(originPos);
        BobberAnim.SetBool("Bite", false);
    }

    private IEnumerator WaitForNibble(float maxWaitTime)
    {
        yield return new WaitForSeconds(Random.Range(maxWaitTime * 0.25F, maxWaitTime));
        UIInteraction.Instance.ImageOn(UIInteraction.Instance.exclamation, Bobber.transform);
        nibble = true;
        BobberAnim.SetBool("Bite", true);
        StartCoroutine(LineBreak(2));
    }

    private IEnumerator LineBreak(float lineBreakTime)
    {
        yield return new WaitForSeconds(lineBreakTime);
        UIInteraction.Instance.ImageOff(UIInteraction.Instance.exclamation);

        nibble = false;
        ResetBobber();
        animator.SetBool("isFishing", false);
        animator.SetBool("Break", true);
    }

    private void CatchFish()
    {
        nibble = false;

        fishtype = FishManager.GetRandomFish();
        Debug.Log($"잡은 물고기 : {fishtype.name}");

        RotatePlayerToTarget(Camera.main.transform.position);

        UIInteraction.Instance.ImageOff(UIInteraction.Instance.exclamation);
        animator.SetBool("Nice", true);
    }

    private void EmoAnimEnd()
    {
        PlayerInfo.Instance.fishing = false;
    }
}
