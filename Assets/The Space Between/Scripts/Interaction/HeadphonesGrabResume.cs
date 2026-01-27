using UnityEngine;

public class HeadphonesGrabResume : MonoBehaviour
{
    [Header("References")]
    public GameObject promptText;

    [Tooltip("Optional delay before resuming narrative")]
    public float resumeDelay = 0.15f;

    Rigidbody rb;
    bool hasResumed = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (hasResumed || rb == null)
            return;

        if (rb.isKinematic)
        {
            hasResumed = true;

            if (promptText)
                promptText.SetActive(false);

            if (resumeDelay > 0f)
                Invoke(nameof(ResumeNarrativeInternal), resumeDelay);
            else
                ResumeNarrativeInternal();
        }
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