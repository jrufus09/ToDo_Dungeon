using UnityEngine;
using UnityEngine.UI;

public class ExitBoardView : MonoBehaviour {
   void Awake() {
        // Check for input
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

     public void OnClick() {
        //SceneLoader.Instance.UnloadScene("BoardView");
        //string objectScene = (gameObject.scene).name; // get name of current object's scene
        //Debug.Log(objectScene);
        //string objectScene = "BoardView";

        // // Re-enable all canvasgroups!
        // We're going back to boardview to enable that
        //SceneLoader.Instance.SetInteractables(DisableInteraction.TypeOfCanvas.ToDo, true);
        SceneLoader.Instance.EnableOnly(DisableInteraction.TypeOfCanvas.ToDo);

        //SceneLoader.Instance.UnloadScene(objectScene);
        //SceneLoader.Instance.SetNewActiveScene(objectScene);
        BoardDataManager.Instance.SetOpenBoard(null);
    }
}
