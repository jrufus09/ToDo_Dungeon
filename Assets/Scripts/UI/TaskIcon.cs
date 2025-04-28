using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskIcon : MonoBehaviour {

    public Toggle isDone;
    public TextMeshProUGUI nameLabel;

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
    
    public void OnToggle(bool value) {
        Debug.Log("toggled");
    }
}
