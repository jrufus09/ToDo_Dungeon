using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardView : MonoBehaviour {

    public EnterDungeon enterDungeon;

    public static BoardView Instance { get; private set; }
    void Awake() {
        if (Instance == null) {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // keep alive between scenes
        } else { Destroy(gameObject); }
    }

    public TextMeshProUGUI nameLabel;
    public Transform contentPane;
    void Start() {

        if (contentPane == null) { // if not set in inspector, get whatever game object has a content size fitter
            GameObject content = GetComponentInChildren<ContentSizeFitter>().gameObject;
            contentPane = content.transform;
        }
        if (enterDungeon == null) {
            enterDungeon = GetComponent<EnterDungeon>();
        }

        Reload();
    }

    public void Reload() {
        
        // set name of board
        nameLabel.text = BoardDataManager.Instance.currentlyOpenBoard.name;
        enterDungeon.SetBoard(BoardDataManager.Instance.currentlyOpenBoard);

        BoardDataManager.Instance.RefreshListIcons();
    }
}
