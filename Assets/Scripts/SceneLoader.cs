using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    void Start() {
        SceneManager.LoadScene("PersistentUI", LoadSceneMode.Additive);
        //Debug.Log("I started the navbar scene");
    }
}