using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    public float moveDistance = 1f;
    public float moveSpeed = 5f;
    public float hopHeight = 0.2f;
    public float squishAmount = 0.2f;

    public Transform spriteHolder; // Assign in Inspector

    [Header("rigidbody stuff")]
    private Rigidbody2D rb;
    private Vector3 targetPosition;
    private Vector3 posBeforeMove;
    private bool isMoving = false;
    public LayerMask wallLayer;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void InitiateAt(Vector2 pos) {
        //Debug.Log(pos);
        transform.position = pos;
        SnapToGrid();
    }

    public void SnapToGrid() {
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x);
        pos.y = Mathf.Round(pos.y);
        transform.position = pos;
        targetPosition = transform.position;
    }

    void FixedUpdate() {
        if (isMoving) {
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            if (Vector2.Distance(rb.position, targetPosition) < 0.01f) {
                rb.position = targetPosition;
                isMoving = false;

                // snap finished, notify turnhandler we've moved
                if (TurnManager.Instance != null) {
                    TurnManager.Instance.OnPlayerTurnCompleted();
                }

            }
        }
    }

    void LateUpdate() {
        if (isMoving) {

            // calculate easing for squish n hop movement
            float totalDistance = Vector3.Distance(posBeforeMove, targetPosition);
            float currentDistance = Vector3.Distance(posBeforeMove, transform.position);
            float progress = totalDistance == 0f ? 1f : Mathf.Clamp01(currentDistance / totalDistance);

            float eased = Mathf.Sin(progress * Mathf.PI);
            float hop = eased * hopHeight;

            spriteHolder.localPosition = new Vector3(0f, hop, 0f);
            float scaleY = 1f - (eased * squishAmount);
            float scaleX = 1f + (eased * squishAmount * 0.5f);
            spriteHolder.localScale = new Vector3(scaleX, scaleY, 1f);

        } else {
            spriteHolder.localPosition = Vector3.zero;
            spriteHolder.localScale = Vector3.one;
        }
    }

    public void Move(Vector2 dir) {
        if (isMoving) return;
        if (Mathf.Abs(dir.x) > 0 && Mathf.Abs(dir.y) > 0) return;
            // no diagonals - one dir only - one of x or y must be 0

        Vector2 origin = rb.position;
        Vector2 destination = origin + dir * moveDistance;

        // make a cast bos smaller than the player's collider
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Vector2 boxSize = box.size * 0.95f;
        float castDistance = moveDistance * 0.95f;

        // cast a ray, see what hits wallLayer
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, dir, castDistance, wallLayer);
        if (hit.collider != null) {
            return;
        }

        posBeforeMove = transform.position;
        targetPosition = destination;
        isMoving = true;
    }

}
