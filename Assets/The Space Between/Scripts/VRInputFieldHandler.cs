using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class VRInputFieldHandler : MonoBehaviour
{
    [Tooltip("Assign your XR Keyboard GameObject here.")]
    public GameObject xrKeyboard;

    private TMP_InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();

        if (inputField != null)
        {
            // Subscribe to TMP events
            inputField.onSelect.AddListener(OnInputSelected);
            inputField.onDeselect.AddListener(OnInputDeselected);
            inputField.onEndEdit.AddListener(OnInputEndEdit);
        }
    }

    private void OnDestroy()
    {
        if (inputField != null)
        {
            inputField.onSelect.RemoveListener(OnInputSelected);
            inputField.onDeselect.RemoveListener(OnInputDeselected);
            inputField.onEndEdit.RemoveListener(OnInputEndEdit);
        }
    }

    private void OnInputSelected(string text)
    {
        if (xrKeyboard != null)
            xrKeyboard.SetActive(true);
    }

    private void OnInputDeselected(string text)
    {
        // Don't close here — TMP triggers this even when clicking the keyboard
    }

    private void OnInputEndEdit(string text)
    {
        // Actually close the keyboard *only* when editing is finished
        if (xrKeyboard != null)
            xrKeyboard.SetActive(false);
    }
}
