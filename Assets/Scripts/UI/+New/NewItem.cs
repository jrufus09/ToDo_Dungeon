using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewItem : MonoBehaviour {

    ListIcon parent; // we get parent.listName, it's important

    void Awake() {
        // Check for input
        GetComponent<Button>().onClick.AddListener(OnClick);
        parent = GetComponentInParent<ListIcon>();
        // we climb
        // Transform target = transform.parent?.parent?.parent;
        // if (target != null && target.TryGetComponent(out ListIcon icon)) {
        //     parent = icon;
        // }
        //Debug.Log("+task was clicked");
    }

    public void OnClick() {
        Debug.Log("+task was clicked");
        //BoardDataManager.Instance.SetCurrentList(parent.listName);
        BoardDataManager.Instance.SetCurrentList(parent.thisList);
        PopupManager.Instance.ShowInputPopup(new Item(), "Drop your task details:", (inputText) => 
        {
            Debug.Log("return: " + inputText);
        });
        parent.RefreshTaskIcons();
    }
    

}
