using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BoardIcon : MonoBehaviour {

    public TextMeshProUGUI nameLabel;
    public Image boardImage;
    public string boardName;

    public void Initialize() { // called externally but doesnt have to be
        nameLabel.text = boardName;
    }

     void Awake() {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() {
        // Open Board View scene and setup
        SceneLoader.Instance.OpenBoardView();
    }
}
