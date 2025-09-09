using UnityEngine;

[CreateAssetMenu(fileName = "New Subtitle", menuName = "Subtitles/Subtitle")]
public class Subtitle : ScriptableObject
{
    [TextArea(3, 10)]
    public string subtitleText;
    public string speaker;
    public float duration = 3f;
}