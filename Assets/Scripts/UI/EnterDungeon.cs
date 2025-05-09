using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections; // for enumerators

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
        SceneLoader.Instance.OpenDungeon();

        // wait for instance to send its ready signal, then gen and set seed
        StartCoroutine(WaitForSeedGen(() => SeedGenerator.Instance != null, () => {
            SeedGenerator.Instance.GenAndSetSeed(boardToSend);
        }));
    }

    public IEnumerator WaitForSeedGen(Func<bool> condition, Action onReady) {
        yield return new WaitUntil(condition);
        onReady?.Invoke();
    }
}
