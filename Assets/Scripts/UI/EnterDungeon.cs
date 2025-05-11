using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections; // for enumerators
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class EnterDungeon : MonoBehaviour {

    Button thisbtn;
    public Board boardToSend;

    void Awake() {
        thisbtn = GetComponent<Button>();
        thisbtn.onClick.AddListener(OnClick);
        
    }

    public void SetBoard(Board boardin) {
        boardToSend = boardin;
    }

    void Update() {
        if (boardToSend == null) {
            thisbtn.interactable = false;
        } else {
            thisbtn.interactable = true;
        }
        
    }

    public void OnClick() {

        // detach self so not destroyed along with parents / reparent to a singleton
        transform.SetParent(BoardDataManager.Instance.gameObject.transform);
        gameObject.transform.localScale = new Vector3(0,0,0); //set self invisible
        //DontDestroyOnLoad(gameObject); // keep active between scenes

        SceneManager.sceneLoaded += OnSceneLoaded; // wait for sceneLoaded signal: --> activate OnSceneLoaded
        SceneManager.LoadScene("Dungeon", LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "Dungeon") {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Debug.Log("scene loaded, now waiting for seed gen");
            StartCoroutine(WaitForSeedGenerator());
        }
    }

    private IEnumerator WaitForSeedGenerator() {

        float timeout = 3f;
        float elapsed = 0f;

        while (SeedGenerator.Instance == null && elapsed < timeout)
        {
            Debug.Log("Waiting for SeedGenerator...");
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (SeedGenerator.Instance == null)
        {
            Debug.LogError("SeedGenerator still null after timeout!");
            yield break;
        }

        Debug.Log("SeedGenerator found!");
        SeedGenerator.Instance.GenAndSetSeed(boardToSend);
        //Destroy(gameObject); //safe to destroy
    }

}
