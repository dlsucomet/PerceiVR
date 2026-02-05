using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Collider))]
public class HallwayThoughtTrigger : MonoBehaviour
{
    [Header("Thought")]
    public ThoughtsController thoughtToPlay;

    [Header("Audio (existing source)")]
    public AudioSource audioToPlay;
    public bool restartIfAlreadyPlaying = true;
    [Min(0f)] public float audioDelay = 0.15f;

    [Header("Resume narrative")]
    public bool resumeViaNarrativeManager = true;
    public PlayableDirector directorToResume;

    [Header("Trigger settings")]
    public bool oneShot = true;

    bool fired;
    Coroutine audioRoutine;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!HasPlayerInParents(other.transform)) return;
        if (oneShot && fired) return;
        fired = true;

        if (thoughtToPlay != null)
            thoughtToPlay.PlayText();

        if (audioToPlay != null)
        {
            if (audioRoutine != null) StopCoroutine(audioRoutine);
            audioRoutine = StartCoroutine(PlayAudioDelayed());
        }

        ResumeNarrative();
    }

    IEnumerator PlayAudioDelayed()
    {
        if (audioDelay > 0f)
            yield return new WaitForSeconds(audioDelay);

        if (audioToPlay == null) yield break;

        if (restartIfAlreadyPlaying && audioToPlay.isPlaying)
            audioToPlay.Stop();

        audioToPlay.Play();
    }

    void ResumeNarrative()
    {
        if (resumeViaNarrativeManager && NarrativeManager.Instance != null)
        {
            NarrativeManager.Instance.ResumeNarrative();
            return;
        }

        if (directorToResume != null)
            directorToResume.Play();
    }

    bool HasPlayerInParents(Transform t)
    {
        while (t != null)
        {
            if (t.CompareTag("Player")) return true;
            t = t.parent;
        }
        return false;
    }
}