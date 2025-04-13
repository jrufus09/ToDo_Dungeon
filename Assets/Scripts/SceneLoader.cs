using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    public static SceneLoader Instance { get; private set; }

    // Scenes that are loaded or no
    //public Scene persistentUI;
    public bool boardView_loaded = false; // keep only one open at once

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep alive between scenes
        } else {
            Destroy(gameObject); // DESTROY duplicates
        }
    }

    void Start() {
        SceneManager.LoadScene("PersistentUI", LoadSceneMode.Additive);
        //Debug.Log("I started the navbar scene");
    }

    public void OpenBoardView(bool setActive = false) {
        // on top: can close down later
        if (boardView_loaded == false) {
            SceneManager.LoadScene("BoardView", LoadSceneMode.Additive);
            boardView_loaded = true;

            // Make it so that the scene below cannot be interacted w temporarily
            // find all <disableinteraction> scripts in the whole game, then call disable / enable
            SetInteractables(false);


            if (setActive == true) {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("BoardView"));
            }
        } else {
            Debug.Log("boardview is already loaded.");
        }
    }

    public bool UnloadScene(string sceneName) {
        bool succeeded = false;
        Scene sceneToUnload = SceneManager.GetSceneByName(sceneName);
        if (sceneToUnload.isLoaded) {
            SceneManager.UnloadSceneAsync(sceneToUnload);
            succeeded = true;

            if (sceneName == "BoardView") { // reset bool
                boardView_loaded = false;
            }
        }
        return succeeded;
    }

    public void SetInteractables(bool enable = true) {
        foreach (var target in GameObject.FindObjectsByType<DisableInteraction>(FindObjectsSortMode.None)) {
            target.EnableInteractions(enable); // if true, disables / if false, enables
        }
    }

}