using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AnxietyTypingController : MonoBehaviour
{
    [Header("Wiring")]
    public TypingManager typingManager;
    public HoldToPlay holdUI;
    public InteractionToggle interactionToggle;

    [Header("Break UI")]
    public GameObject anxiousPrompt;

    [Header("Timing")]
    public float maxContinuousTypingSeconds = 8f;
    public float forcedBreakSeconds = 3f;
    public float graceToClearSeconds = 1.5f;

    [Header("Vignette")]
    public Volume globalVolume;
    [Range(0f, 1f)] public float baseVignette = 0.15f;
    [Range(0f, 1f)] public float maxVignette = 0.60f;

    [Header("Anxiety Audio")]
    public AudioSource heartbeatLoop;
    public float heartbeatMaxVolume = 0.9f;
    public float heartbeatStartAt = 0.15f;
    public float heartbeatFadeSpeed = 6f;

    public AudioSource sfxSource;
    public AudioClip sighClip;

    public AnimationCurve ramp = AnimationCurve.EaseInOut(0, 0, 1, 1);

    float anxiety01 = 0f;
    float continuousTyping = 0f;
    bool isTyping = false;
    bool inForcedBreak = false;

    Vignette vignette;

    void Awake()
    {
        if (anxiousPrompt) anxiousPrompt.SetActive(false);

        if (globalVolume != null && globalVolume.profile != null)
            globalVolume.profile.TryGet(out vignette);

        ApplyVignette(0f);
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
    }

    void Update()
    {
        if (isTyping && !inForcedBreak)
        {
            continuousTyping += Time.deltaTime;

            anxiety01 = Mathf.Clamp01(continuousTyping / maxContinuousTypingSeconds);

            if (continuousTyping >= maxContinuousTypingSeconds)
            {
                StartCoroutine(ForcedBreak());
            }
        }
        else
        {
            if (continuousTyping > 0f)
            {
                continuousTyping = Mathf.Max(0f, continuousTyping - (Time.deltaTime * (maxContinuousTypingSeconds / Mathf.Max(0.1f, graceToClearSeconds))));
                anxiety01 = Mathf.Clamp01(continuousTyping / maxContinuousTypingSeconds);
            }
        }

        UpdateHeartbeat(anxiety01);

        ApplyVignette(anxiety01);
    }

    void UpdateHeartbeat(float a01)
    {
        if (!heartbeatLoop) return;

        float targetVol = 0f;

        if (a01 >= heartbeatStartAt && !inForcedBreak)
        {
            float t = Mathf.InverseLerp(heartbeatStartAt, 1f, a01);
            targetVol = t * heartbeatMaxVolume;
        }

        if (targetVol > 0.001f && !heartbeatLoop.isPlaying)
            heartbeatLoop.Play();

        heartbeatLoop.volume = Mathf.Lerp(
            heartbeatLoop.volume,
            targetVol,
            Time.deltaTime * heartbeatFadeSpeed
        );

        if (heartbeatLoop.isPlaying && heartbeatLoop.volume < 0.001f && targetVol <= 0.001f)
            heartbeatLoop.Stop();
    }

    void OnTypingStarted()
    {
        if (inForcedBreak) return;

        isTyping = true;
        if (anxiousPrompt) anxiousPrompt.SetActive(false);
    }

    void OnTypingStopped()
    {
        isTyping = false;
    }

    IEnumerator ForcedBreak()
    {
        if (inForcedBreak) yield break;
        inForcedBreak = true;

        if (anxiousPrompt) anxiousPrompt.SetActive(true);

        if (sfxSource && sighClip)
            sfxSource.PlayOneShot(sighClip);

        if (holdUI) holdUI.ForceBreak();

        interactionToggle?.DisableInteractions();

        yield return new WaitForSeconds(forcedBreakSeconds);

        interactionToggle?.EnableInteractions();

        if (anxiousPrompt) anxiousPrompt.SetActive(false);

        continuousTyping = Mathf.Max(0f, continuousTyping - maxContinuousTypingSeconds * 0.6f);
        anxiety01 = Mathf.Clamp01(continuousTyping / maxContinuousTypingSeconds);

        isTyping = false;
        inForcedBreak = false;
    }

    void ApplyVignette(float t01)
    {
        if (vignette == null) return;

        float shaped = ramp != null ? ramp.Evaluate(t01) : t01;
        float intensity = Mathf.Lerp(baseVignette, maxVignette, shaped);

        vignette.intensity.Override(intensity);
    }
}
