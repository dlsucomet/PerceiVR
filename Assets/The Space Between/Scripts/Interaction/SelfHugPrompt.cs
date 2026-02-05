using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelfHugPrompt : MonoBehaviour
{
    [Header("UI")]
    public GameObject promptCanvasRoot;
    public Image progressRing;
    public TMP_Text promptText;

    [Header("Prompt Text (optional)")]
    [TextArea] public string promptMessage = "Wrap your arms around yourself to ground your body.";

    [Header("Hold Timing")]
    public float holdSeconds = 2.0f;
    public bool decayWhenNotHolding = true;
    public float decaySpeed = 1.25f;

    [Header("Tracking (assign these)")]
    public Transform headOrChest;
    public Transform leftHand;
    public Transform rightHand;

    [Header("Self-hug shoulder zone (relative to headOrChest)")]
    public float shoulderWidth = 0.20f;
    public float shoulderHeight = -0.15f;
    public float shoulderForward = 0.10f;
    public float zoneRadius = 0.18f;

    [Header("Narrative Control")]
    public bool pauseOnBegin = true;
    public bool setInteractionOnlyOnBegin = true;
    public bool restoreResumeModeBothOnComplete = true;

    float progress01;
    bool active;

    public void BeginSelfHug()
    {
        active = true;
        progress01 = 0f;

        if (promptCanvasRoot != null) promptCanvasRoot.SetActive(true);
        if (progressRing != null) progressRing.fillAmount = 0f;
        if (promptText != null) promptText.text = promptMessage;

        if (pauseOnBegin && NarrativeManager.Instance != null)
        {
            if (setInteractionOnlyOnBegin) NarrativeManager.Instance.SetResumeModeInteractionOnly();
            NarrativeManager.Instance.PauseNarrative();
        }
    }

    public void Cancel()
    {
        active = false;
        progress01 = 0f;
        if (promptCanvasRoot != null) promptCanvasRoot.SetActive(false);
        if (progressRing != null) progressRing.fillAmount = 0f;
    }

    void Update()
    {
        if (!active) return;

        if (headOrChest == null || leftHand == null || rightHand == null) return;

        bool isHugging = DetectSelfHug();

        if (isHugging)
        {
            progress01 += Time.deltaTime / Mathf.Max(0.01f, holdSeconds);
        }
        else if (decayWhenNotHolding)
        {
            progress01 -= Time.deltaTime * decaySpeed / Mathf.Max(0.01f, holdSeconds);
        }

        progress01 = Mathf.Clamp01(progress01);

        if (progressRing != null)
            progressRing.fillAmount = progress01;

        if (progress01 >= 1f)
        {
            Complete();
        }
    }

    bool DetectSelfHug()
    {
        Vector3 leftShoulder = headOrChest.TransformPoint(new Vector3(-shoulderWidth, shoulderHeight, shoulderForward));
        Vector3 rightShoulder = headOrChest.TransformPoint(new Vector3(shoulderWidth, shoulderHeight, shoulderForward));

        float leftToRightShoulder = Vector3.Distance(leftHand.position, rightShoulder);
        float rightToLeftShoulder = Vector3.Distance(rightHand.position, leftShoulder);

        return (leftToRightShoulder <= zoneRadius && rightToLeftShoulder <= zoneRadius);
    }

    void Complete()
    {
        active = false;

        if (promptCanvasRoot != null) promptCanvasRoot.SetActive(false);

        if (NarrativeManager.Instance != null)
        {
            if (restoreResumeModeBothOnComplete)
                NarrativeManager.Instance.SetResumeModeBoth();

            NarrativeManager.Instance.ResumeNarrative();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (headOrChest == null) return;

        Vector3 leftShoulder = headOrChest.TransformPoint(new Vector3(-shoulderWidth, shoulderHeight, shoulderForward));
        Vector3 rightShoulder = headOrChest.TransformPoint(new Vector3(shoulderWidth, shoulderHeight, shoulderForward));

        Gizmos.DrawWireSphere(leftShoulder, zoneRadius);
        Gizmos.DrawWireSphere(rightShoulder, zoneRadius);
    }
}