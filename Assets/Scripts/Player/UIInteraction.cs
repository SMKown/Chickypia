using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIInteraction : MonoBehaviour
{
    public static UIInteraction Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Image collection; // 수집
    public Image gathering; // 채집
    public Image cooking; // 요리
    public Image dialog; // 대화

    public Image gatherProgress;
    public GameObject interactableObj;

    private Dictionary<string, Image> tagToImageMap = new Dictionary<string, Image>();
    private float elapsedTime;
    private float animationDuration = 0.25F;
    private Coroutine currentCoroutine;

    private void Start()
    {
        // 태그와 이미지 매핑
        tagToImageMap.Add("Collectible", collection);   // 0
        tagToImageMap.Add("Gatherable", gathering);     // 1
        tagToImageMap.Add("CookingSpot", cooking);      // 2
        tagToImageMap.Add("Dialog", dialog);            // 3
        tagToImageMap.Add("Chest", collection);         
    }

    private void Update()
    {
        if (interactableObj != null && !interactableObj.activeInHierarchy)
        {
            ImageOff(tagToImageMap[interactableObj.tag]);
            interactableObj = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tagToImageMap.ContainsKey(other.tag))
        {
            interactableObj = other.gameObject;
            ImageOn(tagToImageMap[other.tag], interactableObj.transform);
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (tagToImageMap.ContainsKey(other.tag))
        {
            interactableObj = null;
            ImageOff(tagToImageMap[other.tag]);
        }
    }

    public void ImageOn(Image image, Transform targetTransform)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        image.transform.localScale = Vector3.one;
        currentCoroutine = StartCoroutine(AnimateImageON(image, targetTransform));
    }

    private IEnumerator AnimateImageON(Image image, Transform targetTransform)
    {
        if (image == null || targetTransform == null)
        {
            yield break;
        }

        image.enabled = true;

        while (image.enabled)
        {
            if (image == null || targetTransform == null)
            {
                yield break;
            }

            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetTransform.position);
            image.transform.position = screenPos;
            yield return null;
        }

        elapsedTime = 0F;
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            image.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        image.transform.localScale = Vector3.one;
    }

    public void ImageOff(Image image)
    {
        if (image == null)
        {
            return;
        }
        StartCoroutine(AnimateImageOFF(image));
    }

    private IEnumerator AnimateImageOFF(Image image)
    {
        if (image == null)
        {
            yield break;
        }
        elapsedTime = 0F;
        while (elapsedTime < animationDuration)
        {
            if (image == null)
            {
                yield break;
            }
            float t = elapsedTime / animationDuration;
            image.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (image != null)
        {
            image.transform.localScale = Vector3.zero;
            image.enabled = false;
        }
    }

    public void ShowGatherProgress(float duration)
    {
        if (gatherProgress != null)
            StartCoroutine(GatherProgress(duration));
    }

    private IEnumerator GatherProgress(float duration)
    {
        elapsedTime = 0F;
        gatherProgress.fillAmount = 0F;
        gatherProgress.enabled = true;

        while (elapsedTime < duration)
        {
            if (Input.GetKey(KeyCode.E))
            {
                elapsedTime += Time.deltaTime;
                gatherProgress.fillAmount = elapsedTime / duration;
            }
            else
            {
                HideGatherProgress();
                yield break;
            }
            yield return null;
        }
        gatherProgress.fillAmount = 1F;
    }

    public void HideGatherProgress()
    {
        gatherProgress.fillAmount = 0F;
        gatherProgress.enabled = false;
    }
}
