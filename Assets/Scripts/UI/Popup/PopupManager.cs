using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using System;

public class PopupManager : MonoBehaviour {
    public PopupTextInput newBoardPopupPrefab;
    public PopupTextInput newListPopupPrefab;
    public PopupTextInput newItemPopupPrefab;
    public PopupMessage messagePrefab;
    public static PopupManager Instance { get; private set; } // Singleton ("static"), refer to me as PopupManager.instance
    
    void Awake(){
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
                PopupTextInput x = Instantiate(newBoardPopupPrefab, this.transform);
                x.SetText(title);
                break;
            case ListData l:
                Debug.Log(l.name);
                PopupTextInput y = Instantiate(newListPopupPrefab, this.transform);
                y.SetText(title);
                break;
            case Item i:
                PopupTextInput z = Instantiate(newItemPopupPrefab, this.transform);
                z.SetText(title);
                break;
            case string s:
                if (s.ToUpper() == "BOARD") { // i love not having to nest .equals() :)
                    PopupTextInput a = Instantiate(newBoardPopupPrefab, this.transform);
                    a.SetText(title);
                } else if (s.ToUpper() == "LIST") {
                    PopupTextInput b = Instantiate(newListPopupPrefab, this.transform);
                    b.SetText(title);
                } else if (s.ToUpper() == "ITEM") {
                    PopupTextInput c = Instantiate(newItemPopupPrefab, this.transform);
                    c.SetText(title);
                }
                break;
            default:
                Debug.LogWarning("unknown type passed into ShowInputPopup()");
                break;
        }
    }

    // call it like
    //PopupManager.Instance.ShowMessagePopup("message text", DoThisWhenConfirmed);

    public void ShowMessagePopup(string title, System.Action onConfirm) {
        PopupMessage m = Instantiate(messagePrefab, this.transform);
        m.SetText(title, onConfirm); // passes the method to call next into this instance
    }
}
