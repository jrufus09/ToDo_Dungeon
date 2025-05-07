using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

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
    public LayerMask enemyLayer;
    public Transform sprite;
    public int damage = 40; // yeah yeah i'll move this to its own attack class
    
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
                } else {
                    Debug.Log("turnmanager is null");
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
            return;
        }

        RaycastHit2D hit2 = Physics2D.BoxCast(origin, boxSize, 0f, dir, castDistance, enemyLayer);
        if (hit2.collider != null) {
            return;
        }

        posBeforeMove = transform.position;
        targetPosition = destination;
        isMoving = true;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        //Debug.Log("Collided with: " + collision.gameObject.name);

        //CheckForObstacles();
    }

    public void CheckForObstacles() {
        float checkDistance = 0.7f; // halfway either way
        Vector2 origin = transform.position;

        // trying something new, combining layermasks
        int wallLayer = LayerMask.GetMask("Wall");
        int enemyLayer = LayerMask.GetMask("Enemy");
        int combinedMask = wallLayer | enemyLayer;

        List<Vector2> blockages = new List<Vector2>();
        List<Vector2> enemies = new List<Vector2>();

        for (int i = 0; i < directions.Length; i++) { // for each direction, check raycast
            Vector2 dir = directions[i];
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, checkDistance, combinedMask); 

            if (hit.collider != null) {
                blockages.Add(dir);
                //Debug.Log(hit.transform.gameObject.layer+", "+LayerMask.NameToLayer("Enemy"));
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                    //Debug.Log("detected enemy");
                    enemies.Add(dir);
                }
            }
        }

        // update movement buttons!
        // check list for blockages only once per method; update all directions every time
        if (blockages.Count > 0) {
            foreach(Vector2 dir2 in directions) {
                if (blockages.Contains(dir2)) {
                    //Debug.Log("blocking button in direction "+dir2);
                    DungeonUI.Instance.EnableMoveButton(dir2, false);
                } else {
                    DungeonUI.Instance.EnableMoveButton(dir2, true);
                }
            }
        } else {
            DungeonUI.Instance.EnableAllMoveButtons();
        }

        // Update attack button!
        if (enemies.Count > 0) {
            foreach(Vector2 dir3 in directions) {
                if (enemies.Contains(dir3)) {
                    //Debug.Log("enemy in direction "+dir3);
                    DungeonUI.Instance.EnableAttackButton(enemies); // button script decides who to target
                    break;
                }
            }
        } else {
            DungeonUI.Instance.DisableAttackButton();
        }
    }

    public void PlayerAttack(Vector2Int targetDir) {

        // play animation
        Vector2Int attackDir = targetDir - Player.Instance.coordinates;
        StartCoroutine(PlayAttackAnimation(attackDir));

            // get enemy in the target direction
        Vector2Int enemyPos = Player.Instance.coordinates - targetDir;
        //Debug.Log(enemyPos);
        GameObject target = EnemyHandler.Instance.GetEnemyAt(enemyPos);

        //Health targetH = target.gameObject.GetComponent<Health>();
        if (target.gameObject.TryGetComponent<Health>(out Health targetH)){
            targetH.TakeDamage(damage);
        } else {
            Debug.LogWarning("couldn't get health from enemy - check Health script is attached");
        }

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

        // this counts as a turn
        TurnManager.Instance.OnPlayerTurnCompleted();
    }

}
