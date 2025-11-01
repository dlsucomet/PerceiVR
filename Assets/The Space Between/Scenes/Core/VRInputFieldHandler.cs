using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class VRInputFieldHandler : MonoBehaviour, ISelectHandler
{
    public TMP_InputField myInputField;
    private TouchScreenKeyboard keyboard;

    void Start()
    {
        if (myInputField == null)
            myInputField = GetComponent<TMP_InputField>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        keyboard = TouchScreenKeyboard.Open(
            myInputField.text ?? "",
            TouchScreenKeyboardType.Default,
            false, false, false, false,
            "Enter Text"
        );
    }

    void Update()
    {
        if (keyboard != null)
        {
            if (keyboard.active)
            {
                myInputField.text = keyboard.text;
            }

            if (keyboard.status == TouchScreenKeyboard.Status.Done ||
                keyboard.status == TouchScreenKeyboard.Status.Canceled)
            {
                keyboard = null;
            }
        }
    }
}
