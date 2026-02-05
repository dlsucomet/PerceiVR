using UnityEngine;

public class BodyFollowCamera : MonoBehaviour
{
    public Transform head;
    public float bodyHeightOffset = -1.6f;

    void LateUpdate()
    {
        if (!head) return;

        Vector3 target = head.position;

        target.y += bodyHeightOffset;

        transform.position = target;
    }
}