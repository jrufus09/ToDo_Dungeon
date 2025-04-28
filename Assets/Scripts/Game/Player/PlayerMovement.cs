using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 1f; // How far to move (grid size)
    public float moveSpeed = 5f;     // How fast to move

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start() {
        targetPosition = transform.position;
    }

    void Update() {
        if (!isMoving)
        {
            // Nothing happens unless you call Move(Vector2 direction) from UI buttons
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (transform.position == targetPosition)
            {
                isMoving = false;
            }
        }
    }

    public void Move(Vector2 direction) {
        if (!isMoving)
        {
            targetPosition = transform.position + new Vector3(direction.x * moveDistance, direction.y * moveDistance, 0f);
            isMoving = true;
        }
    }
}
