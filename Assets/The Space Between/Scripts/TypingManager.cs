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

    [Tooltip("If true, segment 2 ends when the video ENDS (recommended). If false, it ends at segment2End.")]
    public bool segment2EndsAtVideoEnd = true;

    [Tooltip("Used only if segment2EndsAtVideoEnd = false.")]
    public double segment2End = 18.0;

    [Tooltip("Pause between segment 1 and 2 (real-time seconds).")]
    public float betweenSegmentsDelay = 2.0f;

    [Header("Optional anxiety controller (segment 2 only)")]
    public AnxietyTypingController anxietyController;

    bool segmentActive = false;
    double activeEndTime = -1.0;
    bool finishOnVideoEnd = false;

    Coroutine twoSegmentRoutine;

    void Start()
    {
        if (videoPlayer == null || typingClip == null) return;

        videoPlayer.clip = typingClip;
        videoPlayer.playOnAwake = false;

        videoPlayer.isLooping = false;

        videoPlayer.loopPointReached += OnVideoEnded;

        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnPrepared;
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoEnded;
    }

    void OnPrepared(VideoPlayer vp)
    {
        vp.Play();
        vp.Pause();
    }

    void Update()
    {
        if (!segmentActive || videoPlayer == null) return;

        if (!finishOnVideoEnd && activeEndTime > 0 && videoPlayer.time >= activeEndTime - 0.0001)
        {
            FinishCurrentSegment();
            return;
        }

        if (finishOnVideoEnd)
        {
            if (videoPlayer.frameCount > 0)
            {
                if (videoPlayer.frame >= (long)videoPlayer.frameCount - 1)
                {
                    FinishCurrentSegment();
                    return;
                }
            }
            else if (videoPlayer.length > 0.0001)
            {
                if (videoPlayer.time >= videoPlayer.length - 0.05)
                {
                    FinishCurrentSegment();
                    return;
                }
            }
        }
    }

    void OnVideoEnded(VideoPlayer vp)
    {
        if (!segmentActive) return;
        if (!finishOnVideoEnd) return;

        FinishCurrentSegment();
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
        videoPlayer.isLooping = false;
        videoPlayer.time = 0;
        videoPlayer.Play();
        videoPlayer.Pause();

        segmentActive = true;
        finishOnVideoEnd = true;
        activeEndTime = -1.0;

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
        StartSegmentInternal(segment1Start, segment1End, endOnVideoEnd: false, useSegmentProgressUI: true);

        yield return new WaitUntil(() => !segmentActive);

        if (betweenSegmentsDelay > 0f)
            yield return new WaitForSeconds(betweenSegmentsDelay);

        SetAnxietyEnabled(true);

        if (segment2EndsAtVideoEnd)
        {
            StartSegmentInternal(segment2Start, endTime: -1.0, endOnVideoEnd: true, useSegmentProgressUI: false);
        }
        else
        {
            StartSegmentInternal(segment2Start, segment2End, endOnVideoEnd: false, useSegmentProgressUI: true);
        }

        yield return new WaitUntil(() => !segmentActive);

        SetAnxietyEnabled(false);

        NarrativeManager.Instance.ResumeNarrative();
        twoSegmentRoutine = null;
    }

    //public void StartTypingSegment1_Assignment()
    //{
    //    StopAnyTwoSegmentRoutine();
    //    NarrativeManager.Instance.PauseNarrative();
    //    SetAnxietyEnabled(false);
    //    StartSegmentInternal(segment1Start, segment1End, endOnVideoEnd: false, useSegmentProgressUI: true);
    //}

    //public void StartTypingSegment2_Email()
    //{
    //    StopAnyTwoSegmentRoutine();
    //    NarrativeManager.Instance.PauseNarrative();
    //    SetAnxietyEnabled(true);

    //    if (segment2EndsAtVideoEnd)
    //        StartSegmentInternal(segment2Start, endTime: -1.0, endOnVideoEnd: true, useSegmentProgressUI: false);
    //    else
    //        StartSegmentInternal(segment2Start, segment2End, endOnVideoEnd: false, useSegmentProgressUI: true);
    //}

    void StartSegmentInternal(double startTime, double endTime, bool endOnVideoEnd, bool useSegmentProgressUI)
    {
        interactionToggle?.EnableInteractions();
        keyboardGlow?.EnableGlow();

        videoPlayer.isLooping = false;
        videoPlayer.clip = typingClip;

        double safeStart = Mathf.Max(0f, (float)startTime);
        videoPlayer.time = safeStart;

        videoPlayer.Play();
        videoPlayer.Pause();

        segmentActive = true;
        finishOnVideoEnd = endOnVideoEnd;

        if (!endOnVideoEnd)
            activeEndTime = endTime;
        else
            activeEndTime = -1.0;

        if (useSegmentProgressUI && !endOnVideoEnd)
            holdUI.Arm(videoPlayer, startTime, endTime);
        else
            holdUI.Arm(videoPlayer);
    }

    void FinishCurrentSegment()
    {
        if (!segmentActive) return;

        SetAnxietyEnabled(false);

        segmentActive = false;
        finishOnVideoEnd = false;
        activeEndTime = -1.0;

        if (videoPlayer != null)
            videoPlayer.Pause();

        if (holdUI != null)
        {
            holdUI.Disarm();
            if (holdUI.typingAudio) holdUI.typingAudio.Stop();
        }

        keyboardGlow?.DisableGlow();
        interactionToggle?.DisableInteractions();
    }

    void SetAnxietyEnabled(bool enabled)
    {
        if (anxietyController == null) return;

        anxietyController.ResetAnxiety();
        anxietyController.enabled = enabled;
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