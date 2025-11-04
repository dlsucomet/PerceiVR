using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DecisionPointLogger : MonoBehaviour
{
    [Header("Decision Point Settings")]
    public string decisionPointName; // auto-filled if empty
    public List<Button> optionButtons = new List<Button>();

    private float startTime;

    private void OnEnable()
    {
        // Automatically set decision point name based on scene
        if (string.IsNullOrEmpty(decisionPointName))
        {
            string sceneName = SceneManager.GetActiveScene().name;
            decisionPointName = $"{sceneName}_DecisionPoint";
        }

        startTime = Time.time;

        // Attach listeners to each button
        foreach (var btn in optionButtons)
        {
            string optionName = btn.name; // Use the button GameObject’s name
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnOptionSelected(optionName));
        }

        Debug.Log($"[DecisionPointLogger] Activated: {decisionPointName}");
    }

    private void OnOptionSelected(string optionName)
    {
        float latency = Time.time - startTime;

        // Log data to DataLogger singleton
        if (DataLogger.Instance != null)
        {
            DataLogger.Instance.LogDecision(decisionPointName, optionName, latency);
        }

        Debug.Log($"[Decision Logged] {decisionPointName}: '{optionName}' selected after {latency:F2}s");
    }
}
