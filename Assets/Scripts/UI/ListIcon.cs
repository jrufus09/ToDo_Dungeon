using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ListIcon : MonoBehaviour {

    public TextMeshProUGUI nameLabel;
    private string listName;
    public Transform contentPane;
    public ListData thisList;

    public void SetName(string newName) {
        listName = newName;
        //Debug.Log("SetName hath been triggered: "+ listName);
        nameLabel.text = listName;
        thisList = BoardDataManager.Instance.GetList(listName, BoardDataManager.Instance.currentlyOpenBoard.name);
        // get boarddatamanager to find the list based off of name and the board that's currently set to open
        
        RefreshTaskIcons();
    }

    public void RefreshTaskIcons() {
        BoardDataManager.Instance.RefreshTaskIcons(contentPane, thisList);
    }


    void Awake() {

        if (contentPane == null) {
            ContentSizeFitter csf = GetComponentInChildren<ContentSizeFitter>();
            contentPane = csf.gameObject.transform;
        }
    }

    public void OnClick() {
        //
    }
}
