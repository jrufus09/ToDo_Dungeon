using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PassThroughClicks : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    // Script for the inner scrollview. attach to content
    // outer scrollview can still clicks/drags

    public ScrollRect scrollRect;

    private bool isDragging = false;

    public void OnPointerDown(PointerEventData eventData) {
        isDragging = false;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (!isDragging)
        {
            // it's a click, not a drag
            ExecuteEvents.ExecuteHierarchy(transform.gameObject, eventData, ExecuteEvents.pointerClickHandler);
        }
    }

    private void Update() {
        if (Input.GetMouseButton(0))
            isDragging = true;
    }
}
