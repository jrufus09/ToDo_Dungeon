using UnityEngine;
using UnityEngine.UI;

public class ExitBoardView : MonoBehaviour {
   void Awake() {
        // Check for input
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

     public void OnClick() {

        // // Re-enable all canvasgroups!
        // We're going back to boardview to enable that
        SceneLoader.Instance.EnableOnly(DisableInteraction.TypeOfCanvas.ToDo);

        BoardDataManager.Instance.SetOpenBoard(null);
    }
}
