using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CompendiumItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image itemIconImage;
    private bool isCollected;
    public ItemData itemData;
    public ItemToolTipUI tooltipUI;
    private void Start()
    {
        if (tooltipUI == null)
        {
            tooltipUI = ItemToolTipUI.Instance;
        }
    }
    public void Setup(ItemData data, bool collected)
    {
        itemData = data;
        itemIconImage.sprite = data.itemIcon;

        if (collected)
        {
            SetCollected();
        }
        else
        {
            SetUncollected();
        }
    }

    public void SetCollected()
    {
        isCollected = true;
        itemIconImage.color = Color.white;
    }

    public void SetUncollected()
    {
        isCollected = false;
        itemIconImage.color = new Color(0.5f, 0.5f, 0.5f);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isCollected)
        {
            SetTooltipPosition();
            ItemToolTipUI.Instance.ShowToolTip(itemData.itemDesc, itemData.itemName);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemToolTipUI.Instance.HideToolTip();
    }

    private void SetTooltipPosition()
    {
        RectTransform itemRectTransform = GetComponent<RectTransform>();
        RectTransform tooltipRectTransform = tooltipUI.GetComponent<RectTransform>();
        RectTransform canvasRectTransform = tooltipUI.transform.parent.GetComponent<RectTransform>();

        Vector3 itemPosition = itemRectTransform.position;
        Vector3 offset = new Vector3(itemRectTransform.rect.width * 0.5f, -itemRectTransform.rect.height * 0.5f, 0);

        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            itemPosition + offset,
            null,
            out anchoredPosition
        );

        tooltipRectTransform.anchoredPosition = anchoredPosition;
    }
}