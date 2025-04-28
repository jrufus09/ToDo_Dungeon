using UnityEngine;
using UnityEngine.EventSystems;

public class UIDebugLogger : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{ // Attach this guy to anything in the UI that isn't getting clicks
    public void OnPointerClick(PointerEventData eventData){
        Debug.Log($"[UI DEBUG] Clicked on {gameObject.name}");
        Debug.Log($"Current Pointer Event: {eventData.pointerCurrentRaycast.gameObject?.name}");
    }

    public void OnPointerEnter(PointerEventData eventData){
        Debug.Log($"[UI DEBUG] Pointer entered {gameObject.name}");
    }

    public void OnPointerExit(PointerEventData eventData){
        Debug.Log($"[UI DEBUG] Pointer exited {gameObject.name}");
    }
}
