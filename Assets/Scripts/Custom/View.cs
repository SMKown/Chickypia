using UnityEngine;
using UnityEngine.EventSystems;
public class View : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private float rotationSpeed = 0.18f;
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

            float rotationX = dragDelta.x * rotationSpeed;
            CharacterBase.Instance.transform.Rotate(Vector3.up, -rotationX, Space.World);

            startDragPos = currentDragPos;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDrag = false;
    }
}