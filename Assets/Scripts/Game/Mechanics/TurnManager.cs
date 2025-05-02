using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class TurnManager : MonoBehaviour {
    public static TurnManager Instance { get; private set; } 
    private Queue<ITurnActor> actors = new Queue<ITurnActor>(); // everyone who takes a turn needs to implement turnactor
    // Queue for turn order which is held up by the player's turn

    public bool waitingForPlayer = true;

    void Awake() {
        Instance = this;
    }
    void Start() {
        // if (Instance == null) {
        //     Instance = this;
        //     DontDestroyOnLoad(gameObject); // keep alive between scenes
        // } else {
        //     Destroy(gameObject); // DESTROY duplicates
        // }
    }

    public void RegisterEnemy(ITurnActor actor) {
        actors.Enqueue(actor);
    }

    public void OnPlayerTurnCompleted() {
        waitingForPlayer = false;
        StartCoroutine(HandleEnemyTurns());
    }

    IEnumerator HandleEnemyTurns() {
        while (actors.Count > 0) {
            ITurnActor actor = actors.Dequeue();
            yield return actor.TakeTurn(); // async movement
            actors.Enqueue(actor);
        }

        waitingForPlayer = true;
    }


}
