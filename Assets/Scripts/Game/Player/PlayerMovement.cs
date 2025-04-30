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

    private Rigidbody2D rb; // consult the rigidbody to obey physics
    private Vector2 moveDirection;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private float moveTimer;

    private float hopOffset;


    void Start() {
        rb = GetComponent<Rigidbody2D>();
        targetPosition = transform.position;

        // Snap player to nearest tile
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x);
        pos.y = Mathf.Round(pos.y);
        transform.position = pos;
    }

    void Update() {
        if (isMoving) {
            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            // consult rigidbody:
            //rb.MovePosition(Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime));
            // movement updated to not need coroutine (interferes with physics)

            moveTimer += Time.deltaTime;
            float t = moveTimer / moveDuration;

            // smooth curve
            float height = Mathf.Sin(t * Mathf.PI) * hopHeight;
            hopOffset = height;

            if (t >= 1f) {
                isMoving = false;
                rb.MovePosition(endPosition);
                hopOffset = 0f;
            }

            // hop hop hop
            // Vector3 basePos = rb.position;
            // transform.position = new Vector3(basePos.x, basePos.y + hopOffset, 0f);
            Vector3 visualOffset = Vector3.up * hopOffset;
            transform.localPosition = visualOffset;


            // snap to grid
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
                transform.position = targetPosition; // snap exactly
                isMoving = false;
            }
        }
        
    }

    void FixedUpdate() {
        if (!isMoving && moveDirection != Vector2.zero) {
            Vector2 target = rb.position + moveDirection * moveDistance;

            // Check for collision
            Collider2D hit = Physics2D.OverlapBox(target, Vector2.one * 0.8f, 0f);
            if (hit == null) {
                startPosition = rb.position;
                endPosition = target;
                moveTimer = 0f;
                isMoving = true;
            }

            moveDirection = Vector2.zero;
        }
    }

    public void Move(Vector2 dir) {
        if (isMoving) return;
        if (Mathf.Abs(dir.x) > 0 && Mathf.Abs(dir.y) > 0) return; // no diagonals

        moveDirection = dir;
    }

    public void Move2(Vector2 direction) {
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
            //transform.position = Vector3.Lerp(start, end, t) + Vector3.up * heightOffset;
            // consult the rigidbody
            rb.MovePosition(Vector3.Lerp(start, end, t) + Vector3.up * heightOffset);


            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end; // Final snap
        isMoving = false;
    }

}
