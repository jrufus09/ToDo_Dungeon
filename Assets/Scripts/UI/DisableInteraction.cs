using UnityEngine;

public class DisableInteraction : MonoBehaviour {
    
    CanvasGroup cg;

    void Start() {
        cg = GetComponent<CanvasGroup>();
    }

    public void EnableInteractions(bool yesno = true) {
        cg.interactable = yesno;
        cg.blocksRaycasts = yesno;
    }
}
