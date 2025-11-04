using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinaleManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("TMP Input Field where the user types their reflection.")]
    public TMP_InputField inputField;

    [Tooltip("Full-screen black Image used for fade effect.")]
    public Image fadeImage;

    [Tooltip("Duration of the fade-out in seconds.")]
    public float fadeDuration = 1.5f;

    [Tooltip("Name of the next scene.")]
    public string nextSceneName = "[CHI] Scene_10_1";

    private bool isFading = false;

    public void OnConfirmPressed()
    {
        if (!isFading)
            StartCoroutine(SaveAndFade());
    }

    private IEnumerator SaveAndFade()
    {
        isFading = true;

        string userText = inputField != null ? inputField.text : "";
        SaveUserMessage(userText);

        float elapsed = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, targetColor, elapsed / fadeDuration);
            yield return null;
        }

        Debug.Log("Fade complete. Triggering ChangeScene signal...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);

    }

    private void SaveUserMessage(string message)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "UserNotes");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, "UserMessage.txt");
        File.WriteAllText(filePath, message);

        Debug.Log($"User message saved to: {filePath}");
    }
}
