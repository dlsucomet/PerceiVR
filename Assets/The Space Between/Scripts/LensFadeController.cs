using UnityEngine;

public class LensFadeController : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 2f;
    public bool fadeOutOnStart = false;

    private Material mat;
    private Color startColor;
    private float timer = 0f;
    private bool isFading = false;
    private bool isFadingOut = true;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        startColor = mat.color;

        if (fadeOutOnStart)
            StartFadeOut();
    }

    void Update()
    {
        if (!isFading) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / fadeDuration);

        Color newColor = startColor;
        if (isFadingOut)
            newColor.a = Mathf.Lerp(1f, 0.05f, t);
        else
            newColor.a = Mathf.Lerp(0f, 1f, t);

        mat.color = newColor;

        if (t >= 1f)
            isFading = false;
    }

    public void StartFadeOut()
    {
        isFading = true;
        isFadingOut = true;
        timer = 0f;
    }

    public void StartFadeIn()
    {
        isFading = true;
        isFadingOut = false;
        timer = 0f;
    }
}
