using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    [Header("UI Canvases")]
    public GameObject buttonCanvas;      // Main menu buttons
    public GameObject playerInputCanvas; // Name input form

    [Header("Player Input Field")]
    public TMP_InputField nameInputField;
    public string nextSceneName = "Scene_1"; // Name of the next scene to load

    private string playerName;

    void Start()
    {
        // Show only main menu at start
        buttonCanvas.SetActive(true);
        playerInputCanvas.SetActive(false);
    }

    public void OnStartButtonClicked()
    {
        // Hide main menu, show name input
        buttonCanvas.SetActive(false);
        playerInputCanvas.SetActive(true);
    }

    public void OnConfirmButtonClicked()
    {
        playerName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Name field is empty!");
            return;
        }

        Debug.Log("Player name set to: " + playerName);

        // Save the name for the next scene
        PlayerPrefs.SetString("PlayerName", playerName);

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
