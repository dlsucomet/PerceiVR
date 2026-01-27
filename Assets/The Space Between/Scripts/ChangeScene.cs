using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is empty. Cannot load scene.");
            return;
        }

        Debug.Log("Loading scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    
    public void SkipToNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"Skipping to scene index: {nextIndex}");
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning("Final scene reached. Nowhere to skip to!");
        }
    }

    public void SkipToPreviousScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int prevIndex = currentIndex - 1;

        if (prevIndex >= 0)
        {
            Debug.Log($"Going back to scene index: {prevIndex}");
            SceneManager.LoadScene(prevIndex);
        }
        else
        {
            Debug.LogWarning("You are at the very first scene!");
        }
    }


}