using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;

public class PTI_NewList : PopupTextInput {

    public override void checkInput(string value) {
        // get unique list confirmation from board data manager
        if (value.Length >= 1) { // 1+  character AND board name is unique
            //if (BoardDataManager.Instance.IsListNameUnique(BoardDataManager.Instance.currentlyOpenBoard.name, value)) { // unique = true.
            if (BoardDataManager.Instance.IsListNameUnique(value)) { // unique = true.
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
        BoardDataManager.Instance.RefreshListIcons(); // input field value -> new save data
        selfDestruct();
    }

}
