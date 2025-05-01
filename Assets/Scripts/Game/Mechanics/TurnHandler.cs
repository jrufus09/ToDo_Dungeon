using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class TurnManager : MonoBehaviour {
    public static TurnManager Instance;
    private Queue<ITurnActor> actors = new Queue<ITurnActor>(); // 

    void Awake() {
        Instance = this;
    }

    public void RegisterActor(ITurnActor actor) {
        actors.Enqueue(actor);
    }

    public void StartTurnCycle() {
        StartCoroutine(HandleTurns());
    }

    private IEnumerator HandleTurns() {
        while (true) {
            ITurnActor current = actors.Dequeue();
            yield return current.TakeTurn();
            actors.Enqueue(current);
        }
    }

    public void OnPlayerMoved() {
        StartTurnCycle(); // You can instead manually call this once per move
    }
}
