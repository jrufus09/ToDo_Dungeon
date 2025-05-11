using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour {
    
    public static DungeonUI Instance { get; private set; }

    private Button[] moveButtons;
    public Button left;
    public Button right;
    public Button up;
    public Button down;
    public Button attack;
    ButtonAttack btnatk;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start() {

        moveButtons = new Button[] {up, left, right, down};
        btnatk = attack.gameObject.GetComponent<ButtonAttack>();

        EnableAllMoveButtons(); // enable all the buttons

    }

    public void EnableAllMoveButtons(bool tf = true) {
        foreach (Button btn in moveButtons) {
            btn.interactable = tf;
        }
    }

    public void EnableMoveButton(Vector2 dir, bool tf=true) {
        if (dir == Vector2.left) {
            left.interactable = tf;
        } else if (dir == Vector2.right) {
            right.interactable = tf;
        } else if (dir == Vector2.up) {
            up.interactable = tf;
        } else if (dir == Vector2.down) {
            down.interactable = tf;
        }
    }

    // takes in a list in case more than one enemy is attackable - cue dropdown
    // but for time's sake only use the first one
    public void EnableAttackButton(List<Vector2> dir, bool tf=true) {
        btnatk.directions = dir;
        attack.interactable = true;
    }
    public void DisableAttackButton() {
        attack.interactable = false;
        btnatk.directions = null;
    }

}
