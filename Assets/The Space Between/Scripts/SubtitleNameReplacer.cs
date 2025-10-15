using UnityEngine;
using System.Collections.Generic;

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
        string savedName = PlayerInputManager.playerName;

        if (string.IsNullOrEmpty(savedName))
        {
            Debug.LogWarning("⚠️ No player name found. Using default 'Student'.");
            return;
        }

        if (subtitlesToReplace == null || subtitlesToReplace.Count == 0)
        {
            Debug.LogWarning("⚠️ No subtitles assigned to SubtitleNameReplacer.");
            return;
        }

        int replacedCount = 0;

        foreach (Subtitle sub in subtitlesToReplace)
        {
            if (sub == null) continue;

            if (sub.speaker == "Student")
            {
                sub.speaker = savedName;
                replacedCount++;
            }
        }

        Debug.Log($"✅ Replaced 'Student' with '{savedName}' in {replacedCount} subtitles.");
    }
}
