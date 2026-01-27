using UnityEngine;
using Oculus.Interaction;

public class HeadphonesGrabLockAndResume : MonoBehaviour
{
    [Header("Oculus Interaction")]
    [Tooltip("The Grabbable component on the headphones.")]
    public Grabbable grabbable;

    [Header("Hand Anchors (assign in Inspector)")]
    public Transform leftHandAnchor;
    public Transform rightHandAnchor;

    [Header("Snap Settings")]
    [Tooltip("If true, snap to the anchor (local pos/rot = 0). If false, keep current offset.")]
    public bool snapToAnchor = true;

    [Header("UI")]
    public GameObject promptRoot;

    [Header("Disable after lock (drag components here)")]
    [Tooltip("Disable these so the object can no longer be released / re-grabbed / transformed.")]
    public Behaviour[] disableAfterLock;

    [Header("Optional")]
    public InteractionToggle interactionToggle;
    public float resumeDelay = 0.05f;

    private Rigidbody rb;
    private bool locked = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("[HeadphonesGrabLockAndResume] No Rigidbody found on Headphones.");

        if (grabbable == null)
            grabbable = GetComponent<Grabbable>();
    }

    void Update()
    {
        if (locked) return;
        if (grabbable == null) return;

        // Reliable "is being grabbed" check for Oculus Interaction
        if (grabbable.SelectingPointsCount > 0)
        {
            LockToNearestHandAndResume();
        }
    }

    void LockToNearestHandAndResume()
    {
        locked = true;

        Transform target = ChooseNearestHand();
        if (target == null)
        {
            Debug.LogError("[HeadphonesGrabLockAndResume] Hand anchors not assigned.");
            locked = false;
            return;
        }

        // Parent to hand so it stays attached even after user releases the controller button
        transform.SetParent(target, worldPositionStays: !snapToAnchor);

        if (snapToAnchor)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        // Freeze physics
        if (rb != null)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector3.zero;
#else
            rb.velocity = Vector3.zero;
#endif
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Hide prompt
        if (promptRoot) promptRoot.SetActive(false);

        // Disable grab/transform scripts so Oculus doesn't "unparent" it on release
        if (disableAfterLock != null)
        {
            foreach (var b in disableAfterLock)
                if (b) b.enabled = false;
        }

        // Optionally disable other interactions once we've completed this one
        interactionToggle?.DisableInteractions();

        // Resume timeline and re-enable BOTH resume paths
        if (resumeDelay > 0f) Invoke(nameof(ResumeNarrativeInternal), resumeDelay);
        else ResumeNarrativeInternal();
    }

    Transform ChooseNearestHand()
    {
        if (leftHandAnchor == null && rightHandAnchor == null) return null;
        if (leftHandAnchor != null && rightHandAnchor == null) return leftHandAnchor;
        if (rightHandAnchor != null && leftHandAnchor == null) return rightHandAnchor;

        float dl = Vector3.Distance(transform.position, leftHandAnchor.position);
        float dr = Vector3.Distance(transform.position, rightHandAnchor.position);
        return (dl <= dr) ? leftHandAnchor : rightHandAnchor;
    }

    void ResumeNarrativeInternal()
    {
        if (NarrativeManager.Instance != null)
        {
            NarrativeManager.Instance.SetResumeModeBoth();
            NarrativeManager.Instance.ResumeNarrative();
        }
    }
}