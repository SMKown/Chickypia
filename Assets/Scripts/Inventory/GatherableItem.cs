using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GatherableItem : MonoBehaviour
{
    public int gatherAmount = 1;
    public float gatherTime = 2F;
    private float elapsedTime;
    public bool isGathering = false;

    private InventoryItem inventoryItem;
    private Coroutine gatherCoroutine;

    public Slider gatherSlider;

    private void Start()
    {
        inventoryItem = GetComponent<InventoryItem>();
        if (gatherSlider != null)
        {
            gatherSlider.gameObject.SetActive(false);
        }
    }

    public void StartGathering(InventoryManager inventoryManager, QuestManager questManager)
    {
        PlayerInfo.Instance.interacting = true;

        if (!isGathering)
        {
            isGathering = true;
            if (gatherSlider != null)
            {
                gatherSlider.gameObject.SetActive(true);
                gatherSlider.value = 0F;
            }
            gatherCoroutine = StartCoroutine(Gather(inventoryManager, questManager));
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

            if (gatherSlider != null)
            {
                gatherSlider.value = 0F;
                gatherSlider.gameObject.SetActive(false);
            }
            PlayerInfo.Instance.interacting = false;
        }
    }

    private IEnumerator Gather(InventoryManager inventoryManager, QuestManager questManager)
    {
        elapsedTime = 0F;
        UIInteraction.Instance.ShowGatherProgress(gatherTime);

        while (elapsedTime < gatherTime)
        {
            if (!isGathering)
            {
                if (gatherSlider != null)
                {
                    gatherSlider.value = 0F;
                    gatherSlider.gameObject.SetActive(false);
                }
                yield break;
            }

            elapsedTime += Time.deltaTime;
            if (gatherSlider != null)
            {
                gatherSlider.value = elapsedTime / gatherTime;
            }
            yield return null;
        }

        isGathering = false;
        gatherCoroutine = null;

        if (inventoryItem != null && inventoryManager != null)
        {
            bool added = inventoryManager.AddItem(inventoryItem.GetItemData());
            if (added)
            {
                UIInteraction.Instance.interactableObj = null;
                UIInteraction.Instance.ImageOff(UIInteraction.Instance.gathering);

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

        if (gatherSlider != null)
        {
            gatherSlider.gameObject.SetActive(false);
        }
        PlayerInfo.Instance.interacting = false;
    }
}