using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour {
    
    public static BoardDataManager Instance { get; private set; }

    public Button[] moveButtons;

    void Start() {

        moveButtons = new Button[3]; // first 4 items
        var btns = GameObject.FindGameObjectsWithTag("MoveButton");
        for (int i = 0; i<moveButtons.Length; i++) {
            moveButtons[i] = btns[i].GetComponent<Button>();
        }

        DisableMoveButtons(false); // enable all the buttons

    }

    public void DisableMoveButtons(bool tf = true) {
        foreach (Button btn in moveButtons) {
            btn.interactable = tf;
        }
    }

    void Update() {

        // idk why it acts like this
        if (TurnManager.Instance.waitingForPlayer == false) {
            DisableMoveButtons(false);
        } else { 
            DisableMoveButtons(true);
        }
    }

}
