using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;
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

    [Header("Fade Transition")]
    public Image blackPanel; // Assign BlackPanel Image here
    public float fadeDuration = 1.5f;

    public FadeManager fadeManager;


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


        fadeManager.FadeAndLoadScene(prologueSceneName);
    
    }

    private IEnumerator FadeAndLoadScene()
    {
        if (blackPanel != null)
        {
            blackPanel.gameObject.SetActive(true);
            Color color = blackPanel.color;
            color.a = 0;
            blackPanel.color = color;

            float elapsed = 0;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsed / fadeDuration);
                blackPanel.color = color;
                yield return null;
            }
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(prologueSceneName);
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
