using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class TapFingers : MonoBehaviour
{
    [Header("RIGHT HAND INPUT")]
    [Tooltip("Bind ONLY to Right Controller Trigger (float 0–1).")]
    public InputActionReference rightTriggerAction;

    [Range(0.05f, 0.95f)]
    public float pressThreshold = 0.8f;

    [Header("Hold Settings")]
    public float requiredHoldSeconds = 2.0f;
    public bool resetProgressOnExit = true;

    [Header("UI")]
    public GameObject promptRoot;
    public Image progressRing;

    [Header("Hand Animation")]
    public Animator rightHandAnimator;
    public string loopStateName = "TapFingers_Loop";
    public string boolParam = "IsTapping";

    [Header("Narrative Integration")]
    public bool autoResumeOnComplete = true;
    public NarrativeManager.ResumeMode restoreResumeMode = NarrativeManager.ResumeMode.Both;

    public event Action Completed;

    private bool running = false;
    private bool armed = false;      // right hand inside zone
    private bool holding = false;    // right trigger pressed
    private float heldTime = 0f;

    void OnEnable()
    {
        if (rightTriggerAction != null)
            rightTriggerAction.action.Enable();
    }

    void OnDisable()
    {
        if (rightTriggerAction != null)
            rightTriggerAction.action.Disable();

        RestoreNarrativeResumeMode();
    }

    void Start()
    {
        if (promptRoot) promptRoot.SetActive(false);

        if (progressRing)
        {
            progressRing.gameObject.SetActive(false);
            progressRing.fillAmount = 0f;
        }
    }

    void Update()
    {
        if (!running) return;

        float triggerValue = rightTriggerAction != null
            ? rightTriggerAction.action.ReadValue<float>()
            : 0f;

        bool pressed = armed && triggerValue >= pressThreshold;

        if (pressed && !holding)
        {
            holding = true;
            if (promptRoot) promptRoot.SetActive(false);
            StartHandLoop();
        }
        else if (!pressed && holding)
        {
            holding = false;
            StopHandLoop();
            if (promptRoot) promptRoot.SetActive(armed);
        }

        if (holding)
        {
            heldTime += Time.deltaTime;
            float t = Mathf.Clamp01(heldTime / requiredHoldSeconds);

            if (progressRing) progressRing.fillAmount = t;

            if (t >= 1f)
                FinishInteraction();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!running) return;
        if (!IsRightHand(other)) return;

        armed = true;
        if (promptRoot) promptRoot.SetActive(!holding);
    }

    void OnTriggerExit(Collider other)
    {
        if (!running) return;
        if (!IsRightHand(other)) return;

        armed = false;
        holding = false;
        StopHandLoop();

        if (promptRoot) promptRoot.SetActive(false);

        if (resetProgressOnExit)
        {
            heldTime = 0f;
            if (progressRing) progressRing.fillAmount = 0f;
        }
    }

    bool IsRightHand(Collider c)
    {
        // IMPORTANT: Tag your RIGHT controller/hand collider as "RightController"
        return c.CompareTag("RightController");
    }

    /// <summary>
    /// Call from Timeline Signal (after PauseNarrative).
    /// </summary>
    public void BeginTapFingers()
    {
        running = true;
        armed = false;
        holding = false;
        heldTime = 0f;

        if (progressRing)
        {
            progressRing.fillAmount = 0f;
            progressRing.gameObject.SetActive(true);
        }

        if (promptRoot) promptRoot.SetActive(true);

        // This blocks A-button resume in NarrativeManager.Update()
        NarrativeManager.Instance.SetResumeModeInteractionOnly();
    }

    void FinishInteraction()
    {
        running = false;
        armed = false;
        holding = false;

        StopHandLoop();

        if (promptRoot) promptRoot.SetActive(false);
        if (progressRing) progressRing.gameObject.SetActive(false);

        Completed?.Invoke();

        RestoreNarrativeResumeMode();

        if (autoResumeOnComplete)
            NarrativeManager.Instance.ResumeNarrative();
    }

    void RestoreNarrativeResumeMode()
    {
        if (NarrativeManager.Instance == null) return;

        switch (restoreResumeMode)
        {
            case NarrativeManager.ResumeMode.ButtonOnly:
                NarrativeManager.Instance.SetResumeModeButtonOnly();
                break;
            case NarrativeManager.ResumeMode.InteractionOnly:
                NarrativeManager.Instance.SetResumeModeInteractionOnly();
                break;
            default:
                NarrativeManager.Instance.SetResumeModeBoth();
                break;
        }
    }

    void StartHandLoop()
    {
        if (!rightHandAnimator) return;

        if (!string.IsNullOrEmpty(boolParam) && HasBoolParam(boolParam))
            rightHandAnimator.SetBool(boolParam, true);
        else
            rightHandAnimator.Play(loopStateName, 0, 0f);
    }

    void StopHandLoop()
    {
        if (!rightHandAnimator) return;

        if (!string.IsNullOrEmpty(boolParam) && HasBoolParam(boolParam))
            rightHandAnimator.SetBool(boolParam, false);
    }

    bool HasBoolParam(string param)
    {
        foreach (var p in rightHandAnimator.parameters)
            if (p.type == AnimatorControllerParameterType.Bool && p.name == param)
                return true;
        return false;
    }
}