using UnityEngine;

public class ContentArea : MonoBehaviour {

    public enum TypeOfContentArea {
        Board,
        List,
        Task
    }

    public string listName;
    // there's only one board area (todo), only one list area (boardview)
    // task areas are many but i'm pretty sure i dealt with it under currentlyEditingList in boarddatamanager

    public TypeOfContentArea type; // set in inspector
    
}
