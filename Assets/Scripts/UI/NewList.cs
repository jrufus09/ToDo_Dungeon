using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewList : MonoBehaviour { // for the BUTTON

    void Awake() {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() {
        //Debug.Log("Button clicked: " + gameObject.name);
        PopupManager.Instance.ShowInputPopup("Enter a list name", (inputText) => 
        {
            Debug.Log("return: " + inputText);
        });
    }
    

}
