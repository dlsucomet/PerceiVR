using UnityEngine;
using TMPro;
using System.Collections;

public class ThoughtsController : MonoBehaviour
{
    [Header("Text Settings")]
    public TextMeshPro textMesh;
    [TextArea] public string fullText;

    [Header("Timings")]
    public float typewriterSpeed = 0.05f;
    public float visibleDuration = 5f;
    public float fadeTime = 1f;

    [Header("Debug (Read Only)")]
    [SerializeField, Tooltip("Total duration including fade-in, typewriter, visible time, and fade-out.")]
    private float totalDuration;

    private Coroutine currentRoutine;

    void OnValidate()
    {
        UpdateTotalDuration();
    }

    void Awake()
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshPro>();
        textMesh.text = "";
        textMesh.alpha = 0;
    }

    public void PlayText()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowTextRoutine());
    }

    IEnumerator ShowTextRoutine()
    {
        // Fade in
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            textMesh.alpha = Mathf.Lerp(0, 1, t / fadeTime);
            yield return null;
        }

        // Typewriter effect
        for (int i = 0; i <= fullText.Length; i++)
        {
            textMesh.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(typewriterSpeed);
        }

        // Visible duration
        yield return new WaitForSeconds(visibleDuration);

        // Fade out
        t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            textMesh.alpha = Mathf.Lerp(1, 0, t / fadeTime);
            yield return null;
        }

        textMesh.text = "";
    }

    private void UpdateTotalDuration()
    {
        float typeTime = fullText != null ? fullText.Length * typewriterSpeed : 0f;
        totalDuration = (2 * fadeTime) + typeTime + visibleDuration;
    }
}
