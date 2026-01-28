using UnityEngine;
using Oculus.Interaction;
using UnityEngine.UI;

public class PenGrabHoldAndSnapBack : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Looping sound while the pen is being held (AudioSource: Loop ON, Play On Awake OFF).")]
    public AudioSource holdLoopSource;

    [Header("Oculus Interaction")]
    public Grabbable grabbable;

    [Tooltip("Disable these on completion to force the pen to release (recommended: HandGrabInteractable + GrabInteractable on ISDK_HandGrabInteraction).")]
    public Behaviour[] disableOnComplete;

    [Header("Hold Points (assign these)")]
    public Transform leftHoldPoint;
    public Transform rightHoldPoint;

    [Header("UI (optional)")]
    [Tooltip("Set this to the TOP PromptCanvas root (not a child).")]
    public GameObject promptCanvasRoot;
    public Image progressRing;

    [Header("Timing")]
    public float holdSeconds = 3.0f;
    public bool useUnscaledTime = true;

    [Header("Snap Back (Table)")]
    public Transform tableSnapPoint;

    [Header("Optional")]
    public InteractionToggle interactionToggle;
    public float resumeDelay = 0.05f;

    Rigidbody rb;
    Vector3 tablePos;
    Quaternion tableRot;

    Vector3 originalWorldScale;

    float heldTime = 0f;
    bool armed = false;
    bool completed = false;

    bool wasGrabbed = false;
    Transform currentHoldPoint;

    void Awake()
    {
        if (grabbable == null) grabbable = GetComponent<Grabbable>();
        rb = GetComponent<Rigidbody>();

        originalWorldScale = transform.lossyScale;

        if (tableSnapPoint != null)
        {
            tablePos = tableSnapPoint.position;
            tableRot = tableSnapPoint.rotation;
        }
        else
        {
            tablePos = transform.position;
            tableRot = transform.rotation;
        }

        if (progressRing) progressRing.fillAmount = 0f;
        if (promptCanvasRoot) promptCanvasRoot.SetActive(false);

        if (holdLoopSource != null)
        {
            holdLoopSource.loop = true;
            holdLoopSource.playOnAwake = false;
            holdLoopSource.Stop();
        }
    }

    public void Arm()
    {
        armed = true;
        completed = false;
        heldTime = 0f;

        if (progressRing) progressRing.fillAmount = 0f;
        if (promptCanvasRoot) promptCanvasRoot.SetActive(true);

        NarrativeManager.Instance?.SetResumeModeInteractionOnly();
        NarrativeManager.Instance?.PauseNarrative();

        interactionToggle?.EnableInteractions();

        if (disableOnComplete != null)
            foreach (var b in disableOnComplete)
                if (b) b.enabled = true;
    }

    void Update()
    {
        if (!armed || completed || grabbable == null)
            return;

        bool isGrabbed = grabbable.SelectingPointsCount > 0;

        if (!wasGrabbed && isGrabbed) OnGrabbed();
        if (wasGrabbed && !isGrabbed) OnReleased();

        wasGrabbed = isGrabbed;

        if (!isGrabbed)
            return;

        SetWorldScale(transform, originalWorldScale);

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        heldTime += dt;

        if (progressRing) progressRing.fillAmount = Mathf.Clamp01(heldTime / holdSeconds);

        if (heldTime >= holdSeconds)
            Complete();
    }

    void OnGrabbed()
    {
        currentHoldPoint = ChooseHoldPoint();
        if (currentHoldPoint == null) return;

        transform.SetParent(currentHoldPoint, worldPositionStays: true);
        transform.position = currentHoldPoint.position;
        transform.rotation = currentHoldPoint.rotation;
        SetWorldScale(transform, originalWorldScale);

        if (rb != null)
        {
            rb.isKinematic = true;
            SetRbLinearVelocity(rb, Vector3.zero);
            rb.angularVelocity = Vector3.zero;
        }

        if (holdLoopSource != null && !holdLoopSource.isPlaying)
            holdLoopSource.Play();
    }

    void OnReleased()
    {
        if (holdLoopSource != null && holdLoopSource.isPlaying)
            holdLoopSource.Stop();

        transform.SetParent(null, worldPositionStays: true);
        currentHoldPoint = null;

        if (armed && !completed)
        {
            SnapBackToTable();

            heldTime = 0f;
            if (progressRing) progressRing.fillAmount = 0f;
        }
    }

    Transform ChooseHoldPoint()
    {
        if (leftHoldPoint == null && rightHoldPoint == null) return null;
        if (leftHoldPoint != null && rightHoldPoint == null) return leftHoldPoint;
        if (rightHoldPoint != null && leftHoldPoint == null) return rightHoldPoint;

        float dl = Vector3.Distance(transform.position, leftHoldPoint.position);
        float dr = Vector3.Distance(transform.position, rightHoldPoint.position);
        return (dl <= dr) ? leftHoldPoint : rightHoldPoint;
    }

    void Complete()
    {
        completed = true;
        armed = false;

        if (holdLoopSource != null && holdLoopSource.isPlaying)
            holdLoopSource.Stop();

        if (disableOnComplete != null)
            foreach (var b in disableOnComplete)
                if (b) b.enabled = false;

        interactionToggle?.DisableInteractions();

        transform.SetParent(null, worldPositionStays: true);

        transform.position = tablePos;
        transform.rotation = tableRot;
        SetWorldScale(transform, originalWorldScale);

        if (rb != null)
        {
            rb.isKinematic = true;
            SetRbLinearVelocity(rb, Vector3.zero);
            rb.angularVelocity = Vector3.zero;
        }

        if (promptCanvasRoot) promptCanvasRoot.SetActive(false);

        if (resumeDelay > 0f) Invoke(nameof(ResumeNarrativeInternal), resumeDelay);
        else ResumeNarrativeInternal();
    }

    void SnapBackToTable()
    {
        transform.position = tablePos;
        transform.rotation = tableRot;
        SetWorldScale(transform, originalWorldScale);

        if (rb != null)
        {
            rb.isKinematic = true;
            SetRbLinearVelocity(rb, Vector3.zero);
            rb.angularVelocity = Vector3.zero;
        }
    }

    void ResumeNarrativeInternal()
    {
        NarrativeManager.Instance?.SetResumeModeBoth();
        NarrativeManager.Instance?.ResumeNarrative();
    }

    static void SetWorldScale(Transform t, Vector3 worldScale)
    {
        var parent = t.parent;
        if (parent == null)
        {
            t.localScale = worldScale;
            return;
        }

        Vector3 parentScale = parent.lossyScale;
        t.localScale = new Vector3(
            parentScale.x != 0 ? worldScale.x / parentScale.x : t.localScale.x,
            parentScale.y != 0 ? worldScale.y / parentScale.y : t.localScale.y,
            parentScale.z != 0 ? worldScale.z / parentScale.z : t.localScale.z
        );
    }

    static void SetRbLinearVelocity(Rigidbody body, Vector3 v)
    {
#if UNITY_6000_0_OR_NEWER
        body.linearVelocity = v;
#else
        body.velocity = v;
#endif
    }
}