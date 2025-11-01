using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage; // A fullscreen UI Image with black color
    public float fadeDuration = 1f;

    private void Start()
    {
        if (fadeImage != null)
        {
            // Start with a transparent image
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    public void FadeAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float t = 0f;

        // Fade to black
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Load the next scene only after fade completes
        SceneManager.LoadScene(sceneName);
    }
}
