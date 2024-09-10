using UnityEngine;
using UnityEngine.EventSystems;
public class View : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private float rotSpeed = 0.2F;
    private bool isDrag;
    private Vector2 startDragPos;

    public void OnPointerDown(PointerEventData eventData)
    {
        isDrag = true;
        startDragPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDrag)
        {
            Vector2 currentDragPos = eventData.position;
            Vector2 dragDelta = currentDragPos - startDragPos;

            float rotationX = dragDelta.x * rotSpeed;
            CharacterBase.Instance.transform.Rotate(Vector3.up, -rotationX, Space.World);

            startDragPos = currentDragPos;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDrag = false;
    }
}