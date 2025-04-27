using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewItem : MonoBehaviour {

    ListIcon parent; // we get parent.listName, it's important

    void Awake() {
        // Check for input
        GetComponent<Button>().onClick.AddListener(OnClick);
        parent = GetComponentInParent<ListIcon>();
    }

    public void OnClick() {
        BoardDataManager.Instance.SetCurrentList(parent.listName);
        PopupManager.Instance.ShowInputPopup(new Item(), "Drop your task details:", (inputText) => 
        {
            Debug.Log("return: " + inputText);
        });
    }
    

}
