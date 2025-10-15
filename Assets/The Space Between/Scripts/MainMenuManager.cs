using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene To Load")]
    public string prologueSceneName = "Scene_1";

    [Header("UI Canvases")]
    public GameObject buttonCanvas;      // Start, Credits, Quit
    public GameObject creditsCanvas;
    public GameObject playerInputCanvas; // Input field screen
    public bool isCreditsOpen = false;

    [Header("Player Input Field")]
    public TMP_InputField nameInputField;

    void Start()
    {
        buttonCanvas.SetActive(true);
        playerInputCanvas.SetActive(false);
        if (creditsCanvas != null) creditsCanvas.SetActive(false);
    }

    // --- Called when user presses "Start" ---
    public void OnStartButtonClicked()
    {
        Debug.Log("Start button clicked — showing player input.");
        buttonCanvas.SetActive(false);
        playerInputCanvas.SetActive(true);
    }

    // --- Called when user confirms their name ---
    public void OnConfirmButtonClicked()
    {
        string playerName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Name field is empty!");
            return;
        }

        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();

        Debug.Log("✅ Player name saved: " + playerName);

        SceneManager.LoadScene(prologueSceneName);
    }

    public void ShowCredits()
    {
        isCreditsOpen = !isCreditsOpen;
        creditsCanvas.SetActive(isCreditsOpen);
    }

    public void QuitExperience()
    {
        Debug.Log("Quitting application...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
