using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewBoard : MonoBehaviour { // for the BUTTON

    void Awake() {
        // Check for input
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() {
        //Debug.Log("Button clicked: " + gameObject.name);
        PopupManager.Instance.ShowInputPopup(new Board(), "Name your cool new board:", (inputText) => 
        {
            Debug.Log("return: " + inputText);
        });
    }
    

}
