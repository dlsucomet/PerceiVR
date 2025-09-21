using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene To Load")]
    public string prologueSceneName = "Prologue";

    [Header("UI Canvases")]
    public GameObject mainMenuCanvas;
    public GameObject creditsCanvas;
    public bool isCreditsOpen = false;

    void Start()
    {
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(true);
        }
        if (creditsCanvas != null)
        {
            creditsCanvas.SetActive(false);
        }
    }

    public void PlayExperience()
    {
        Debug.Log($"Loading Scene: {prologueSceneName}");
        SceneManager.LoadScene(prologueSceneName);
    }

    public void ShowCredits()
    {
        Debug.Log("Showing Credits Canvas...");
        if (isCreditsOpen == true)
        {
            creditsCanvas.SetActive(false);
            isCreditsOpen = false;
        }
        else
        {
            creditsCanvas.SetActive(true);
            isCreditsOpen = true;
        }
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