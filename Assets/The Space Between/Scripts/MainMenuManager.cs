using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene To Load")]
    public string prologueSceneName = "Scene_1";

    [Header("UI Canvases")]
    public GameObject buttonCanvas;
    public GameObject creditsCanvas;
    public GameObject playerInputCanvas;
    public GameObject genderSelectionCanvas;
    public GameObject notesCanvas;

    [Header("Player Input Field")]
    public TMP_InputField nameInputField;

    [Header("Notes UI")]
    public RectTransform notesContentArea;
    public GameObject noteEntryPrefab;

    [Header("Fade Transition")]
    public Image blackPanel;
    public float fadeDuration = 1.5f;
    public FadeManager fadeManager;

    private bool isCreditsOpen = false;

    void Start()
    {
        playerInputCanvas.SetActive(false);
        genderSelectionCanvas.SetActive(false);
        if (creditsCanvas != null) creditsCanvas.SetActive(false);
        if (notesCanvas != null) notesCanvas.SetActive(false);
    }

    public void OnStartButtonClicked()
    {
        buttonCanvas.SetActive(false);
        playerInputCanvas.SetActive(true);
    }

    public void OnConfirmNameButtonClicked()
    {
        string playerName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Name field is empty!");
            return;
        }

        nameInputField.DeactivateInputField();

        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();

        playerInputCanvas.SetActive(false);
        genderSelectionCanvas.SetActive(true);
    }

    public void OnGenderSelected(string gender)
    {
        PlayerPrefs.SetString("PlayerGender", gender);
        PlayerPrefs.Save();

        Debug.Log($"Saved Player: {PlayerPrefs.GetString("PlayerName")} as {gender}");

        genderSelectionCanvas.SetActive(false);

        if (fadeManager != null)
        {
            fadeManager.FadeAndLoadScene(prologueSceneName);
        }
        else
        {
            StartCoroutine(FadeAndLoadSceneCoroutine());
        }
    }

    public void ShowCredits()
    {
        isCreditsOpen = !isCreditsOpen;
        creditsCanvas.SetActive(isCreditsOpen);
    }

    public void QuitExperience()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnViewNotesClicked()
    {
        buttonCanvas.SetActive(false);
        notesCanvas.SetActive(true);
        LoadAndDisplayNotes();
    }

    public void OnCloseNotesClicked()
    {
        notesCanvas.SetActive(false);
        buttonCanvas.SetActive(true);
    }

    private void LoadAndDisplayNotes()
    {
        foreach (Transform child in notesContentArea)
        {
            Destroy(child.gameObject);
        }

        string folderPath = Path.Combine(Application.persistentDataPath, "UserNotes");
        if (Directory.Exists(folderPath))
        {
            string[] noteFiles = Directory.GetFiles(folderPath, "*.txt");
            if (noteFiles.Length == 0)
            {
                InstantiateNote(new NoteData { playerName = "System", playerMessage = "You haven't written any notes yet." });
            }
            else
            {
                foreach (string filePath in noteFiles)
                {
                    string json = File.ReadAllText(filePath);
                    NoteData noteData = JsonUtility.FromJson<NoteData>(json);
                    InstantiateNote(noteData);
                }
            }
        }
    }

    private void InstantiateNote(NoteData data)
    {
        GameObject noteObject = Instantiate(noteEntryPrefab, notesContentArea);
        TextMeshProUGUI playerNameText = noteObject.transform.Find("PlayerNameText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI messageText = noteObject.transform.Find("MessageText")?.GetComponent<TextMeshProUGUI>();

        if (playerNameText != null && messageText != null)
        {
            playerNameText.text = data.playerName;
            messageText.text = data.playerMessage;
        }
    }

    private IEnumerator FadeAndLoadSceneCoroutine()
    {
        if (blackPanel != null)
        {
            blackPanel.gameObject.SetActive(true);
            float elapsed = 0;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                Color color = blackPanel.color;
                color.a = Mathf.Clamp01(elapsed / fadeDuration);
                blackPanel.color = color;
                yield return null;
            }
        }
        SceneManager.LoadScene(prologueSceneName);
    }
}