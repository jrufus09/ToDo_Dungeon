using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;

public class PopupTextInput : MonoBehaviour {

    [Header("This is a base class: remove me from the prefab variant")]
    public GameObject okBtnObj;
    public GameObject cancelBtnObj;
    public GameObject inputFieldObj;
    public Button okBtn;
    public TMP_InputField inputField;
    private string currInput;
    public TextMeshProUGUI titleLabel;

    public void SetText(string title){
        if (title != null) {
            titleLabel.text = title;
        }
    }

    void Start() {
        okBtn = okBtnObj.GetComponent<Button>();
        okBtn.onClick.AddListener(onClickOK);
        cancelBtnObj.GetComponent<Button>().onClick.AddListener(onClickCancel);

        // Disable OK button until there's something in inputField
        okBtn.interactable = false;
        inputField = inputFieldObj.GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener(checkInput);
    }

    public virtual void checkInput(string value) {
        if (value.Length >= 1) { // 1+  character AND board name is unique
            if (BoardDataManager.Instance.IsBoardNameUnique(value)) { // unique = true.
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

    public virtual void onClickOK() {
        //Debug.Log(inputField.text);
        BoardDataManager.Instance.NewBoard(inputField.text); // input field value -> new save data
        selfDestruct();
    }

    public void onClickCancel() {
        selfDestruct();
    }

    public void selfDestruct() {
        Destroy(gameObject);
    }
}
