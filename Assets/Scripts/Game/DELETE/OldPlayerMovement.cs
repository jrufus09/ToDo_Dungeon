//using System.Numerics;
using UnityEngine;

public class OldPlayerMovement : MonoBehaviour {
    public float moveDistance = 1f;
    public float moveSpeed = 5f;
    public float moveDuration = 0.2f;
    public bool doHop = true;

    [Header("hop stuff")]
    private Transform sprite;
    public float hopHeight = 0.2f;
    public float squishAmount = 0.2f;


    [Header("rigidbody movement")]
    private Vector3 targetPosition;
    private Vector3 posBefore; // pos for animation; rb moves separate to sprite
    private bool isMoving = false;

    private Rigidbody2D rb; // consult the rigidbody to obey physics
    private Vector2 moveDirection;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private float moveTimer;
    private float hopOffset;

    bool setupDone = false;


    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>().transform;

        // grid snapping has to be moved to after-instancing so it doesnt lock itself out of the world
    }

    public void InitiateAt(Vector2 pos) {
        Debug.Log(pos);
        transform.position = pos;
        SnapToGrid();
        setupDone = true;
    }

    public void SnapToGrid() { // CALL IT AFTER INSTANTIATED

        // Snap player to nearest tile
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x);
        pos.y = Mathf.Round(pos.y);
        transform.position = pos;
        targetPosition = transform.position;
    }

    void Update() {
        if (isMoving) {
            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            // consult rigidbody:
            //rb.MovePosition(Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime));
            // movement updated: move rb first (on parent), then sprite (child)

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

            float totalDistance = Vector3.Distance(posBefore, targetPosition);
            float currentDistance = Vector3.Distance(posBefore, transform.position);
            float moveProgress = totalDistance == 0f ? 1f : Mathf.Clamp01(currentDistance / totalDistance);

            float eased = Mathf.Sin(moveProgress * Mathf.PI); // smooth in-out bounce
            float hop = eased * hopHeight;
            sprite.localPosition = new Vector3(0f, hop, 0f);

            // squish
            float scaleY = 1f - (eased * 0.2f);  // squish vertically on max hop
            float scaleX = 1f + (eased * 0.1f);  // stretch horizontally a bit
            sprite.localScale = new Vector3(scaleX, scaleY, 1f);


        } else {
            sprite.localPosition = Vector3.zero;
        }

        // hop hop hop
        // Vector3 basePos = rb.position;
        // transform.position = new Vector3(basePos.x, basePos.y + hopOffset, 0f);
        // Vector3 visualOffset = Vector3.up * hopOffset;
        // transform.localPosition = visualOffset;
        
        //Debug.Log("Current player position: " + transform.position);
    }

    void LateUpdate() {
        // snap to grid
        if (setupDone) {

            // if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
            //     //transform.position = targetPosition; // snap exactly
            //     transform.localPosition = targetPosition;
            //     isMoving = false;
            // }

            if (isMoving) {

                // SPRITE MOVEMENT - calc rate of movement
                float totalDistance = Vector3.Distance(posBefore, targetPosition);
                float currentDistance = Vector3.Distance(posBefore, transform.position);
                float progress = totalDistance == 0f ? 1f : Mathf.Clamp01(currentDistance / totalDistance);

                float eased = Mathf.Sin(progress * Mathf.PI);
                float hop = eased * hopHeight;

                sprite.localPosition = new Vector3(0f, hop, 0f);
                float scaleY = 1f - (eased * squishAmount);
                float scaleX = 1f + (eased * squishAmount * 0.5f);
                sprite.localScale = new Vector3(scaleX, scaleY, 1f);

            } else { // stay still lol
                sprite.localPosition = Vector3.zero;
                sprite.localScale = Vector3.one;
            }
        }
    }

    void FixedUpdate() {
        if (!isMoving && moveDirection != Vector2.zero) {
            
            // Vector2 target = rb.position + moveDirection * moveDistance;
            // // Check for collision manually
            // Collider2D hit = Physics2D.OverlapBox(target, Vector2.one * 0.8f, 0f);
            // if (hit == null) {
            //     startPosition = rb.position;
            //     endPosition = target;
            //     moveTimer = 0f;
            //     isMoving = true;
            // }
            // moveDirection = Vector2.zero;

            if (isMoving) {

                // move rigidbody
                Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(newPos);

                // snap
                if (Vector2.Distance(rb.position, targetPosition) < 0.01f) {
                    rb.position = targetPosition;
                    isMoving = false;
                }
            }

        }
    }

    public void Move(Vector2 dir) {
        if (isMoving) return;
        if (Mathf.Abs(dir.x) > 0 && Mathf.Abs(dir.y) > 0) return; // no diagonals

        moveDirection = dir;

        Vector3 nextPosition = transform.position + new Vector3(dir.x * moveDistance, dir.y * moveDistance, 0f);
        posBefore = transform.position; // save starting point
        targetPosition = nextPosition;
        Debug.Log("posBefore: "+ posBefore + ", nextPosition/target: "+ targetPosition + "... now moving");
        isMoving = true;
    }

//     public void Move2(Vector2 direction) {
//         if (isMoving) return;

//         // only horizontal or vertical, meaning one value of X or Y needs to be 0
//         if (Mathf.Abs(direction.x) > 0 && Mathf.Abs(direction.y) > 0) {
//             return; // diagonal input ignored
//         }

//         if (doHop == false) {
//             Vector3 nextPosition = transform.position + new Vector3(direction.x * moveDistance, direction.y * moveDistance, 0f);

//             // clamp movement to grid boundaries
//             // float halfGridWidth = (gridWidth / 2f) * moveDistance;
//             // float halfGridHeight = (gridHeight / 2f) * moveDistance;

//             // if (nextPosition.x < -halfGridWidth || nextPosition.x > halfGridWidth ||
//             //     nextPosition.y < -halfGridHeight || nextPosition.y > halfGridHeight)
//             // {
//             //     return; // prevent moving off grid
//             // }

//             targetPosition = nextPosition;
//             isMoving = true;

//         } else {
//             Vector3 start = transform.position;
//             Vector3 end = start + new Vector3(direction.x * moveDistance, direction.y * moveDistance, 0f);

//             // respect boundaries
//             // float halfGridWidth = (gridWidth / 2f) * moveDistance;
//             // float halfGridHeight = (gridHeight / 2f) * moveDistance;
//             // if (end.x < -halfGridWidth || end.x > halfGridWidth ||
//             //     end.y < -halfGridHeight || end.y > halfGridHeight)
//             //     return;

//             // do a hop
//             StartCoroutine(HopMove(start, end));
//         }
//     }

//     private System.Collections.IEnumerator HopMove(Vector3 start, Vector3 end) {
//         isMoving = true;
//         float elapsed = 0f;

//         while (elapsed < moveDuration)
//         {
//             float t = elapsed / moveDuration;
//             // Smooth movement with sine easing
//             float heightOffset = Mathf.Sin(t * Mathf.PI) * hopHeight;
//             //transform.position = Vector3.Lerp(start, end, t) + Vector3.up * heightOffset;
//             // consult the rigidbody
//             rb.MovePosition(Vector3.Lerp(start, end, t) + Vector3.up * heightOffset);


//             elapsed += Time.deltaTime;
//             yield return null;
//         }

//         transform.position = end; // Final snap
//         isMoving = false;
//     }

}
