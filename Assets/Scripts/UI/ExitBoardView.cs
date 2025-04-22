using UnityEngine;
using UnityEngine.UI;

public class ExitBoardView : MonoBehaviour {
   void Awake() {
        // Check for input
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

     public void OnClick() {
        //SceneLoader.Instance.UnloadScene("BoardView");
        //Debug.Log("exiting board view");
        string objectScene = (gameObject.scene).name; // get name of current object's scene
        // Re-enable all canvasgroups!
        SceneLoader.Instance.SetInteractables(true);
        SceneLoader.Instance.UnloadScene(objectScene);
        BoardDataManager.Instance.SetOpenBoard(null);
    }
}
