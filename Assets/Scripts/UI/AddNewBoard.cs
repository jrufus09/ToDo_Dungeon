using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AddNewBoard : MonoBehaviour { // for the BUTTON

    void Awake() {
        // Check for input
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() {
        //Debug.Log("Button clicked: " + gameObject.name);
        PopupManager.Instance.ShowInputPopup("Enter a board name", (inputText) => 
        {
            Debug.Log("return: " + inputText);
        });
    }
    

}
