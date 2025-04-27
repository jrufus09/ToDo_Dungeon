using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PrintSceneChildren : MonoBehaviour {
   void Awake() {
        // Check for input
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

     public void OnClick() {
        // get own scene's name: print the children of that scene
        SceneLoader.Instance.PrintSceneChildren(gameObject.scene.name);
        Debug.Log(gameObject.scene.name);
    }
}
