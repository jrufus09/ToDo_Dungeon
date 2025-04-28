using UnityEngine;

public class DisableInteraction : MonoBehaviour {
    
    CanvasGroup cg; // attach to canvasgroups that need to be toggled; make sure cameras are children of this node if possible

    public enum TypeOfCanvas {
        ToDo,
        BoardView
    }

    public TypeOfCanvas type; // set in inspector

    void Start() {
        cg = GetComponent<CanvasGroup>();
    }

    public void EnableInteractions(TypeOfCanvas typeIn, bool yesno = true) {
        if (typeIn == type) {
            cg.interactable = yesno;
            cg.blocksRaycasts = yesno;

            if (yesno) { // is true - so enable - alpha =1
                cg.alpha = 1;
            } else { //disappear
                cg.alpha = 0;
            }
        }
    }

    public void EnableOnly(TypeOfCanvas typeIn) {
        // enable this canvas type, and disable the canvases of every other type

        bool wasEnabled = true;
        if (typeIn == type) { // is equal, so enable this
            cg.interactable = true;
            cg.blocksRaycasts = true;
            cg.alpha = 1;

            // find any inactive cameras and enable them
            Camera cam = GetComponentInChildren<Camera>(true);
            //if (cam == null) { // attempt to find in parent}

            if (cam != null) {
                cam.gameObject.SetActive(true);   
            }

        } else { // not equal, block this
            cg.interactable = false;
            cg.blocksRaycasts = false;
            cg.alpha = 0;

            // try to find cameras and disable them
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) {
                cam.gameObject.SetActive(false);
            }
            wasEnabled = false;
        }
        //Debug.Log(type + " was enabled: " + wasEnabled);
    }
}
