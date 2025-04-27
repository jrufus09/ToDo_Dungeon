using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ListIcon : MonoBehaviour {

    public TextMeshProUGUI nameLabel;
    //public Image boardImage; // background
    public string listName;
    public Transform contentPane;
    public void SetName(string newName) {
        listName = newName;
        nameLabel.text = listName;
    }

    void Awake() {
        //GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() {
        //
    }
}
