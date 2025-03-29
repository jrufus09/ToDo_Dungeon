using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AddNewBoard : MonoBehaviour { // for the BUTTON

    //private BoardDataManager dataManager;
    private PopupManager popupManager;

    void Awake() {

        // assign if inspector hasn't done so
        // if (dataManager == null) {
        //     dataManager = Object.FindFirstObjectByType<BoardDataManager>();
        // }
        if (popupManager == null) {
           popupManager = Object.FindFirstObjectByType<PopupManager>();
        }

        // Check for input
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() {
        //Debug.Log("Button clicked: " + gameObject.name);
        popupManager.ShowInputPopup("Enter a board name", (inputText) => 
        {
            Debug.Log("return: " + inputText);
        });
    }
    

}
