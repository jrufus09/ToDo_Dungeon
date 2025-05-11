using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskIcon : MonoBehaviour {

    public Toggle isDone;
    public TextMeshProUGUI nameLabel;

    // should have more variables but not due to time constraints

    void Awake() {
        //Debug.Log("TaskIcon was awakened");
        // toggle setup
        if (isDone == null) {
            isDone = GetComponentInChildren<Toggle>();
        }
        isDone.onValueChanged.AddListener(OnToggle);

        // label setup
        if (nameLabel == null) {
            nameLabel = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void SetName(string newName) {
        nameLabel.text = newName;

        // manually set own size because unity ui hates me
        //transform.localScale=new Vector3(1,1,1);
    }
    
    public void OnToggle(bool value) {
        BoardDataManager.Instance.EditItem(nameLabel.text, item => {
            item.isCompleted = value;
        });
        BoardDataManager.Instance.SaveData();
    }
}
