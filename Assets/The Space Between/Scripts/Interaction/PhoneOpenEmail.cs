using UnityEngine;

public class PhoneOpenEmail : MonoBehaviour
{
    public GameObject promptText;
    public GameObject phoneRoot;
    public Behaviour phoneInteractable;
    public InteractionToggle interactionToggle;

    public void OnPhoneClicked()
    {
        if (promptText)
            promptText.SetActive(false);

        if (phoneInteractable)
            phoneInteractable.enabled = false;

        if (phoneRoot)
            phoneRoot.SetActive(false);

        interactionToggle?.DisableInteractions();

        if (NarrativeManager.Instance != null)
        {
            NarrativeManager.Instance.SetResumeModeBoth();
            NarrativeManager.Instance.ResumeNarrative();
        }
    }
}