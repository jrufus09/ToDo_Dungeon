using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections; // for enumerators
using UnityEngine.SceneManagement;

public class EnterDungeon : MonoBehaviour {

    Button thisbtn;
    public Board boardToSend;

    void Awake() {
        thisbtn = GetComponent<Button>();
        thisbtn.onClick.AddListener(OnClick);
    }

    void Update() {
        if (boardToSend == null) {
            thisbtn.interactable = false;
        } else {
            thisbtn.interactable = true;
        }
        
    }

    public void OnClick() {
        DontDestroyOnLoad(this.gameObject); // keep this guy surviving between scenes

        //SceneManager.sceneLoaded += OnSceneLoaded; // subscribe
        SceneLoader.Instance.OpenDungeon();

        // wait for instance to send its ready signal, then gen and set seed
        StartCoroutine(WaitForSeedGen(() => SeedGenerator.Instance != null, () => {
            SeedGenerator.Instance.GenAndSetSeed(boardToSend);
            Debug.Log($"I set the seed to {SeedGenerator.Instance.globalSeed}");
            Destroy(gameObject);
        }));

    }

    public IEnumerator WaitForSeedGen(Func<bool> condition, Action onReady) {
        yield return new WaitUntil(condition);
        onReady?.Invoke();
    }

    // void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
    //     if (scene.name == "Dungeon") {
    //         SeedGenerator.Instance.GenAndSetSeed(boardToSend);
    //         Debug.Log($"I set the seed to {SeedGenerator.Instance.globalSeed}");

    //         SceneManager.sceneLoaded -= OnSceneLoaded; // clean up
    //     }
    // }
}
