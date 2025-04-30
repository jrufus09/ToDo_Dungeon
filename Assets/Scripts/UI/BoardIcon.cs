using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BoardIcon : MonoBehaviour {

    public TextMeshProUGUI nameLabel;
    public Image boardImage;
    public string boardName;

    public void SetName(string newName) {
        boardName = newName;
        nameLabel.text = boardName;
    }

    void Awake() {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() {
        // using name of current board, get its data and send it back to board manager
        BoardDataManager.Instance.SetOpenBoard(BoardDataManager.Instance.GetBoard(boardName));

        // Open Board View scene and setup
        SceneLoader.Instance.OpenBoardView();
    }
}
