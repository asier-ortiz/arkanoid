using UnityEngine;
using UnityEngine.EventSystems;

public class TouchControls : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isBeingPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        isBeingPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isBeingPressed = false;
    }
}