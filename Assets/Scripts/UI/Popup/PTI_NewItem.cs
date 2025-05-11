using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine.UIElements.Experimental;

public class PTI_NewItem : PopupTextInput {

    public override void checkInput(string value) {
        // do nothing; no need to validate uniqueness
        okBtn.interactable = true;
    }

    public override void onClickOK() {
        //Debug.Log(inputField.text);

        // Add item
        string boardName = BoardDataManager.Instance.currentlyOpenBoard.name;
        string listName = BoardDataManager.Instance.currentlyEditingList.name;
        BoardDataManager.Instance.NewItem(boardName, listName, inputField.text, null, false); // input field value -> new save data
        
        // refresh items
        selfDestruct();
    }

}
