using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TapFingers : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI")]
    public GameObject holdPrompt;
    public Image progressRing;

    [Header("Timing")]
    [Tooltip("Seconds of continuous holding required to complete.")]
    public float holdDurationSeconds = 2.0f;

    [Tooltip("Use unscaled time so it still progresses while Timeline/Narrative is paused.")]
    public bool useUnscaledTime = true;

    [Header("Audio")]
    [Tooltip("Looping tapping audio while holding. (Play On Awake OFF, Loop ON)")]
    public AudioSource tappingLoopSource;

    [Header("Haptics (OVRInput)")]
    [Tooltip("Fallback controller if we can't detect which hand poked.")]
    public OVRInput.Controller fallbackController = OVRInput.Controller.RTouch;

    [Range(0f, 1f)] public float vibrationStrength = 0.35f;

    [Tooltip("How often to pulse vibration while holding (lower = faster tapping feel).")]
    public float vibrationPulseInterval = 0.06f;

    [Tooltip("High-frequency motor (0..1). Often feels 'buzzier'.")]
    [Range(0f, 1f)] public float vibrationFrequency = 0.7f;

    [Header("Requirements (Optional)")]
    public GrabState requiredGrab;

    public event Action HoldStarted;
    public event Action HoldStopped;
    public event Action HoldCompleted;

    bool armed = false;
    bool holding = false;
    bool requireRelease = false;
    bool suppressHoldPrompt = false;

    float heldTime = 0f;
    float pulseTimer = 0f;

    OVRInput.Controller activeController;

    public bool IsArmed => armed;
    public bool IsHolding => holding;
    public bool IsReleaseRequired => requireRelease;

    void Start()
    {
        if (holdPrompt) holdPrompt.SetActive(false);

        if (progressRing)
        {
            progressRing.fillAmount = 0f;
            progressRing.gameObject.SetActive(false);
        }

        if (tappingLoopSource)
        {
            tappingLoopSource.loop = true;
            tappingLoopSource.playOnAwake = false;
            tappingLoopSource.Stop();
        }

        activeController = fallbackController;
    }

    void Update()
    {
        if (!armed || !holding || holdDurationSeconds <= 0f)
            return;

        if (requiredGrab != null && !requiredGrab.IsGrabbed)
            return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        heldTime += dt;
        if (progressRing)
            progressRing.fillAmount = Mathf.Clamp01(heldTime / holdDurationSeconds);

        pulseTimer += dt;
        if (pulseTimer >= vibrationPulseInterval)
        {
            pulseTimer = 0f;
            OVRInput.SetControllerVibration(vibrationFrequency, vibrationStrength, activeController);
        }

        if (heldTime >= holdDurationSeconds)
        {
            Complete();
        }
    }

    public void SetHoldPromptSuppressed(bool suppressed)
    {
        suppressHoldPrompt = suppressed;
        if (holdPrompt)
            holdPrompt.SetActive(armed && !holding && !suppressHoldPrompt);
    }

    public void Arm(float durationSeconds)
    {
        holdDurationSeconds = Mathf.Max(0.01f, durationSeconds);

        armed = true;
        holding = false;
        requireRelease = false;

        heldTime = 0f;
        pulseTimer = 0f;
        activeController = fallbackController;

        if (holdPrompt) holdPrompt.SetActive(!suppressHoldPrompt);

        if (progressRing)
        {
            progressRing.fillAmount = 0f;
            progressRing.gameObject.SetActive(true);
        }

        StopFeedback();
    }

    public void Disarm()
    {
        armed = false;
        holding = false;
        requireRelease = false;

        heldTime = 0f;
        pulseTimer = 0f;

        StopFeedback();

        if (holdPrompt) holdPrompt.SetActive(false);
        if (progressRing) progressRing.gameObject.SetActive(false);
    }

    public void ResetProgress()
    {
        heldTime = 0f;
        if (progressRing) progressRing.fillAmount = 0f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!armed || requireRelease)
            return;

        holding = true;
        pulseTimer = vibrationPulseInterval;

        activeController = TryDetectController(eventData, fallbackController);

        // if (holdPrompt) holdPrompt.SetActive(false);

        if (tappingLoopSource && !tappingLoopSource.isPlaying)
            tappingLoopSource.Play();

        HoldStarted?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        holding = false;

        if (requireRelease)
            requireRelease = false;

        StopFeedback();

        if (holdPrompt) holdPrompt.SetActive(!suppressHoldPrompt);

        HoldStopped?.Invoke();
    }

    public void ForceBreak()
    {
        holding = false;
        requireRelease = true;

        StopFeedback();

        if (holdPrompt) holdPrompt.SetActive(!suppressHoldPrompt);

        HoldStopped?.Invoke();
    }

    void Complete()
    {
        holding = false;
        requireRelease = false;

        StopFeedback();
        HoldCompleted?.Invoke();
    }

    void StopFeedback()
    {
        if (tappingLoopSource)
            tappingLoopSource.Stop();

        OVRInput.SetControllerVibration(0, 0, activeController);
        OVRInput.SetControllerVibration(0, 0, fallbackController);
    }

    OVRInput.Controller TryDetectController(PointerEventData eventData, OVRInput.Controller fallback)
    {
        if (eventData == null)
            return fallback;

        Camera cam = eventData.pressEventCamera != null ? eventData.pressEventCamera : eventData.enterEventCamera;

        if (cam != null)
        {
            string n = cam.name.ToLowerInvariant();
            if (n.Contains("left")) return OVRInput.Controller.LTouch;
            if (n.Contains("right")) return OVRInput.Controller.RTouch;

            Transform t = cam.transform;
            for (int i = 0; i < 4 && t != null; i++, t = t.parent)
            {
                string pn = t.name.ToLowerInvariant();
                if (pn.Contains("left")) return OVRInput.Controller.LTouch;
                if (pn.Contains("right")) return OVRInput.Controller.RTouch;
            }
        }

        return fallback;
    }
}