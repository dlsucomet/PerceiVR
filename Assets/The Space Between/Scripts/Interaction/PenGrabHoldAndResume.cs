using UnityEngine;
using Oculus.Interaction;
using UnityEngine.UI;

public class PenGrabHoldAndResume : MonoBehaviour
{
    [Header("Oculus Interaction")]
    public Grabbable grabbable;

    [Header("UI (optional)")]
    public GameObject promptCanvasRoot;
    public Image progressRing;

    [Header("Timing")]
    public float holdSeconds = 3.0f;
    public bool useUnscaledTime = true;

    [Header("Completion")]
    public bool hidePromptOnComplete = true;
    public InteractionToggle interactionToggle;
    public float resumeDelay = 0.05f;

    float heldTime = 0f;
    bool armed = false;
    bool completed = false;

    void Awake()
    {
        if (grabbable == null)
            grabbable = GetComponent<Grabbable>();

        if (progressRing != null)
            progressRing.fillAmount = 0f;

        if (promptCanvasRoot != null)
            promptCanvasRoot.SetActive(false);
    }

    public void Arm()
    {
        armed = true;
        completed = false;
        heldTime = 0f;

        if (progressRing != null)
            progressRing.fillAmount = 0f;

        if (promptCanvasRoot != null)
            promptCanvasRoot.SetActive(true);

        if (NarrativeManager.Instance != null)
        {
            NarrativeManager.Instance.SetResumeModeInteractionOnly();
            NarrativeManager.Instance.PauseNarrative();
        }

        interactionToggle?.EnableInteractions();
    }

    void Update()
    {
        if (!armed || completed || grabbable == null)
            return;

        bool isGrabbed = grabbable.SelectingPointsCount > 0;
        if (!isGrabbed)
            return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        heldTime += dt;

        if (progressRing != null)
            progressRing.fillAmount = Mathf.Clamp01(heldTime / holdSeconds);

        if (heldTime >= holdSeconds)
            Complete();
    }

    void Complete()
    {
        completed = true;
        armed = false;

        if (hidePromptOnComplete && promptCanvasRoot != null)
            promptCanvasRoot.SetActive(false);

        interactionToggle?.DisableInteractions();

        if (resumeDelay > 0f) Invoke(nameof(ResumeNarrativeInternal), resumeDelay);
        else ResumeNarrativeInternal();
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