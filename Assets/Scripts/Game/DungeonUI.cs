using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour {
    
    public static DungeonUI Instance { get; private set; }

    private Button[] moveButtons;
    public Button left;
    public Button right;
    public Button up;
    public Button down;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start() {

        //moveButtons = new Button[3]; // first 4 items
        //var btns = GameObject.FindGameObjectsWithTag("MoveButton");
        // for (int i = 0; i<moveButtons.Length; i++) {
        //     moveButtons[i] = btns[i].GetComponent<Button>();
        // }

        moveButtons = new Button[] {up, left, right, down};

        EnableAllMoveButtons(); // enable all the buttons

    }

    public void EnableAllMoveButtons(bool tf = true) {
        foreach (Button btn in moveButtons) {
            btn.interactable = tf;
        }
    }

    // public void EnableOneMoveButton(string dir, bool tf=true) {
    //     if (dir.ToUpper() == "LEFT") {
    //         left.interactable = tf;
    //     } else if (dir.ToUpper() == "RIGHT") {
    //         right.interactable = tf;
    //     } else if (dir.ToUpper() == "UP") {
    //         up.interactable = tf;
    //     } else if (dir.ToUpper() == "DOWN") {
    //         down.interactable = tf;
    //     }
    // }

    public void EnableMoveButton(Vector2 dir, bool tf=true) {
        if (dir == new Vector2(-1, 0)) {
            left.interactable = tf;
        } else if (dir == new Vector2(1, 0)) {
            right.interactable = tf;
        } else if (dir == new Vector2(0, 1)) {
            up.interactable = tf;
        } else if (dir == new Vector2(0, -1)) {
            down.interactable = tf;
        }
    }

    void Update() {

        // if (TurnManager.Instance.waitingForPlayer == false) {
        //     DisableAllMoveButtons(true); // not waiting for player, don't let them move
        // } else { 
        //     DisableAllMoveButtons(false);
        // }
    }

}
