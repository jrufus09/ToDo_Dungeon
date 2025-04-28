using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewList : MonoBehaviour { // for the BUTTON

    void Awake() {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() {
        //Debug.Log("Button clicked: " + gameObject.name);
        PopupManager.Instance.ShowInputPopup("List", "Enter a name for your new list!", (inputText) => 
        {
            Debug.Log("return: " + inputText);
        });
    }
    

}
