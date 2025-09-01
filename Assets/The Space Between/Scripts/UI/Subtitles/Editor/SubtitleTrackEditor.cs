using UnityEditor.Timeline;
using UnityEngine.Timeline;

[CustomTimelineEditor(typeof(SubtitleTrack))]
public class SubtitleTrackEditor : TrackEditor
{
    public override TrackBindingFlags GetBindingFlags()
    {
        return TrackBindingFlags.AllowCreate;
    }
}