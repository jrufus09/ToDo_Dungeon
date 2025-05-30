using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using static Cell;

public class Enemy : MonoBehaviour, ITurnActor {

    public Transform sprite;
    public float moveDuration = 0.15f; // For smooth movement later
    private Queue<Vector2Int> cachedPath = new Queue<Vector2Int>();
    private Vector2Int? lastKnownPlayerPos = null;
    private Vector2Int currentGridPos => Cell.WorldToGrid(transform.position);
    private Rigidbody2D rb;
    private float moveSpeed = 5f;

    public Vector2Int coordinates;
    public Vector2Int oldPosition;
    private Health health;
    public int damage = 10;


    void Start() {

        rb = GetComponent<Rigidbody2D>();

        // snap to grid!
        rb.MovePosition(Cell.GridToWorldCenteredUnity(Cell.WorldToGrid(transform.position)));


        //oordinates = Cell.WorldToGrid(transform.position);
        coordinates = Cell.PosToGrid(transform.position);
        EnemyHandler.Instance.RegisterEnemy(coordinates, this.gameObject);
        // Register in turnmanager
        TurnManager.Instance.RegisterEnemy(this);
        oldPosition = coordinates;

        health = GetComponent<Health>();
        health.OnDeath += Die; // hook up Health's onDeath action to this one

    }

    void Update() {
    }

    public IEnumerator TakeTurn() {

        Vector2Int[] path = Cell.PathToPlayerVec2(transform.position, DungeonGenerator.Instance.walkableMap);
        //Debug.Log(path[0] + ", " + path[1] + ", " + path[2]);

        if (path != null && path.Length > 1) {
            // assuming path[0] is current position and path[1] is next move,
            Vector2Int nextStep = path[1];
            Vector3 targetWorld = Cell.GridToWorldCentered(nextStep);

            // path[] is pathfinder space and player coordinates are unity space
            // convert player's position to pathfinder space and check theyre not the same
            Vector2Int playerPos = Cell.WorldToGridForPathfinder(Player.Instance.transform.position);
            //Debug.Log($"Enemy path[1]: {nextStep}, Player at: {playerPos}");

            if ((nextStep == playerPos) || (path[0] == playerPos)){
                // don't try to cosy up to player if your next step is ONTO player bro
                //Debug.Log("i attack!");

                // damage
                if (Player.Instance.TryGetComponent<Health>(out Health targetH)){
                    targetH.TakeDamage(damage);
                } else {
                    Debug.LogWarning("couldn't get health from player - check Health script is attached");
                }

                Vector2Int attackDir = nextStep - coordinates;
                yield return StartCoroutine(PlayAttackAnimation(attackDir));
                yield break; // LEAVE
            }

            oldPosition = coordinates;
            yield return StartCoroutine(SmoothMoveTo(targetWorld));
            coordinates = nextStep; // coordinates = new/current positon

            // return unity-space position
            EnemyHandler.Instance.EnemyMoved(oldPosition, Cell.PosToGrid(transform.position));
            
            coordinates = Cell.PosToGrid(transform.position);
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

    public void Die() { // this exists on health, maybe attach system action if you want animations
        // yeah me too mate
        Debug.Log("goodbye cruel worldddddd");
        EnemyHandler.Instance.EnemyDied(coordinates);
        TurnManager.Instance.UnregisterEnemy(this);
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

    public IEnumerator PlayAttackAnimation(Vector2Int direction) {
        Vector3 originalPosition = sprite.localPosition;
        Vector3 lungeOffset = new Vector3(direction.x, direction.y, 0) * 0.1f;
        Vector3 squishScale = new Vector3(1.2f, 0.8f, 1f);

        float duration = 0.2f;

        // Lunge forward with squish
        float t = 0;
        while (t < duration) {
            t += Time.deltaTime;
            float lerp = t / duration;
            sprite.localPosition = Vector3.Lerp(originalPosition, originalPosition + lungeOffset, lerp);
            sprite.localScale = Vector3.Lerp(Vector3.one, squishScale, lerp);
            yield return null;
        }

        // Snap back to original
        sprite.localPosition = originalPosition;
        sprite.localScale = Vector3.one;
    }


}
