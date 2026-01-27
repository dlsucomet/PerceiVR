using UnityEngine;

public class KeyboardGlow : MonoBehaviour
{
    [Header("Renderer")]
    public Renderer targetRenderer;

    [Header("Emission")]
    public Color emissionColor = Color.white;
    [Range(0f, 10f)] public float maxIntensity = 2.0f;
    public bool pulse = true;
    public float pulseSpeed = 2.0f;

    MaterialPropertyBlock mpb;
    bool glowOn = false;

    static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    void Awake()
    {
        if (targetRenderer == null) targetRenderer = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
        SetEmission(0f);
    }

    void Update()
    {
        if (!glowOn) return;

        float intensity = maxIntensity;
        if (pulse)
        {
            float p = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            intensity = Mathf.Lerp(maxIntensity * 0.4f, maxIntensity, p);
        }

        SetEmission(intensity);
    }

    public void EnableGlow()
    {
        Debug.Log("KeyboardGlow: EnableGlow");
        glowOn = true;
    }

    public void DisableGlow()
    {
        glowOn = false;
        SetEmission(0f);
    }

    void SetEmission(float intensity)
    {
        if (targetRenderer == null) return;

        Color final = emissionColor * intensity;

        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(EmissionColorID, final);
        targetRenderer.SetPropertyBlock(mpb);
    }
}
