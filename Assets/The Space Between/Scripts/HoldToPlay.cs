using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class HoldToPlay : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI")]
    public GameObject holdPrompt;
    public Image progressRing;

    VideoPlayer videoPlayer;
    bool armed = false;
    bool holding = false;

    public AudioSource typingAudio;

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

        if (videoPlayer != null)
            videoPlayer.Pause();

        if (holdPrompt) holdPrompt.SetActive(false);
        if (progressRing) progressRing.gameObject.SetActive(false);

        videoPlayer = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("HoldToPlay: PointerDown");

        if (!armed || videoPlayer == null) return;

        holding = true;
        if (holdPrompt) holdPrompt.SetActive(false);

        videoPlayer.Play();

        if (typingAudio && !typingAudio.isPlaying)
        {
            typingAudio.Play();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("HoldToPlay: PointerUp");

        if (!armed || videoPlayer == null) return;

        holding = false;

        videoPlayer.Pause();

        if (typingAudio && typingAudio.isPlaying)
        {
            typingAudio.Pause();
        }

        if (holdPrompt) holdPrompt.SetActive(true);
    }

    void Update()
    {
        if (!armed || progressRing == null || videoPlayer == null) return;
        if (videoPlayer.length <= 0.0001) return;

        progressRing.fillAmount = (float)(videoPlayer.time / videoPlayer.length);
    }
}
