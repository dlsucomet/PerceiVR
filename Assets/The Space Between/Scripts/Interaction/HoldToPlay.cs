using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using System;

public class HoldToPlay : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI")]
    public GameObject holdPrompt;
    public Image progressRing;

    VideoPlayer videoPlayer;
    bool armed = false;
    bool holding = false;

    public AudioSource typingAudio;

    public event Action TypingStarted;
    public event Action TypingStopped;

    bool requireRelease = false;
    bool suppressHoldPrompt = false;

    public bool IsReleaseRequired => requireRelease;
    public bool IsHoldingTyping => holding;

    bool useSegmentRange = false;
    double segmentStart = 0.0;
    double segmentEnd = 0.0;

    void Start()
    {
        if (holdPrompt) holdPrompt.SetActive(false);
        if (progressRing) progressRing.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!armed || progressRing == null || videoPlayer == null)
            return;

        if (useSegmentRange)
        {
            double len = segmentEnd - segmentStart;
            if (len > 0.0001)
            {
                progressRing.fillAmount =
                    Mathf.Clamp01((float)((videoPlayer.time - segmentStart) / len));
            }
        }
        else
        {
            if (videoPlayer.length > 0.0001)
                progressRing.fillAmount =
                    (float)(videoPlayer.time / videoPlayer.length);
        }
    }

    public void SetHoldPromptSuppressed(bool suppressed)
    {
        suppressHoldPrompt = suppressed;
        if (holdPrompt)
            holdPrompt.SetActive(armed && !holding && !suppressHoldPrompt);
    }

    public void Arm(VideoPlayer vp)
    {
        videoPlayer = vp;
        armed = true;
        holding = false;
        requireRelease = false;

        useSegmentRange = false;
        segmentStart = 0.0;
        segmentEnd = 0.0;

        if (holdPrompt)
            holdPrompt.SetActive(!suppressHoldPrompt);
        if (progressRing)
        {
            progressRing.fillAmount = 0f;
            progressRing.gameObject.SetActive(true);
        }
    }

    public void Arm(VideoPlayer vp, double startTime, double endTime)
    {
        videoPlayer = vp;
        armed = true;
        holding = false;
        requireRelease = false;

        useSegmentRange = true;
        segmentStart = startTime;
        segmentEnd = Mathf.Max((float)startTime + 0.0001f, (float)endTime);

        if (holdPrompt)
            holdPrompt.SetActive(!suppressHoldPrompt);

        if (progressRing)
        {
            progressRing.fillAmount = 0f;
            progressRing.gameObject.SetActive(true);
        }
    }

    public void Disarm()
    {
        armed = false;
        holding = false;
        requireRelease = false;

        useSegmentRange = false;
        segmentStart = 0.0;
        segmentEnd = 0.0;

        if (videoPlayer) videoPlayer.Pause();
        if (typingAudio) typingAudio.Stop();

        if (holdPrompt) holdPrompt.SetActive(false);
        if (progressRing) progressRing.gameObject.SetActive(false);

        videoPlayer = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!armed || requireRelease) return;

        holding = true;
        if (holdPrompt) holdPrompt.SetActive(false);

        videoPlayer.Play();
        typingAudio?.Play();

        TypingStarted?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        holding = false;

        videoPlayer.Pause();
        typingAudio?.Pause();

        if (requireRelease)
            requireRelease = false;

        if (holdPrompt)
            holdPrompt.SetActive(!suppressHoldPrompt);

        TypingStopped?.Invoke();
    }

    public void ForceBreak()
    {
        holding = false;
        requireRelease = true;

        videoPlayer.Pause();
        typingAudio?.Pause();

        if (holdPrompt)
            holdPrompt.SetActive(!suppressHoldPrompt);
    }
}