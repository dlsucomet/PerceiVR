using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

[RequireComponent(typeof(AudioSource))]
public class VRButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [Header("Audio Clips")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Haptic Feedback")]
    [Range(0, 1)] public float hapticIntensity = 0.7f;
    public float hapticDuration = 0.1f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        
        if (eventData.currentInputModule is XRUIInputModule uiInputModule)
        {
            if (uiInputModule.GetInteractor(eventData.pointerId) is UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor controllerInteractor)
            {
                controllerInteractor.xrController.SendHapticImpulse(hapticIntensity, hapticDuration);
            }
        }
    }
}