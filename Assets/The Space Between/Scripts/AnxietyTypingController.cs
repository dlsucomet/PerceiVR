using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AnxietyTypingController : MonoBehaviour
{
    [Header("Wiring")]
    public HoldToPlay holdUI;
    public InteractionToggle interactionToggle;

    [Header("Prompt")]
    public GameObject anxiousPrompt;

    [Header("Timing")]
    public float maxContinuousTypingSeconds = 8f;
    public float forcedBreakSeconds = 3f;

    [Header("Vignette")]
    public Volume globalVolume;
    public float baseVignette = 0.15f;
    public float maxVignette = 0.6f;

    [Header("Heartbeat")]
    public AudioSource heartbeatLoop;
    public float heartbeatMaxVolume = 0.9f;

    [Header("Breathing / SFX")]
    public AudioSource breathingSfxSource;
    public AudioClip breathingClip;
    [Range(0f, 1f)] public float breathingVolume = 1f;
    public bool restartBreathingIfAlreadyPlaying = true;

    [Header("Controller Haptics")]
    public float maxVibrationAmplitude = 0.8f;
    public float vibrationFrequency = 0.4f;

    [Tooltip("How slowly vibration ramps in (seconds). Larger = slower start.")]
    public float hapticsAttackSeconds = 0.6f;

    [Tooltip("How quickly vibration fades out (seconds). Smaller = faster fade.")]
    public float hapticsReleaseSeconds = 0.12f;

    [Tooltip("Delay before vibration starts ramping in (seconds).")]
    public float hapticsStartDelay = 0.25f;

    [Tooltip("Nonlinear shaping. >1 = slower at the start, stronger near the end.")]
    public float hapticsRampPower = 2.2f;

    float hapticsAmpCurrent = 0f;

    float anxiety01 = 0f;
    float continuousTyping = 0f;
    bool isTyping = false;
    bool inForcedBreak = false;

    Vignette vignette;

    void Awake()
    {
        if (globalVolume && globalVolume.profile)
            globalVolume.profile.TryGet(out vignette);

        ResetAnxiety();
    }

    void OnEnable()
    {
        if (holdUI != null)
        {
            holdUI.TypingStarted += OnTypingStarted;
            holdUI.TypingStopped += OnTypingStopped;
        }
    }

    void OnDisable()
    {
        if (holdUI != null)
        {
            holdUI.TypingStarted -= OnTypingStarted;
            holdUI.TypingStopped -= OnTypingStopped;
        }
        ResetAnxiety();
    }

    public void ResetAnxiety()
    {
        StopAllCoroutines();

        anxiety01 = 0f;
        continuousTyping = 0f;
        isTyping = false;
        inForcedBreak = false;
        hapticsAmpCurrent = 0f;

        anxiousPrompt?.SetActive(false);
        heartbeatLoop?.Stop();
        SetVibration(0f);

        ApplyVignette(0f);
    }

    void Update()
    {
        if (inForcedBreak)
            return;

        if (isTyping)
        {
            continuousTyping += Time.deltaTime;
            anxiety01 = Mathf.Clamp01(continuousTyping / maxContinuousTypingSeconds);

            if (continuousTyping >= maxContinuousTypingSeconds)
                StartCoroutine(ForcedBreak());
        }
        else
        {
            continuousTyping = Mathf.Max(0f, continuousTyping - Time.deltaTime);
            anxiety01 = Mathf.Clamp01(continuousTyping / maxContinuousTypingSeconds);
        }

        SetVibration(anxiety01);
        UpdateHeartbeat(anxiety01);
        ApplyVignette(anxiety01);
    }

    void OnTypingStarted() => isTyping = true;
    void OnTypingStopped() => isTyping = false;

    IEnumerator ForcedBreak()
    {
        if (inForcedBreak) yield break;
        inForcedBreak = true;

        anxiousPrompt?.SetActive(true);

        if (breathingSfxSource && breathingClip)
        {
            if (restartBreathingIfAlreadyPlaying)
                breathingSfxSource.Stop();

            breathingSfxSource.PlayOneShot(breathingClip, breathingVolume);
        }

        holdUI.SetHoldPromptSuppressed(true);
        holdUI.ForceBreak();

        yield return new WaitUntil(() => !holdUI.IsReleaseRequired);

        interactionToggle?.DisableInteractions();

        float duration = Mathf.Max(0.01f, forcedBreakSeconds);

        float startA = 1f;

        float endA = 0.4f;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);

            float a = Mathf.Lerp(startA, endA, k);

            SetVibration(a);
            UpdateHeartbeat(a);
            ApplyVignette(a);

            yield return null;
        }

        interactionToggle?.EnableInteractions();

        anxiousPrompt?.SetActive(false);
        holdUI.SetHoldPromptSuppressed(false);

        continuousTyping = endA * maxContinuousTypingSeconds;
        anxiety01 = Mathf.Clamp01(continuousTyping / maxContinuousTypingSeconds);

        inForcedBreak = false;
    }

    void UpdateHeartbeat(float a01)
    {
        if (!heartbeatLoop) return;

        heartbeatLoop.volume = Mathf.Lerp(
            heartbeatLoop.volume,
            a01 * heartbeatMaxVolume,
            Time.deltaTime * 6f
        );

        if (!heartbeatLoop.isPlaying && a01 > 0.05f)
            heartbeatLoop.Play();
        else if (heartbeatLoop.isPlaying && a01 < 0.01f)
            heartbeatLoop.Stop();
    }

    void ApplyVignette(float a01)
    {
        if (vignette == null) return;
        vignette.intensity.Override(Mathf.Lerp(baseVignette, maxVignette, a01));
    }

    void SetVibration(float a01)
    {
        float delayed01 = Mathf.Clamp01((a01 - hapticsStartDelay) / Mathf.Max(0.0001f, (1f - hapticsStartDelay)));

        float shaped = Mathf.Pow(delayed01, hapticsRampPower);

        float targetAmp = shaped * maxVibrationAmplitude;

        float tau = (targetAmp > hapticsAmpCurrent) ? hapticsAttackSeconds : hapticsReleaseSeconds;
        float lerpT = 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.0001f, tau));

        hapticsAmpCurrent = Mathf.Lerp(hapticsAmpCurrent, targetAmp, lerpT);

        OVRInput.SetControllerVibration(vibrationFrequency, hapticsAmpCurrent, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(vibrationFrequency, hapticsAmpCurrent, OVRInput.Controller.RTouch);
    }
}