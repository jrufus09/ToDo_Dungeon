using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

    //public void OpenBoardView(bool setActive = false) { // this was optional before
    public void OpenBoardView() {
        // on top: can close down later
        if (boardView_loaded == false) {
            //SceneManager.LoadScene("BoardView", LoadSceneMode.Additive); // moved to LoadThenActivate
            // if this is single, make sure the event system is enabled

            // if (setActive == true) {
            //     // set the scene as active, but ONLY WHEN IT'S FINISHED LOADING
            //     Scene.isLoaded
            //     SceneManager.SetActiveScene(SceneManager.GetSceneByName("BoardView"));
            // }

            StartCoroutine(LoadThenActivate("BoardView")); // yields - so continuing means it's done
            boardView_loaded = true;

             // Make it so that the scene below cannot be interacted w temporarily
            // find all <disableinteraction> scripts in the whole game, then call disable / enable
            // moved to end

        } else {
            //Debug.Log("boardview is already loaded.");
            BoardView.Instance.Reload(); // re-set name and open lists and so on
        }
        EnableOnly(DisableInteraction.TypeOfCanvas.BoardView);
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

    // inputs: enum type of canvas (ToDo, BoardView, etc) and whether or not to enable or disable
    public void SetInteractables(DisableInteraction.TypeOfCanvas type, bool enable = true) {
        foreach (var target in GameObject.FindObjectsByType<DisableInteraction>(FindObjectsSortMode.None)) {
            target.EnableInteractions(type, enable); // if true, disables / if false, enables
        }
    }
    public void EnableOnly(DisableInteraction.TypeOfCanvas type) {
        foreach (var target in GameObject.FindObjectsByType<DisableInteraction>(FindObjectsSortMode.None)) {
            target.EnableOnly(type); // if true, disables / if false, enables
        }
    }

    //StartCoroutine(LoadThenActivate(sceneName));
    private IEnumerator LoadThenActivate(string sceneName) {

        // temp cam
        GameObject tempCam = new GameObject("TempCamera");;
        if (Camera.main == null) {
            tempCam.AddComponent<Camera>();
            DontDestroyOnLoad(tempCam);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // wait until the scene is done loading before cont - basically a while loop but better
        while (!asyncLoad.isDone) {
            yield return null;
        }

        // Scene scene = SceneManager.GetSceneByName(sceneName);
        // foreach (GameObject rootObj in scene.GetRootGameObjects()) {
        //     Debug.Log("Root object: " + rootObj.name);
        // }

        //SetNewActiveScene(sceneName);
        
        Destroy(tempCam);
    }

    public void SetNewActiveScene(string sceneName) {
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        if (loadedScene.IsValid() && loadedScene.isLoaded) {
            SceneManager.SetActiveScene(loadedScene);
            Debug.Log($"{sceneName} is now active!");
        } else {
            Debug.LogError($"{sceneName} scene failed to load :(");
        }
    }

    public void PrintSceneChildren(string sceneName) { // for debugging
        Debug.Log("hello");
        Scene scene = SceneManager.GetSceneByName(sceneName);
        foreach (GameObject rootObj in scene.GetRootGameObjects()) {
            Debug.Log("Root object: " + rootObj.name);
        }

    }

    public void OpenToDo() {
        Scene scene = SceneManager.GetSceneByName("ToDo");
        if (scene.isLoaded) {
            // persistent ui
            SceneManager.LoadScene("PersistentUI", LoadSceneMode.Additive);
            SetNewActiveScene("ToDo");
        } else {
            SceneManager.LoadScene("ToDo", LoadSceneMode.Single);
            SceneManager.LoadScene("PersistentUI", LoadSceneMode.Additive);
        }
        EnableOnly(DisableInteraction.TypeOfCanvas.ToDo);
        BoardDataManager.Instance.Start();
    }

    public void OpenDungeon() {
        Scene scene = SceneManager.GetSceneByName("Dungeon");
        if (scene.isLoaded) {
            SetNewActiveScene("Dungeon");
        } else {
            SceneManager.LoadScene("Dungeon", LoadSceneMode.Single);
        }
    }

}