using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PopupTextInput : MonoBehaviour
{
    public GameObject okBtnObj;
    public GameObject cancelBtnObj;
    public GameObject inputFieldObj;
    private Button okBtn;
    //public Button cancelBtn;
    [SerializeField]private TMP_InputField inputField;
    private string currInput;

    void Start() {
        okBtn = okBtnObj.GetComponent<Button>();
        okBtn.onClick.AddListener(onClickOK);
        cancelBtnObj.GetComponent<Button>().onClick.AddListener(onClickCancel);

        // Disable OK button until there's something in inputField
        okBtn.interactable = false;
        inputField = inputFieldObj.GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener(checkInput);
    }

    private void checkInput(string value) {
        if (value.Length >= 1) { // 1+  character - valid input
           if (okBtn.interactable == false) {
            okBtn.interactable = true;
           }
        } else { // not valid input
            okBtn.interactable = false;
        }
    }

    private void onClickOK() {
        //Debug.Log(inputField.text);
        //BoardDataManager.Instance.NewSaveData(inputField.text); // input field value -> new save data
        selfDestruct();
    }

    private void onClickCancel() {
        selfDestruct();
    }

    public void selfDestruct() {
        Destroy(gameObject);
    }
}
