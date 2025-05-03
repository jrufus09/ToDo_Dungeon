using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cell;

public class Enemy : MonoBehaviour, ITurnActor {

    public float moveDuration = 0.15f; // For smooth movement later
    private Queue<Vector2Int> cachedPath = new Queue<Vector2Int>();
    private Vector2Int? lastKnownPlayerPos = null;
    private Vector2Int currentGridPos => Cell.WorldToGrid(transform.position);

    // public Transform player; // gave in and made player an instance
    // public void SetPlayer(Transform plIn) {
    //     player = plIn;
    // }

    public IEnumerator TakeTurn() {
        //Vector2Int playerGridPos = Player.Instance.coordinates;

        // recalculate path if player has moved or path is empty
        if (lastKnownPlayerPos == null || Player.Instance.coordinates != lastKnownPlayerPos || cachedPath.Count == 0) {
            // lastKnownPlayerPos = playerGridPos;

            // bool[,] walkableMap = DungeonGenerator.Instance.GetWalkableMap();
            // var path = Pathfinder.GeneratePathSync(
            //     currentGridPos.x, currentGridPos.y,
            //     playerGridPos.x, playerGridPos.y,
            //     walkableMap
            // );

            var path = Cell.PathToPlayerVec2(transform.position);

            cachedPath = new Queue<Vector2Int>(path);
            // Remove first step if it's the enemy's current tile
            if (cachedPath.Count > 0 && cachedPath.Peek() == currentGridPos) {
                cachedPath.Dequeue();
            }
        }

        // Move one tile along path
        if (cachedPath.Count > 0) {
            Vector2Int nextStep = cachedPath.Dequeue();
            yield return MoveToTile(nextStep);
        } else {
            yield return null; // wait one turn if no move
        }
    }

    private IEnumerator MoveToTile(Vector2Int targetGridPos) {
        Vector3 start = transform.position;
        Vector3 end = Cell.GridToWorldCentered(targetGridPos);
        float elapsed = 0f;

        while (elapsed < moveDuration) {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, elapsed / moveDuration);
            yield return null;
        }

        transform.position = end;
    }
}
