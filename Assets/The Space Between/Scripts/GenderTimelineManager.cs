using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class GenderTimelineManager : MonoBehaviour
{
    public PlayableDirector director;

    void Awake()
    {
        if (director == null) director = GetComponent<PlayableDirector>();
        
        ApplyGenderSettings();
    }

    void ApplyGenderSettings()
    {
        string selectedGender = PlayerPrefs.GetString("PlayerGender", "Male");
        TimelineAsset timeline = (TimelineAsset)director.playableAsset;

        foreach (var track in timeline.GetOutputTracks())
        {
            if (track.name == "Male" && selectedGender == "Female")
            {
                track.muted = true;
            }
            else if (track.name == "Female" && selectedGender == "Male")
            {
                track.muted = true;
            }
            else if (track.name == "Male" || track.name == "Female")
            {
                track.muted = false;
            }
        }

        director.RebuildGraph();
    }
}