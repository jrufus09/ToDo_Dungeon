using UnityEngine;
using UnityEngine.EventSystems;

public class TouchTest : MonoBehaviour
{
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("Screen touched at: " + touch.position);
                
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    Debug.Log("UI was touched!");
                }
                else
                {
                    Debug.Log("UI was NOT touched.");
                }
            }
        }
    }
}