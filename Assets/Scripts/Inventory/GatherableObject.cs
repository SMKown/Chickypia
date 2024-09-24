using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherableObject : MonoBehaviour
{
    public int gatherAmount = 1;
    public float gatherTime = 2F;
    private float elapsedTime;
    public bool isGathering = false;
    
    private InventoryItem inventoryItem;
    private Coroutine gatherCoroutine;

    private void Start()
    {
        inventoryItem = GetComponent<InventoryItem>();
    }

    public void StartGathering(InventoryManager inventoryManager)
    {
        PlayerInfo.Instance.interacting = true;

        if (!isGathering)
        {
            isGathering = true;
            gatherCoroutine = StartCoroutine(Gather(inventoryManager));
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

            UIInteraction.Instance.gatherProgressCircle.fillAmount = 0F;
            UIInteraction.Instance.HideGatherProgress();
            PlayerInfo.Instance.interacting = false;
        }
    }

    private IEnumerator Gather(InventoryManager inventoryManager)
    {
        elapsedTime = 0F;
        UIInteraction.Instance.ShowGatherProgress(gatherTime);

        while (elapsedTime < gatherTime)
        {
            if (!isGathering)
            {
                UIInteraction.Instance.gatherProgressCircle.fillAmount = 0F;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            UIInteraction.Instance.gatherProgressCircle.fillAmount = elapsedTime / gatherTime;
            yield return null;
        }


        isGathering = false;
        gatherCoroutine = null;
        
        if (inventoryItem != null && inventoryManager != null)
        {
            bool added = inventoryManager.AddItem(inventoryItem.GetItemData());
            if (added)
            {
                UIInteraction.Instance.ImageOff(UIInteraction.Instance.collection);
            }
        }
        
        UIInteraction.Instance.HideGatherProgress();
        PlayerInfo.Instance.interacting = false;
    }
}
