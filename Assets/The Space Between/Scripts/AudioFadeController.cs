using UnityEngine;

public class AudioFadeController : MonoBehaviour
{
    public AudioSource audioSource;
    private float targetVolume;
    private float fadeSpeed;
    private float currentFadeDuration = 2.0f;

    void Update()
    {
        if (!Mathf.Approximately(audioSource.volume, targetVolume))
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.unscaledDeltaTime);
        }
    }

    public void SetTargetVolume(float volume)
    {
        targetVolume = Mathf.Clamp01(volume);
        CalculateFadeSpeed();
    }

    public void SetFadeDuration(float duration)
    {
        currentFadeDuration = Mathf.Max(0.001f, duration);
        CalculateFadeSpeed();
    }

    private void CalculateFadeSpeed()
    {
        float distance = Mathf.Abs(audioSource.volume - targetVolume);
        fadeSpeed = distance / currentFadeDuration;
    }
}