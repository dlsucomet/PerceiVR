using UnityEngine;

[DisallowMultipleComponent]
public class HoldToTap : MonoBehaviour
{
    [Header("References")]
    public TapFingers tapFingers;
    public GameObject uiRootToToggle;

    [Tooltip("Assign your InteractionToggleManager here (optional).")]
    public InteractionToggle interactionToggle;

    [Header("Settings")]
    public float holdSeconds = 2.0f;

    void Awake()
    {
        if (tapFingers == null)
            tapFingers = GetComponent<TapFingers>();

        if (uiRootToToggle != null)
            uiRootToToggle.SetActive(false);
    }

    public void StartTableTapInteraction()
    {
        if (tapFingers == null || NarrativeManager.Instance == null)
        {
            Debug.LogError("HoldToTap: Missing TapFingers or NarrativeManager.");
            return;
        }

        if (uiRootToToggle != null)
            uiRootToToggle.SetActive(true);

        NarrativeManager.Instance.SetResumeModeInteractionOnly();
        NarrativeManager.Instance.PauseNarrative();

        tapFingers.Arm(holdSeconds);
    }

    void OnEnable()
    {
        if (tapFingers != null)
            tapFingers.HoldCompleted += OnHoldCompleted;
    }

    void OnDisable()
    {
        if (tapFingers != null)
            tapFingers.HoldCompleted -= OnHoldCompleted;
    }

    void OnHoldCompleted()
    {
        tapFingers.Disarm();

        if (uiRootToToggle != null)
            uiRootToToggle.SetActive(false);

        if (interactionToggle != null)
            interactionToggle.DisableInteractions();

        NarrativeManager.Instance.SetResumeModeBoth();
        NarrativeManager.Instance.ResumeNarrative();
    }
}