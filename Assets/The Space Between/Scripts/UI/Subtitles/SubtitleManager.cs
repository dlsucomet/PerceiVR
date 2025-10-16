using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SubtitleManager : MonoBehaviour
{
    public GameObject subtitleContainer;
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI speakerText;

    public List<Subtitle> subtitleSequence;
    private int currentIndex = 0;

    void Start()
    {
        subtitleContainer.SetActive(false);
    }

    public void DisplayNextSubtitle()
    {
        if (currentIndex < subtitleSequence.Count)
        {
            Subtitle subtitle = subtitleSequence[currentIndex];

            subtitleContainer.SetActive(true);
            subtitleText.text = subtitle.subtitleText;

            if (speakerText != null)
            {
                // Use runtimeSpeakerName if available
                string displayName = !string.IsNullOrEmpty(subtitle.runtimeSpeakerName)
                    ? subtitle.runtimeSpeakerName
                    : subtitle.speaker; // fallback

                if (!string.IsNullOrEmpty(displayName))
                {
                    speakerText.gameObject.SetActive(true);
                    speakerText.text = displayName; // CHANGED
                }
                else
                {
                    speakerText.gameObject.SetActive(false);
                }
            }

            if (subtitle.duration > 0)
            {
                CancelInvoke("HideSubtitle");
                Invoke("HideSubtitle", subtitle.duration);
            }

            currentIndex++;
        }
        else
        {
            Debug.Log("End of subtitle sequence.");
            HideSubtitle();
        }
    }

    public void ResetSubtitleIndex()
    {
        currentIndex = 0;
    }

    public void HideSubtitle()
    {
        subtitleContainer.SetActive(false);
    }
}
