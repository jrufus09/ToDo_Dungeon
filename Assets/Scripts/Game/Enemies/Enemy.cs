using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using static Cell;

public class Enemy : MonoBehaviour, ITurnActor {

    public float moveDuration = 0.15f; // For smooth movement later
    private Queue<Vector2Int> cachedPath = new Queue<Vector2Int>();
    private Vector2Int? lastKnownPlayerPos = null;
    private Vector2Int currentGridPos => Cell.WorldToGrid(transform.position);
    private Rigidbody2D rb;
    private float moveSpeed = 5f;

    public Vector2Int coordinates;

    void Start() {

        rb = GetComponent<Rigidbody2D>();

        // snap to grid!
        Vector2Int gridPos = Cell.WorldToGrid(transform.position);
        rb.MovePosition(Cell.GridToWorldCentered(gridPos));

        // Register in turnmanager
        TurnManager.Instance.RegisterEnemy(this);

    }

    void Update() {
        coordinates = Cell.WorldToGrid(transform.position);
    }

    public IEnumerator TakeTurn() {

        Vector2Int[] path = Cell.PathToPlayerVec2(transform.position, DungeonGenerator.Instance.walkableMap);
        //Debug.Log(path[0] + ", " + path[1] + ", " + path[2]);

        if (path != null && path.Length > 1) {
            // assuming path[0] is current position and path[1] is next move,
            Vector2Int nextStep = path[1];
            Vector3 targetWorld = Cell.GridToWorldCentered(nextStep);

            if (nextStep == Player.Instance.coordinates) {
                // don't try to cosy up to player if your next step is onto player
                Debug.Log("i attack!");
                yield return null;
            }

            yield return StartCoroutine(SmoothMoveTo(targetWorld));
            coordinates = nextStep;
        }

        yield return null;

        //Debug.Log("I, the enemy, am performing a turn");

    }

    private IEnumerator MoveToTile(Vector2Int targetGridPos) {
        Vector3 start = transform.position;
        Vector3 end = Cell.GridToWorldCentered(targetGridPos);
        float elapsed = 0f;

        while (elapsed < moveDuration) {
            elapsed += Time.deltaTime;
            //transform.position = Vector3.Lerp(start, end, elapsed / moveDuration);
            rb.MovePosition(Vector3.Lerp(start, end, elapsed / moveDuration));
            yield return null;
        }

        //transform.position = end;
        rb.MovePosition(end);
        Debug.Log("i have made a move!");
    }

    public void Die() {
        // yeah me too mate
        Debug.Log("goodbye cruel worldddddd");
        Destroy(gameObject);
    }

    private IEnumerator SmoothMoveTo(Vector3 target) {
        float elapsed = 0f;
        Vector3 start = transform.position;
        while (elapsed < 1f) {
            elapsed += Time.deltaTime * moveSpeed;
            rb.MovePosition(Vector3.Lerp(start, target, elapsed));
            yield return null;
        }
        rb.MovePosition(target); // snap to exact
    }

}
