using UnityEngine;

public class XStickyBillboard : MonoBehaviour
{
    [Header("References")]
    public Transform headCenter;
    public Transform hmd;

    [Header("Placement")]
    [Tooltip("How far from head center the X sits (approx head radius in meters).")]
    public float headRadius = 0.12f;

    [Tooltip("Extra push outward to prevent clipping into the head mesh.")]
    public float surfacePadding = 0.01f;

    [Header("Rotation")]
    [Tooltip("Keep the X upright (recommended).")]
    public bool lockY = true;

    void LateUpdate()
    {
        if (!headCenter || !hmd) return;

        Vector3 toCam = (hmd.position - headCenter.position);
        if (lockY) toCam.y = 0f;

        if (toCam.sqrMagnitude < 1e-6f) return;

        Vector3 dir = toCam.normalized;

        transform.position = headCenter.position + dir * (headRadius + surfacePadding);

        Vector3 faceDir = (transform.position - hmd.position);
        if (lockY) faceDir.y = 0f;

        transform.rotation = Quaternion.LookRotation(faceDir, Vector3.up);
    }
}