using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigationButton : MonoBehaviour
{

    [Tooltip("Scene Name")]
    public string sceneNameToLoad;

    public void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(sceneNameToLoad))
        {
            Debug.LogError("assign a scene name in the Inspector");
            return;
        }

        Debug.Log($"Loading scene: {sceneNameToLoad}");
        SceneManager.LoadScene(sceneNameToLoad);
    }
}