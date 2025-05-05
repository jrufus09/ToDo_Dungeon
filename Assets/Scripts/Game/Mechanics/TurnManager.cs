using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TurnManager : MonoBehaviour {
    public static TurnManager Instance { get; private set; } 
    private Queue<ITurnActor> actors = new Queue<ITurnActor>();
    // everyone who takes a turn needs to implement turnactor
    // Queue for turn order which is held up by the player's turn

    private bool waitingForPlayer = true;
    private bool isHandlingEnemies = false; // <- NEW

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void RegisterEnemy(ITurnActor actor) {
        if (!actors.Contains(actor)) {
            actors.Enqueue(actor);
        }
    }

    public void OnPlayerTurnCompleted() {
        if (isHandlingEnemies) return; // <- dont overlap calls
        waitingForPlayer = false;
        StartCoroutine(HandleEnemyTurns());
    }

    private IEnumerator HandleEnemyTurns() {
        int actorCount = actors.Count;

        for (int i = 0; i < actorCount; i++) {
            ITurnActor actor = actors.Dequeue();
            yield return actor.TakeTurn();
            actors.Enqueue(actor); // ready for the next round
        }

        waitingForPlayer = true;
    }

}