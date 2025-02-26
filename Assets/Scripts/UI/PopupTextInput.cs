using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupTextInput : MonoBehaviour
{
    public GameObject okBtn;
    public GameObject cancelBtn;

    void Start() {
        okBtn.GetComponent<Button>().onClick.AddListener(onClickOK);
        cancelBtn.GetComponent<Button>().onClick.AddListener(onClickCancel);
    }

    private void onClickOK() {
        Debug.Log("ok");
        selfDestruct();
    }

    private void onClickCancel() {
        selfDestruct();
    }

    public void selfDestruct() {
        Destroy(gameObject);
    }
}
