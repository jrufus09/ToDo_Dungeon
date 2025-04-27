using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardView : MonoBehaviour {


    //we want only one at runtime
    public static BoardView Instance { get; private set; } // Singleton ("static")
    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep alive between scenes
        } else { Destroy(gameObject); }
    }

    public TextMeshProUGUI nameLabel;
    public Transform contentPane;
    void Start() {

        if (contentPane == null) { // if not set in inspector, get whatever game object has a content size fitter
            GameObject content = GetComponentInChildren<ContentSizeFitter>().gameObject;
            contentPane = content.transform;
        }

        // set name of board
        nameLabel.text = BoardDataManager.Instance.currentlyOpenBoard.name;

        //Debug.Log(contentPane);
        BoardDataManager.Instance.LoadAllListIcons(contentPane);
    }
}
