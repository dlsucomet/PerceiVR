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

    public bool IsHoldingTyping => holding;

    bool requireRelease = false;

    void Start()
    {
        if (holdPrompt) holdPrompt.SetActive(false);
        if (progressRing)
        {
            progressRing.fillAmount = 0f;
            progressRing.gameObject.SetActive(false);
        }
    }

    public void Arm(VideoPlayer vp)
    {
        videoPlayer = vp;
        armed = true;
        holding = false;

        if (holdPrompt) holdPrompt.SetActive(true);
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

        if (videoPlayer != null) videoPlayer.Pause();
        if (typingAudio != null) typingAudio.Stop();

        if (holdPrompt) holdPrompt.SetActive(false);
        if (progressRing) progressRing.gameObject.SetActive(false);

        videoPlayer = null;
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log("HoldToPlay: PointerDown");

    //    if (!armed || videoPlayer == null) return;

    //    holding = true;
    //    if (holdPrompt) holdPrompt.SetActive(false);

    //    videoPlayer.Play();

    //    if (typingAudio && !typingAudio.isPlaying)
    //    {
    //        typingAudio.Play();
    //    }
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    Debug.Log("HoldToPlay: PointerUp");

    //    if (!armed || videoPlayer == null) return;

    //    holding = false;

    //    videoPlayer.Pause();

    //    if (typingAudio && typingAudio.isPlaying)
    //    {
    //        typingAudio.Pause();
    //    }

    //    if (holdPrompt) holdPrompt.SetActive(true);
    //}

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!armed || videoPlayer == null) return;

        if (requireRelease) return;

        holding = true;
        if (holdPrompt) holdPrompt.SetActive(false);

        videoPlayer.Play();
        if (typingAudio && !typingAudio.isPlaying) typingAudio.Play();

        TypingStarted?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!armed || videoPlayer == null) return;

        holding = false;

        videoPlayer.Pause();
        if (typingAudio && typingAudio.isPlaying) typingAudio.Pause();

        if (holdPrompt) holdPrompt.SetActive(true);

        if (requireRelease) requireRelease = false;

        TypingStopped?.Invoke();
    }

    void Update()
    {
        if (!armed || progressRing == null || videoPlayer == null) return;
        if (videoPlayer.length <= 0.0001) return;

        progressRing.fillAmount = (float)(videoPlayer.time / videoPlayer.length);
    }

    public void ForceBreak()
    {
        holding = false;
        requireRelease = true;

        if (videoPlayer != null) videoPlayer.Pause();
        if (typingAudio != null) typingAudio.Pause();

        if (holdPrompt) holdPrompt.SetActive(true);
    }

}
