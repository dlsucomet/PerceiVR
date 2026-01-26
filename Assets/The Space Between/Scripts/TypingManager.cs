using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class TypingManager : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public VideoClip typingClip;

    [Header("UI + Hold")]
    public HoldToPlay holdUI;
    public KeyboardGlow keyboardGlow;
    public InteractionToggle interactionToggle;

    [Header("Segments (seconds)")]
    public double segment1Start = 0.0;
    public double segment1End = 8.0;

    public double segment2Start = 8.0;
    public double segment2End = 18.0;

    [Tooltip("Pause between segment 1 and 2 (real-time seconds).")]
    public float betweenSegmentsDelay = 2.0f;

    [Header("Optional anxiety controller (segment 2 only)")]
    public AnxietyTypingController anxietyController;

    bool segmentActive = false;
    double activeEndTime = -1.0;
    bool useSegmentProgressUI = false;

    Coroutine twoSegmentRoutine;

    void Start()
    {
        if (videoPlayer == null || typingClip == null) return;

        videoPlayer.clip = typingClip;
        videoPlayer.playOnAwake = false;

        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnPrepared;
    }

    void OnPrepared(VideoPlayer vp)
    {
        vp.Play();
        vp.Pause();
    }

    void Update()
    {
        if (!segmentActive || videoPlayer == null) return;

        if (activeEndTime > 0 && videoPlayer.time >= activeEndTime - 0.0001)
        {
            FinishCurrentSegment();
        }
    }

    public void StartTypingBeat()
    {
        Debug.Log("Starting Typing Beat...");
        if (videoPlayer == null || holdUI == null || typingClip == null) return;

        StopAnyTwoSegmentRoutine();

        SetAnxietyEnabled(false);

        NarrativeManager.Instance.PauseNarrative();

        keyboardGlow?.EnableGlow();

        videoPlayer.clip = typingClip;
        videoPlayer.time = 0;
        videoPlayer.Play();
        videoPlayer.Pause();

        segmentActive = true;
        activeEndTime = typingClip.length;
        useSegmentProgressUI = false;

        holdUI.Arm(videoPlayer);
    }

    public void StartTwoSegmentBeat()
    {
        if (videoPlayer == null || holdUI == null || typingClip == null) return;

        StopAnyTwoSegmentRoutine();
        twoSegmentRoutine = StartCoroutine(TwoSegmentFlow());
    }

    IEnumerator TwoSegmentFlow()
    {
        NarrativeManager.Instance.PauseNarrative();

        SetAnxietyEnabled(false);

        StartSegmentInternal(segment1Start, segment1End, segmentProgress: true);

        yield return new WaitUntil(() => !segmentActive);

        if (betweenSegmentsDelay > 0f)
            yield return new WaitForSeconds(betweenSegmentsDelay);

        SetAnxietyEnabled(true);

        StartSegmentInternal(segment2Start, segment2End, segmentProgress: true);

        yield return new WaitUntil(() => !segmentActive);

        NarrativeManager.Instance.ResumeNarrative();
        twoSegmentRoutine = null;
    }

    public void StartTypingSegment1_Assignment()
    {
        StopAnyTwoSegmentRoutine();
        NarrativeManager.Instance.PauseNarrative();
        SetAnxietyEnabled(false);
        StartSegmentInternal(segment1Start, segment1End, segmentProgress: true);
    }

    public void StartTypingSegment2_Email()
    {
        StopAnyTwoSegmentRoutine();
        NarrativeManager.Instance.PauseNarrative();
        SetAnxietyEnabled(true);
        StartSegmentInternal(segment2Start, segment2End, segmentProgress: true);
    }

    void StartSegmentInternal(double startTime, double endTime, bool segmentProgress)
    {
        interactionToggle?.EnableInteractions();
        keyboardGlow?.EnableGlow();

        videoPlayer.clip = typingClip;
        videoPlayer.time = startTime;

        videoPlayer.Play();
        videoPlayer.Pause();

        segmentActive = true;
        activeEndTime = endTime;
        useSegmentProgressUI = segmentProgress;

        if (segmentProgress)
            holdUI.Arm(videoPlayer, startTime, endTime);
        else
            holdUI.Arm(videoPlayer);
    }

    void FinishCurrentSegment()
    {
        if (!segmentActive) return;

        segmentActive = false;

        videoPlayer.Pause();

        // Hide prompt/progress and stop interaction
        holdUI.Disarm();
        keyboardGlow?.DisableGlow();

        if (holdUI.typingAudio) holdUI.typingAudio.Stop();

        interactionToggle?.DisableInteractions();
    }

    void SetAnxietyEnabled(bool enabled)
    {
        if (anxietyController == null) return;

        if (enabled)
        {
            anxietyController.enabled = true;
            anxietyController.ResetAnxiety();
        }
        else
        {

            anxietyController.ResetAnxiety();
            anxietyController.enabled = false;
        }
    }

    void StopAnyTwoSegmentRoutine()
    {
        if (twoSegmentRoutine != null)
        {
            StopCoroutine(twoSegmentRoutine);
            twoSegmentRoutine = null;
        }
    }
}