using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoverEarsPrompt : MonoBehaviour
{
    [Header("UI")]
    public GameObject promptCanvasRoot;
    public Image progressRing;
    public TMP_Text promptText;
    [TextArea] public string promptMessage = "Cover your ears.";

    [Header("Hold Timing")]
    public float holdSeconds = 2.0f;
    public float decaySpeed = 1.25f;

    [Header("Tracking")]
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    [Header("Ear Zones")]
    public float earSide = 0.12f;
    public float earHeight = -0.05f;
    public float earForward = 0.04f;
    public float zoneRadius = 0.18f;

    float progress;
    bool active;

    public void BeginCoverEars()
    {
        active = true;
        progress = 0f;

        promptCanvasRoot.SetActive(true);
        progressRing.fillAmount = 0f;

        if (!string.IsNullOrWhiteSpace(promptMessage))
            promptText.text = promptMessage;

        NarrativeManager.Instance.SetResumeModeInteractionOnly();
        NarrativeManager.Instance.PauseNarrative();
    }

    void Update()
    {
        if (!active) return;

        bool covering = DetectCover();

        if (covering)
            progress += Time.deltaTime / holdSeconds;
        else
            progress -= Time.deltaTime * decaySpeed / holdSeconds;

        progress = Mathf.Clamp01(progress);
        progressRing.fillAmount = progress;

        if (progress >= 1f)
            Complete();
    }

    bool DetectCover()
    {
        Vector3 leftEar =
            head.TransformPoint(new Vector3(-earSide, earHeight, earForward));

        Vector3 rightEar =
            head.TransformPoint(new Vector3(earSide, earHeight, earForward));

        bool leftCorrect =
            Vector3.Distance(leftHand.position, leftEar) <= zoneRadius;

        bool rightCorrect =
            Vector3.Distance(rightHand.position, rightEar) <= zoneRadius;

        return leftCorrect && rightCorrect;
    }

    void Complete()
    {
        active = false;

        promptCanvasRoot.SetActive(false);

        NarrativeManager.Instance.SetResumeModeBoth();
        NarrativeManager.Instance.ResumeNarrative();
    }

    void OnDrawGizmosSelected()
    {
        if (head == null) return;

        Vector3 leftEar =
            head.TransformPoint(new Vector3(-earSide, earHeight, earForward));

        Vector3 rightEar =
            head.TransformPoint(new Vector3(earSide, earHeight, earForward));

        Gizmos.DrawWireSphere(leftEar, zoneRadius);
        Gizmos.DrawWireSphere(rightEar, zoneRadius);
    }
}