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

    public Image exclamation; // !

    public Image gatherProgressCircle;
    public GameObject interactableObj;

    private Dictionary<string, Image> tagToImageMap = new Dictionary<string, Image>();
    private float elapsedTime;

    private float animationDuration = 0.25F;
    private Vector3 offsetPos;

    private void Start()
    {
        // 태그와 이미지 매핑
        tagToImageMap.Add("Collectible", collection);
        tagToImageMap.Add("Gatherable", gathering);
        tagToImageMap.Add("CookingSpot", cooking);
        tagToImageMap.Add("Dialog", dialog);
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
        StartCoroutine(AnimateImageON(image, targetTransform));
    }

    private IEnumerator AnimateImageON(Image image, Transform targetTransform)
    {
        image.enabled = true;

        offsetPos = targetTransform.position;
        offsetPos.y += 0.5F;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(offsetPos);
        image.transform.position = screenPos;

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
        StartCoroutine(AnimateImageOFF(image));
    }

    private IEnumerator AnimateImageOFF(Image image)
    {
        elapsedTime = 0F;
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            image.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        image.transform.localScale = Vector3.zero;

        image.enabled = false;
    }

    public void ShowGatherProgress(float duration)
    {
        if (gatherProgressCircle != null)
            StartCoroutine(GatherProgress(duration));
    }

    private IEnumerator GatherProgress(float duration)
    {
        elapsedTime = 0F;
        gatherProgressCircle.fillAmount = 0F;
        gatherProgressCircle.enabled = true;

        while (elapsedTime < duration)
        {
            if (Input.GetKey(KeyCode.E))
            {
                elapsedTime += Time.deltaTime;
                gatherProgressCircle.fillAmount = elapsedTime / duration;
            }
            else
            {
                HideGatherProgress();
                yield break;
            }
            yield return null;
        }
        gatherProgressCircle.fillAmount = 1F;
    }

    public void HideGatherProgress()
    {
        gatherProgressCircle.fillAmount = 0F;
        gatherProgressCircle.enabled = false;
    }
}
