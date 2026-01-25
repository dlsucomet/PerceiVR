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

    void OnEnable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnClipFinished;
    }

    void OnDisable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnClipFinished;
    }

    public void StartTypingBeat()
    {
        Debug.Log("Starting Typing Beat...");
        if (videoPlayer == null || holdUI == null || typingClip == null) return;

        NarrativeManager.Instance.PauseNarrative();

        keyboardGlow?.EnableGlow();

        videoPlayer.clip = typingClip;
        videoPlayer.time = 0;
        videoPlayer.Play();
        videoPlayer.Pause();

        holdUI.Arm(videoPlayer);
    }

    void OnClipFinished(VideoPlayer vp)
    {
        vp.Pause();

        // Hide prompt/progress and stop interaction
        holdUI.Disarm();
        keyboardGlow?.DisableGlow();

        if (holdUI.typingAudio) holdUI.typingAudio.Stop();

        interactionToggle?.DisableInteractions();

        NarrativeManager.Instance.ResumeNarrative();
    }
}
