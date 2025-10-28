using System.Collections.Generic;
using UnityEngine;

public class SubtitleNameReplacer : MonoBehaviour
{
    [Header("Assign your subtitle assets here")]
    public List<Subtitle> subtitlesToReplace;

    void Awake()
    {
        ReplaceStudentNames();
    }

    private void ReplaceStudentNames()
    {
        string savedName = PlayerPrefs.GetString("PlayerName", "");

        if (string.IsNullOrEmpty(savedName))
        {
            Debug.LogWarning("No player name found. Using default 'Student'.");
            savedName = "Student";
        }

        if (subtitlesToReplace == null || subtitlesToReplace.Count == 0)
        {
            Debug.LogWarning("No subtitles assigned to SubtitleNameReplacer.");
            return;
        }

        int replacedCount = 0;

        foreach (Subtitle sub in subtitlesToReplace)
        {
            if (sub == null) continue;

            // Replace speaker name variations
            string speaker = sub.speaker.Trim();
            if (speaker == "Student" || speaker == "Student:" ||
                speaker == "<User>" || speaker == "<User>:")
            {
                // Ensure it always has a colon at the end
                sub.speaker = savedName.EndsWith(":") ? savedName : savedName + ":";
                replacedCount++;
            }

            // Replace name placeholders in subtitle text
            if (!string.IsNullOrEmpty(sub.subtitleText))
            {
                string newText = sub.subtitleText
                    .Replace("<User>", savedName);
                sub.subtitleText = newText;
            }
        }

        Debug.Log($"Replaced 'Student'/'<User>' with '{savedName}' in {replacedCount} subtitles.");
    }
}
