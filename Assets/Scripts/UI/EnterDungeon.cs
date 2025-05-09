using UnityEngine;
using UnityEngine.UI;

public class EnterDungeon : MonoBehaviour {

    Button thisbtn;
    public Board boardToSend;

    void Awake() {
        thisbtn = GetComponent<Button>();
        thisbtn.onClick.AddListener(OnClick);
    }

    void Update() {
        if (boardToSend == null) {
            thisbtn.interactable = false;
        } else {
            thisbtn.interactable = true;
        }
        
    }

    public void OnClick() {
        SceneLoader.Instance.OpenDungeon();
        SeedGenerator.Instance.GenAndSetSeed(boardToSend);
    }
}
