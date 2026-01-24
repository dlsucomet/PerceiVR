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
    public Color selectedColor = Color.white;
    public Color dimmedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    private Color choiceAOriginalColor;
    private Color choiceBOriginalColor;

    [Header("Timing")]
    public float confirmDelay = 0.4f;
    public float fadeOutDuration = 0.3f;

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

    public void ResetDecisionUI()
    {
        decisionMade = false;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        choiceAButton.image.color = choiceAOriginalColor;
        choiceBButton.image.color = choiceBOriginalColor;
    }


    private IEnumerator HandleDecision(Button selected, Button other, string sceneName)
    {
        decisionMade = true;

        // Disable input
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Visual lock-in
        selected.image.color = choiceAOriginalColor * 1.1f;
        other.image.color = dimmedColor;

        // Sound
        if (sfxSource && clickSFX)
            sfxSource.PlayOneShot(clickSFX);

        // Micro-delay
        yield return new WaitForSeconds(confirmDelay);

        // Fade out
        float t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;

        // Load scene
        SceneManager.LoadScene(sceneName);
    }
}
