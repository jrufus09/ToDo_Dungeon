using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float moveDistance = 1f;
    public float moveSpeed = 5f;
    public int gridWidth = 10;   // x
    public int gridHeight = 10;  // y

    public float hopHeight = 0.2f;
    public float moveDuration = 0.2f;
    public bool doHop = true;

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start() {
        targetPosition = transform.position;

        // Snap player to nearest tile
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x);
        pos.y = Mathf.Round(pos.y);
        transform.position = pos;
    }

    void Update() {
        if (isMoving) {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
                transform.position = targetPosition; // snap exactly
                isMoving = false;
            }
        }
    }

    public void Move(Vector2 direction) {
        if (isMoving) return;

        // only horizontal or vertical, meaning one value of X or Y needs to be 0
        if (Mathf.Abs(direction.x) > 0 && Mathf.Abs(direction.y) > 0) {
            return; // diagonal input ignored
        }

        if (doHop == false) {
            Vector3 nextPosition = transform.position + new Vector3(direction.x * moveDistance, direction.y * moveDistance, 0f);

            // clamp movement to grid boundaries
            // float halfGridWidth = (gridWidth / 2f) * moveDistance;
            // float halfGridHeight = (gridHeight / 2f) * moveDistance;

            // if (nextPosition.x < -halfGridWidth || nextPosition.x > halfGridWidth ||
            //     nextPosition.y < -halfGridHeight || nextPosition.y > halfGridHeight)
            // {
            //     return; // prevent moving off grid
            // }

            targetPosition = nextPosition;
            isMoving = true;

        } else {
            Vector3 start = transform.position;
            Vector3 end = start + new Vector3(direction.x * moveDistance, direction.y * moveDistance, 0f);

            // respect boundaries
            // float halfGridWidth = (gridWidth / 2f) * moveDistance;
            // float halfGridHeight = (gridHeight / 2f) * moveDistance;
            // if (end.x < -halfGridWidth || end.x > halfGridWidth ||
            //     end.y < -halfGridHeight || end.y > halfGridHeight)
            //     return;

            // do a hop
            StartCoroutine(HopMove(start, end));
        }
    }

    private System.Collections.IEnumerator HopMove(Vector3 start, Vector3 end) {
        isMoving = true;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            // Smooth movement with sine easing
            float heightOffset = Mathf.Sin(t * Mathf.PI) * hopHeight;
            transform.position = Vector3.Lerp(start, end, t) + Vector3.up * heightOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end; // Final snap
        isMoving = false;
    }
}
