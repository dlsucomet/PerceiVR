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
    
    private string finalUserText = "";

    public void OnInputEndEdit()
    {
        if (inputField != null)
        {
            finalUserText = inputField.text;
            Debug.Log("Input field editing finished. Text captured: '" + finalUserText + "'");
        }
    }

    public void OnConfirmPressed()
    {
        if (!isFading)
            StartCoroutine(SaveAndFade());
    }

    private IEnumerator SaveAndFade()
    {
        isFading = true;

        SaveUserMessage(finalUserText);

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

        string currentName = PlayerPrefs.GetString("PlayerName", "Anonymous");
        NoteData note = new NoteData { playerName = currentName, playerMessage = message };
        string json = JsonUtility.ToJson(note, true);
        string folderPath = Path.Combine(Application.persistentDataPath, "UserNotes");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"Note_{timeStamp}.txt";
        string filePath = Path.Combine(folderPath, fileName);
        File.WriteAllText(filePath, json);
        Debug.Log($"Note saved for {currentName} to: {filePath}");
    }
}