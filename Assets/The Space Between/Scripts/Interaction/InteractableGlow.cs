using UnityEngine;

public class InteractableGlow : MonoBehaviour
{
    [Header("Glow Settings")]
    public Color glowColor = Color.cyan;
    public float glowIntensity = 2f;

    Renderer[] renderers;
    Color[] originalEmission;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalEmission = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_EmissionColor"))
                originalEmission[i] = renderers[i].material.GetColor("_EmissionColor");
        }
    }

    public void EnableGlow()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (!renderers[i].material.HasProperty("_EmissionColor")) continue;

            renderers[i].material.EnableKeyword("_EMISSION");
            renderers[i].material.SetColor("_EmissionColor", glowColor * glowIntensity);
        }
    }

    public void DisableGlow()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (!renderers[i].material.HasProperty("_EmissionColor")) continue;

            renderers[i].material.SetColor("_EmissionColor", originalEmission[i]);
        }
    }
}