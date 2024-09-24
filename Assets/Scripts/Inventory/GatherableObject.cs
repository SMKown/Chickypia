using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GatherableObject : MonoBehaviour
{
    public ItemData item;
    public int gatherAmount = 1;
    public float gatherTime = 2f;
    public bool isGathering = false;
    private Coroutine gatherCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIInteraction.Instance.ShowGatherProgress(gatherTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            UIInteraction.Instance.HideGatherProgress();
            StopGathering();
        }
    }

    public void StartGathering(PlayerMovement player)
    {
        if (!isGathering)
        {
            isGathering = true;
            gatherCoroutine = StartCoroutine(Gather(player));
        }
    }
    public void StopGathering()
    {
        if (isGathering)
        {
            isGathering = false;
            if (gatherCoroutine != null)
            {
                StopCoroutine(gatherCoroutine);
                gatherCoroutine = null;
            }
            UIInteraction.Instance.gatherProgressCircle.fillAmount = 0f;
            UIInteraction.Instance.HideGatherProgress();
        }
    }

    private IEnumerator Gather(PlayerMovement player)
    {
        float elapsedTime = 0f;
        UIInteraction.Instance.ShowGatherProgress(gatherTime);

        while (elapsedTime < gatherTime)
        {
            if (!isGathering)
            {
                UIInteraction.Instance.gatherProgressCircle.fillAmount = 0f;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            UIInteraction.Instance.gatherProgressCircle.fillAmount = elapsedTime / gatherTime;
            yield return null;
        }

        player.CollectItem(item, gatherAmount);
        UIInteraction.Instance.HideGatherProgress();
        isGathering = false;
        gatherCoroutine = null;
    }
}
