using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataLogger : MonoBehaviour
{
    public static DataLogger Instance;
    private List<DecisionLog> logs = new List<DecisionLog>();

    private string filePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateNewSessionFile();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CreateNewSessionFile()
    {
        string folderPath = Path.Combine(Application.dataPath, "Data Logs");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fileName = "Session_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
        filePath = Path.Combine(folderPath, fileName);

        File.WriteAllText(filePath, "Timestamp,Scene,DecisionPoint,SelectedOption,ResponseTime\n");

        Debug.Log($"Data log created at: {filePath}");
    }
    public void LogDecision(string decisionPointName, string optionName, float latency)
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string logLine = $"{System.DateTime.Now:HH:mm:ss},{sceneName},{decisionPointName},{optionName},{latency:F2}\n";
        File.AppendAllText(filePath, logLine);

        Debug.Log($"Logged Decision → Scene: {sceneName}, Point: {decisionPointName}, Option: {optionName}, Time: {latency:F2}s");
    }

}
