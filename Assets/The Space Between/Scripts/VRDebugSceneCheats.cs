using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR; // Required for VR Input
using System.Collections.Generic;

public class VRDebugSceneCheats : MonoBehaviour
{
    private bool isGripPressed = false;

    void Update()
    {
        // 1. Get the Left Hand Device
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        // 2. Try to get the Grip Button usage value
        if (leftHand.TryGetFeatureValue(CommonUsages.gripButton, out bool gripValue))
        {
            // 3. Check for the "Pressed" event (Debouncing)
            if (gripValue && !isGripPressed)
            {
                // Button was JUST pressed
                isGripPressed = true;
                SkipToNextScene();
            }
            else if (!gripValue && isGripPressed)
            {
                // Button was JUST released
                isGripPressed = false;
            }
        }

        // Optional: Keep Keyboard 'N' as a backup for when you aren't wearing the headset
        if (Application.isEditor && Input.GetKeyDown(KeyCode.N))
        {
            SkipToNextScene();
        }
    }

    public void SkipToNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"<color=cyan>VR Debug:</color> Skipping to scene: {nextIndex}");
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning("You are at the last scene!");
        }
    }
}