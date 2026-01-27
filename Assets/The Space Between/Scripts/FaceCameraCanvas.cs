using UnityEngine;

public class FaceCameraCanvas : MonoBehaviour
{
    Transform cam;

    void Start()
    {
        cam = Camera.main?.transform;
    }

    void LateUpdate()
    {
        if (!cam) return;

        Vector3 dir = transform.position - cam.position;

        dir.y = 0f;

        if (dir.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 8f
            );
        }
    }
}