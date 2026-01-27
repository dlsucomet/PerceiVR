using UnityEngine;
using TMPro;
using System.Collections;

public class IntroTextSignalController : MonoBehaviour
{
    public TextMeshProUGUI introTextUI;
    public CanvasGroup canvasGroup;

    [Header("Intro Text Sequence")]
    [TextArea(3, 6)]
    public string[] introTexts;

    public float fadeDuration = 0.8f;

    private int index = -1;
    private bool isTransitioning = false;

    void Start()
    {
        canvasGroup.alpha = 0;
    }

    // SIGNAL: Move to next text
    public void NextTextSignal()
    {
        if (isTransitioning) return;

        if (index < introTexts.Length - 1)
        {
            index++;
            StartCoroutine(FadeToNextText());
        }
        else
        {
            Debug.Log("Intro sequence finished!");
        }
    }

    // ✅ SIGNAL: Reset sequence
    public void ResetTextSignal()
    {
        StopAllCoroutines();
        isTransitioning = false;

        index = -1;
        introTextUI.text = "";
        canvasGroup.alpha = 0;
    }

    IEnumerator FadeToNextText()
    {
        isTransitioning = true;

        // Fade out current (if visible)
        if (canvasGroup.alpha > 0)
        {
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 0;
        }

        // Update text
        introTextUI.text = introTexts[index];

        // Fade in new text
        float t2 = 0;
        while (t2 < fadeDuration)
        {
            t2 += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t2 / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1;
        isTransitioning = false;
    }
}
