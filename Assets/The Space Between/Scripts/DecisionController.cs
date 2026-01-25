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
    public float confirmDelay = 0.6f;
    public float fadeOutDuration = 0.4f;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip clickSFX;

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

    // public void ResetDecisionUI()
    // {
    //     decisionMade = false;
    //     canvasGroup.alpha = 1f;
    //     canvasGroup.interactable = true;
    //     canvasGroup.blocksRaycasts = true;

    //     choiceAButton.image.color = choiceAOriginalColor;
    //     choiceBButton.image.color = choiceBOriginalColor;
    //     choiceAButton.transform.localScale = Vector3.one;
    //     choiceBButton.transform.localScale = Vector3.one;
        
    // }


    private IEnumerator HandleDecision(Button selected, Button other, string sceneName)
    {
        decisionMade = true;

        // disable input
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // visual lock-in
        selected.image.color = selectedColor;
        selected.transform.localScale = Vector3.one * 1.05f;
        other.image.color = dimmedColor;

        // sound
        if (sfxSource && clickSFX)
            sfxSource.PlayOneShot(clickSFX);

        // decision pause 
        yield return new WaitForSeconds(confirmDelay);

        // smooth fade
        float t = 0f;
        float startAlpha = canvasGroup.alpha;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeOutDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;

        // cinematic micro-pause before scene load
        yield return new WaitForSeconds(0.4f);

        // delayed scene load
        SceneManager.LoadScene(sceneName);
    }

}
