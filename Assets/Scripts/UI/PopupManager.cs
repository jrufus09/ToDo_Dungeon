using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupManager : MonoBehaviour {
    public GameObject popupPrefab;
    public static PopupManager Instance { get; private set; } // Singleton ("static"), refer to me as PopupManager.instance
    
    void Awake(){
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    public void ShowInputPopup(string title, System.Action<string> onConfirm) {
        // find canvas (first and only MainCanvas tag)
        //GameObject canvas =  GameObject.FindGameObjectsWithTag("MainCanvas")[0];
        //Instantiate(popupPrefab, canvas.transform);

        // use popup manager as canvas and instantiate as own child
        Instantiate(popupPrefab, this.transform);
    }
}
