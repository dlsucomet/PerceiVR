using UnityEngine;

public class PhonePromptController : MonoBehaviour
{
    [Header("References")]
    public GameObject promptText;
    public GameObject phoneRoot;
    public Behaviour phoneInteractable;
    public InteractionToggle interactionToggle;

    public void StartPhonePrompt()
    {
        if (NarrativeManager.Instance != null)
        {
            NarrativeManager.Instance.SetResumeModeInteractionOnly();
            NarrativeManager.Instance.PauseNarrative();
        }

        interactionToggle?.EnableInteractions();

        if (promptText)
            promptText.SetActive(true);

        if (phoneRoot)
            phoneRoot.SetActive(true);

        if (phoneInteractable)
            phoneInteractable.enabled = true;
    }
}