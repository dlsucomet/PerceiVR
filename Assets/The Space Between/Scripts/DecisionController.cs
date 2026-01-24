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

    [Header("Timing")]
    public float confirmDelay = 0.4f;

    [Header("Fade")]
    public Animator blackPanelAnimator;
    public float fadeDuration = 0.5f;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip clickSFX;

    private bool decisionMade = false;
    private string targetScene;

    public void ChooseA(string sceneName)
    {
        if (decisionMade) return;
        targetScene = sceneName;
        StartCoroutine(HandleDecision(choiceAButton, choiceBButton));
    }

    public void ChooseB(string sceneName)
    {
        if (decisionMade) return;
        targetScene = sceneName;
        StartCoroutine(HandleDecision(choiceBButton, choiceAButton));
    }

    private IEnumerator HandleDecision(Button selected, Button other)
    {
        decisionMade = true;

        // Lock visuals
        selected.image.color = selectedColor;
        other.image.color = dimmedColor;

        // Disable buttons
        choiceAButton.interactable = false;
        choiceBButton.interactable = false;

        // Sound
        if (sfxSource && clickSFX)
            sfxSource.PlayOneShot(clickSFX);

        // Micro-delay
        yield return new WaitForSeconds(confirmDelay);

        // Trigger fade
        if (blackPanelAnimator)
            blackPanelAnimator.ResetTrigger("FadeIn");
            blackPanelAnimator.SetTrigger("FadeOut");

        // Wait for fade animation
        yield return new WaitForSeconds(fadeDuration);

        SceneManager.LoadScene(targetScene);
    }
}
