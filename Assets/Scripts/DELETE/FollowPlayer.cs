using UnityEngine;

public class FollowPlayer : MonoBehaviour

// camera follow functionality moved to cinemachine
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10f);

    void Awake()
    {
        var pm = GetComponent<Player>();
        target = pm.transform;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
