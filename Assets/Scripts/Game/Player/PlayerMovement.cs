using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public static PlayerMovement Instance { get; private set; }

    Vector2[] directions = {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

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
    
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void InitiateAt(Vector2 pos) {
        rb = GetComponent<Rigidbody2D>(); // apparently start() didnt get it

        //Debug.Log(pos);
        //transform.position = pos;
        rb.MovePosition(pos);
        SnapToGrid();
    }

    public void SnapToGrid() {
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x);
        pos.y = Mathf.Round(pos.y);
        //transform.position = pos;
        rb.MovePosition(pos);
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
                    Debug.Log("player turn was completed");
                }

            }
        }

        // check if next move viable
        CheckForObstacles();

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

        // make a cast box smaller than the player's collider
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Vector2 boxSize = box.size * 0.95f;
        float castDistance = moveDistance * 0.95f;

        // cast a ray, see what hits wallLayer
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, dir, castDistance, wallLayer);
        if (hit.collider != null) {
            // if the raycast hit a thing, don't move.
            //Debug.Log(hit.collider);
            
            // Disable movement in that direction
            // Debug.Log(dir+ ", "+ DungeonUI.Instance.transform);
            // DungeonUI.Instance.EnableMoveButton(dir, false);

            return;
        }
        // } else {
        //     DungeonUI.Instance.EnableAllMoveButtons();
        // }

        posBeforeMove = transform.position;
        targetPosition = destination;
        isMoving = true;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log("Collided with: " + collision.gameObject.name);
    }

    public void CheckForObstacles() {
        float checkDistance = 0.5f; // halfway either way
        Vector2 origin = transform.position;

        // trying something new, combining layermasks
        int wallLayer = LayerMask.GetMask("Wall");
        int enemyLayer = LayerMask.GetMask("Enemy");
        int combinedMask = wallLayer | enemyLayer;

        for (int i = 0; i < directions.Length; i++) { // for each direction, check raycast
            Vector2 dir = directions[i];
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, checkDistance, combinedMask); 

            if (hit.collider != null) {
                //Debug.Log("blocked in direction: " + dir + " by: " + hit.collider.name);
                DungeonUI.Instance.EnableMoveButton(dir, false); // your custom function
            } else {
                DungeonUI.Instance.EnableAllMoveButtons();
            }
        }
    }


}
