using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager Instance;

    [Header("Timeline Settings")]
    public PlayableDirector currentDirector;

    [Header("Input Settings")]
    public InputActionReference resumeAction;

    public enum ResumeMode
    {
        ButtonOnly,
        InteractionOnly,
        Both
    }

    [Header("Resume Control")]
    public ResumeMode resumeMode = ResumeMode.Both;

    [Header("Haptic Settings")]
    [Range(0f, 1f)] public float vibrationIntensity = 0.5f;
    public float vibrationDuration = 0.2f;

    [Header("Spoken Subtitles (Dialogue)")]
    public SubtitleManager spokenSubtitleManager;
    public GameObject spokenNextIndicator;

    [Header("Thought Subtitles (Internal Monologue)")]
    public SubtitleManager thoughtsSubtitleManager;
    public GameObject thoughtsNextIndicator;

    private double pauseTime;
    private bool isWaiting = false;

    private void Awake() => Instance = this;

    private void OnEnable()
    {
        if (resumeAction != null) resumeAction.action.Enable();

        if (spokenNextIndicator != null) spokenNextIndicator.SetActive(false);
        if (thoughtsNextIndicator != null) thoughtsNextIndicator.SetActive(false);
    }

    public bool IsWaiting() => isWaiting;

    public void SetResumeModeInteractionOnly() => resumeMode = ResumeMode.InteractionOnly;

    public void SetResumeModeBoth() => resumeMode = ResumeMode.Both;

    public void SetResumeModeButtonOnly() => resumeMode = ResumeMode.ButtonOnly;

    public void PauseNarrative()
    {
        if (currentDirector == null) return;

        pauseTime = currentDirector.time;
        isWaiting = true;

        if (spokenSubtitleManager != null) spokenSubtitleManager.CancelInvoke("HideSubtitle");
        if (thoughtsSubtitleManager != null) thoughtsSubtitleManager.CancelInvoke("HideSubtitle");

        if (spokenNextIndicator != null) spokenNextIndicator.SetActive(true);
        if (thoughtsNextIndicator != null) thoughtsNextIndicator.SetActive(true);

        TriggerHaptics();
        Debug.Log($"Narrative Paused at {pauseTime}. Waiting for input...");
    }

    private void Update()
    {
        if (!isWaiting) return;

        if (currentDirector != null)
            currentDirector.time = pauseTime;

        bool allowButtonResume = (resumeMode == ResumeMode.ButtonOnly || resumeMode == ResumeMode.Both);

        if (allowButtonResume && resumeAction != null && resumeAction.action != null && resumeAction.action.WasPressedThisFrame())
        {
            ResumeNarrative();
        }
    }

    public void ResumeNarrative()
    {
        isWaiting = false;

        // Hide Spoken UI
        if (spokenSubtitleManager != null) spokenSubtitleManager.HideSubtitle();
        if (spokenNextIndicator != null) spokenNextIndicator.SetActive(false);

        // Hide Thoughts UI
        if (thoughtsSubtitleManager != null) thoughtsSubtitleManager.HideSubtitle();
        if (thoughtsNextIndicator != null) thoughtsNextIndicator.SetActive(false);

        if (currentDirector != null) currentDirector.Play();

        Debug.Log("Narrative Resumed.");
    }

    private void TriggerHaptics()
    {
        VibrateController(XRNode.LeftHand);
        VibrateController(XRNode.RightHand);
    }

    private void VibrateController(XRNode node)
    {
        UnityEngine.XR.InputDevice device = InputDevices.GetDeviceAtXRNode(node);

        if (device.isValid)
        {
            HapticCapabilities capabilities;
            if (device.TryGetHapticCapabilities(out capabilities) && capabilities.supportsImpulse)
            {
                device.SendHapticImpulse(0u, vibrationIntensity, vibrationDuration);
            }
        }
    }
}