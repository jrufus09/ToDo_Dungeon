using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using System;

public class PopupManager : MonoBehaviour {
    public GameObject newBoardPopupPrefab;
    public GameObject newListPopupPrefab;
    public GameObject newItemPopupPrefab;
    public static PopupManager Instance { get; private set; } // Singleton ("static"), refer to me as PopupManager.instance
    
    void Awake(){
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    // string that shows in the popup; delegate/lambda pass your own completion method here 
    // pass new List/new Board/etc into return type
    public void ShowInputPopup(object returnType, string title, System.Action<string> onConfirm) {
        // find canvas (first and only MainCanvas tag)
        //GameObject canvas =  GameObject.FindGameObjectsWithTag("MainCanvas")[0];
        //Instantiate(newBoardPopupPrefab, canvas.transform);
        // use popup manager as canvas and instantiate as own child

        // switch based on what we can parse returnType into
        switch (returnType) {
            case Board b:
                Debug.Log("board");
                Instantiate(newBoardPopupPrefab, this.transform);
                break;
            case ListData l:
                Debug.Log(l.name);
                Instantiate(newListPopupPrefab, this.transform);
                break;
            case Item i:
                break;
            case string s:
                if (s.ToUpper() == "BOARD") { // i love not having to nest .equals() :)
                    Instantiate(newBoardPopupPrefab, this.transform);
                } else if (s.ToUpper() == "LIST") {
                    Instantiate(newListPopupPrefab, this.transform);
                } else if (s.ToUpper() == "ITEM") {

                }
                break;
            default:
                Debug.LogWarning("unknown type passed into ShowInputPopup()");
                break;
        }



        //Instantiate(newBoardPopupPrefab, this.transform);
    }
}
