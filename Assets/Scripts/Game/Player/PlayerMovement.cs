using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float moveDistance = 1f;
    public float moveSpeed = 5f;
    public float hopHeight = 0.2f;
    public float squishAmount = 0.2f;

    public Transform spriteHolder; // Assign in Inspector

    private Rigidbody2D rb;
    private Vector3 targetPosition;
    private Vector3 positionBeforeMove;
    private bool isMoving = false;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void InitiateAt(Vector2 pos) {
        Debug.Log(pos);
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
            }
        }
    }

    void LateUpdate() {
        if (isMoving) {

            // calculate easing for squish n hop movement
            float totalDistance = Vector3.Distance(positionBeforeMove, targetPosition);
            float currentDistance = Vector3.Distance(positionBeforeMove, transform.position);
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

    public void Move(Vector2 direction) {
        if (isMoving) return;
        if (Mathf.Abs(direction.x) > 0 && Mathf.Abs(direction.y) > 0) return;

        Vector2 offset = direction * moveDistance;
        Vector2 origin = rb.position;
        Vector2 destination = origin + offset;

        // check for obstacle in the way
        RaycastHit2D[] hits = new RaycastHit2D[1];
        int hitCount = rb.Cast(direction, hits, moveDistance - 0.01f); // cast slightly short of the full move

        if (hitCount > 0) {
            // there is a thing in the way, don't move
            return;
        }

        // collisions clear, so move
        positionBeforeMove = transform.position;
        targetPosition = destination;
        isMoving = true;
    }

}
