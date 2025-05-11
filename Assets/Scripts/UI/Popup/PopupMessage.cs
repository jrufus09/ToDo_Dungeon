using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;

public class PopupMessage: MonoBehaviour {

    public GameObject okBtnObj;
    public Button okBtn;
    public TextMeshProUGUI titleLabel;
    public System.Action OnOk;
    // the caller should subscribe to:
    // thisPopup.OnOk += PostPopup;

    public void SetText(string title, System.Action onConfirm){
        if (title != null) {
            titleLabel.text = title;
        }
        OnOk = onConfirm; // save it for later
    }

    void Start() {
        okBtn = okBtnObj.GetComponent<Button>();
        okBtn.onClick.AddListener(onClickOK);
    }

    public virtual void onClickOK() {
        // call the method that was passed in
        OnOk?.Invoke();
        selfDestruct();
    }

    public void onClickCancel() {
        selfDestruct();
    }

    public void selfDestruct() {
        Destroy(gameObject);
    }
}
