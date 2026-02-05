using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections;

public class GenderTimelineManager : MonoBehaviour
{
    public PlayableDirector director;

    public enum GenderOverride { UsePlayerPrefs, ForceMale, ForceFemale }

    [Header("Debug Settings")]
    public GenderOverride debugGenderOverride = GenderOverride.UsePlayerPrefs;

    void Awake()
    {
        if (director == null) director = GetComponent<PlayableDirector>();

        // Step 1: Mute the tracks
        ApplyGenderMute();
    }

    public void ApplyGenderMute()
    {
        string selectedGender;

        // Determine Gender selection
        if (debugGenderOverride == GenderOverride.ForceMale) selectedGender = "Male";
        else if (debugGenderOverride == GenderOverride.ForceFemale) selectedGender = "Female";
        else selectedGender = PlayerPrefs.GetString("PlayerGender", "Male");

        Debug.Log($"<color=cyan>[Gender System]</color> Muting tracks for: {selectedGender}");

        TimelineAsset timelineAsset = (TimelineAsset)director.playableAsset;
        if (timelineAsset == null) return;

        // Iterate through all tracks
        foreach (var track in timelineAsset.GetOutputTracks())
        {
            // We check the name of the track OR the name of its parent group
            bool isMale = track.name.Contains("Male") || (track.parent != null && track.parent.name.Contains("Male"));
            bool isFemale = track.name.Contains("Female") || (track.parent != null && track.parent.name.Contains("Female"));

            if (isMale && selectedGender == "Female")
            {
                track.muted = true;
                Debug.Log("Muting Male Track: " + track.name);
            }
            else if (isFemale && selectedGender == "Male")
            {
                track.muted = true;
                Debug.Log("Muting Female Track: " + track.name);
            }
            else if (isMale || isFemale)
            {
                // Ensure the correct one is UNMUTED
                track.muted = false;
            }
        }

        // Step 2: Rebuild the audio graph so the mute takes effect
        director.RebuildGraph();

        // Step 3: Start playback manually to avoid the "Timeline Stopped" bug
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        // We wait for the end of the frame to let the Graph settle after muting
        yield return new WaitForEndOfFrame();

        director.Play();

        // Ensure speed is 1 (prevents accidental pause on first frame)
        if (director.playableGraph.IsValid())
        {
            director.playableGraph.GetRootPlayable(0).SetSpeed(1);
        }
    }
}