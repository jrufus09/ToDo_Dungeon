using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;

public class PTI_NewList : PopupTextInput {

    // Inherited
    // public GameObject okBtnObj;
    // public GameObject cancelBtnObj;
    // public GameObject inputFieldObj;
    // private Button okBtn;
    // //public Button cancelBtn;
    // [SerializeField]private TMP_InputField inputField;
    // private string currInput;
    // start...

    public override void checkInput(string value) {
        // get unique list confirmation from board data manager
        if (value.Length >= 1) { // 1+  character AND board name is unique
            if (BoardDataManager.Instance.IsListNameUnique(BoardDataManager.Instance.currentlyOpenBoard.name, value)) { // unique = true.
                if (okBtn.interactable == false) {
                    okBtn.interactable = true;
                }
            } else {
                okBtn.interactable = false;
            }
        } else { // not valid input
            okBtn.interactable = false;
        }
    }

    public override void onClickOK() {
        //Debug.Log(inputField.text);
        BoardDataManager.Instance.NewList(BoardDataManager.Instance.currentlyOpenBoard.name, inputField.text); // input field value -> new save data
        selfDestruct();
    }

    // inherited
    // private void onClickCancel() { selfDestruct(); }
    // public void selfDestruct() { Destroy(gameObject);}

}
