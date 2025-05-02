using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, ITurnActor {
    public float moveDuration = 0.2f;
    public Transform player;

    public void SetPlayer(Transform plIn) {
        player = plIn;
    }

    public IEnumerator TakeTurn() {
        Vector2Int myPos = Vector2Int.RoundToInt(transform.position);
        Vector2Int playerPos = Vector2Int.RoundToInt(player.position);

        Vector2Int direction = GetStepToward(myPos, playerPos);
        Vector3 nextPos = transform.position + (Vector3)(Vector2)direction;

        yield return MoveTo(nextPos);
    }

    Vector2Int GetStepToward(Vector2Int from, Vector2Int to) {
        Vector2Int diff = to - from;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return new Vector2Int((int)Mathf.Sign(diff.x), 0);
        else
            return new Vector2Int(0, (int)Mathf.Sign(diff.y));
    }

    IEnumerator MoveTo(Vector3 target) {
        Vector3 start = transform.position;
        float t = 0f;
        while (t < 1f) {
            t += Time.deltaTime / moveDuration;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
        transform.position = target;
    }

    public void Die() {
        //EnemyHandler.Instance.UpdateDictionaryEnemyDied(transform.position)
    }
}
