using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BoardIcon : MonoBehaviour {

    public TextMeshProUGUI nameLabel;
    public Image boardImage;
    public string boardName;

    void Start() {
        
    }

    public void Initialize() {
        nameLabel.text = boardName;
    }
}
