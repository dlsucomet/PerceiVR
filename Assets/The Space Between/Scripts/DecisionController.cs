using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class DecisionController : MonoBehaviour
{
    [Header("Buttons")]
    public Button choiceAButton;
    public Button choiceBButton;

    [Header("Visual Settings")]
    public Color selectedColor = new Color(0.2f, 0.8f, 0.3f, 1f); // green
    public Color dimmedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    private Color choiceAOriginalColor;
    private Color choiceBOriginalColor;

    [Header("Timing")]
    public float confirmDelay = 0.8f;
    public float fadeOutDuration = 0.6f;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip clickSFX;

    [Header("Screen Fade")]
    public CanvasGroup screenFader;


    private CanvasGroup canvasGroup;
    private bool decisionMade = false;


    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        choiceAOriginalColor = choiceAButton.image.color;
        choiceBOriginalColor = choiceBButton.image.color;
    }


    public void ChooseA(string sceneName)
    {
        if (decisionMade) return;
        StartCoroutine(HandleDecision(choiceAButton, choiceBButton, sceneName));
    }

    public void ChooseB(string sceneName)
    {
        if (decisionMade) return;
        StartCoroutine(HandleDecision(choiceBButton, choiceAButton, sceneName));
    }

    private IEnumerator HandleDecision(Button selected, Button other, string sceneName)
    {
        decisionMade = true;

        // disable decision UI
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // lock in visuals
        selected.image.color = selectedColor;
        selected.transform.localScale = Vector3.one * 1.05f;
        other.image.color = dimmedColor;

        // sound
        if (sfxSource && clickSFX)
            sfxSource.PlayOneShot(clickSFX);

        // short decision pause
        yield return new WaitForSeconds(confirmDelay);

        // fade in full-screen black
        float t = 0f;
        float fadeDuration = fadeOutDuration;
        screenFader.gameObject.SetActive(true);
        screenFader.alpha = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            screenFader.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        screenFader.alpha = 1f;

        // micro-pause before loading scene
        yield return new WaitForSeconds(0.6f);

        // load next scene
        SceneManager.LoadScene(sceneName);
    }

}
