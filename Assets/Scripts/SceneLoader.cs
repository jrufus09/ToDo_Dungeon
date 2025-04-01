using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    public static SceneLoader Instance { get; private set; }
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

    public void OpenBoardView() {
        // on top: can close down later
        SceneManager.LoadScene("BoardView", LoadSceneMode.Additive);
    }

    public void CloseBoardView() {
        // unload? idk
    }

}