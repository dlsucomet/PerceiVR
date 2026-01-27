using UnityEngine;

public class HeadphonesPromptController : MonoBehaviour
{
    [Header("References")]
    public GameObject promptText;
    public Behaviour grabInteractable;
    public InteractionToggle interactionToggle;
    public InteractableGlow headphonesGlow;

    public void StartHeadphonesPrompt()
    {
        if (NarrativeManager.Instance != null)
        {
            NarrativeManager.Instance.SetResumeModeInteractionOnly();
            NarrativeManager.Instance.PauseNarrative();
        }

        interactionToggle?.EnableInteractions();

        if (grabInteractable)
            grabInteractable.enabled = true;

        if (promptText)
            promptText.SetActive(true);

        if (headphonesGlow)
        {
            Debug.Log("HeadphonesPromptController: enabling glow");
            headphonesGlow.EnableGlow();
        }
    }
}