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

    public Image itemImage;
    public Image cookingImage;
    public Image gatherProgressCircle;
    private GameObject interactableObj;

    private Vector3 startScale;
    private Vector3 targetScale;
    private Vector3 targetPos;

    [SerializeField] private float animationDuration = 0.25F;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            interactableObj = other.gameObject;
            targetPos = interactableObj.transform.position;
            StartCoroutine(AnimateImageON(itemImage));
        }
        else if (other.CompareTag("Gatherable"))
        {
            interactableObj = other.gameObject;
            targetPos = interactableObj.transform.position;
            StartCoroutine(AnimateImageON(itemImage));
        }
        else if (other.CompareTag("CookingSpot"))
        {
            interactableObj = other.gameObject;
            targetPos = interactableObj.transform.position;
            StartCoroutine(AnimateImageON(cookingImage));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            interactableObj = null;
            ImageOff(itemImage);
        }
        else if (other.CompareTag("Gatherable"))
        {
            interactableObj = null;
            ImageOff(itemImage);
        }
        else if (other.CompareTag("CookingSpot"))
        {
            interactableObj = null;
            ImageOff(cookingImage);
        }
    }

    private IEnumerator AnimateImageON(Image image)
    {
        image.enabled = true;

        startScale = Vector3.zero;
        targetScale = Vector3.one;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            image.transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        image.transform.localScale = targetScale;
    }

    public void ImageOff(Image image)
    {
        StartCoroutine(AnimateImageOFF(image));
    }

    private IEnumerator AnimateImageOFF(Image image)
    {
        startScale = Vector3.one;
        targetScale = Vector3.zero;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            image.transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        image.transform.localScale = targetScale;
        image.enabled = false;
    }

    public void ShowGatherProgress(float duration)
    {
        StartCoroutine(GatherProgress(duration));
    }

    private IEnumerator GatherProgress(float duration)
    {
        float elapsedTime = 0f;
        gatherProgressCircle.fillAmount = 0f;
        gatherProgressCircle.gameObject.SetActive(true);

        while (elapsedTime < duration)
        {
            if (Input.GetKey(KeyCode.E))
            {
                elapsedTime += Time.deltaTime;
                gatherProgressCircle.fillAmount = elapsedTime / duration;
            }
            else
            {
                gatherProgressCircle.fillAmount = 0f;
                gatherProgressCircle.gameObject.SetActive(false);
                yield break;
            }

            yield return null;
        }

        gatherProgressCircle.fillAmount = 1f;
    }

    public void HideGatherProgress()
    {
        gatherProgressCircle.fillAmount = 0f;
        gatherProgressCircle.gameObject.SetActive(false);
    }

}
