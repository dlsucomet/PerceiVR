using UnityEngine;

public class InteractableGlow : MonoBehaviour
{
    [Header("Renderers")]
    [Tooltip("If empty, auto-finds all Renderers in children.")]
    public Renderer[] targetRenderers;

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
        if (targetRenderers == null || targetRenderers.Length == 0)
            targetRenderers = GetComponentsInChildren<Renderer>(true);

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
        Debug.Log("InteractableGlow: EnableGlow");
        glowOn = true;
    }

    public void DisableGlow()
    {
        glowOn = false;
        SetEmission(0f);
    }

    void SetEmission(float intensity)
    {
        if (targetRenderers == null) return;

        Color final = emissionColor * intensity;

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            var r = targetRenderers[i];
            if (r == null) continue;

            r.GetPropertyBlock(mpb);
            mpb.SetColor(EmissionColorID, final);
            r.SetPropertyBlock(mpb);
        }
    }
}