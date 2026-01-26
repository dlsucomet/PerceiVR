using System.Collections.Generic;
using UnityEngine;

public class SubtitleNameReplacer : MonoBehaviour
{
    [Header("Subtitle Assets")]
    public List<Subtitle> subtitles;

    private Dictionary<Subtitle, string> originalSpeakerCache = new();
    private Dictionary<Subtitle, string> originalTextCache = new();

    void Awake()
    {
        CacheOriginalData();
        RefreshSubtitles();
    }

    public void RefreshSubtitles()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Student");

        foreach (Subtitle sub in subtitles)
        {
            if (sub == null) continue;

            // restore originals first (undo-safe)
            sub.speaker = originalSpeakerCache[sub];
            sub.subtitleText = originalTextCache[sub];

            // replace speaker
            if (sub.speaker == "Student" || sub.speaker == "Student:" ||
                sub.speaker == "<User>" || sub.speaker == "<User>:")
            {
                sub.speaker = playerName + ":";
            }

            // replace text placeholders
            sub.subtitleText =
                sub.subtitleText.Replace("<User>", playerName);
        }
    }

    void CacheOriginalData()
    {
        originalSpeakerCache.Clear();
        originalTextCache.Clear();

        foreach (Subtitle sub in subtitles)
        {
            if (sub == null) continue;

            if (!originalSpeakerCache.ContainsKey(sub))
                originalSpeakerCache[sub] = sub.speaker;

            if (!originalTextCache.ContainsKey(sub))
                originalTextCache[sub] = sub.subtitleText;
        }
    }
}
