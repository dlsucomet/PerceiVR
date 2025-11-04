using System;
using UnityEngine;

[Serializable]
public class DecisionLog
{
    public string sceneName;
    public string decisionPoint;
    public string selectedOption;
    public float responseTime; // in seconds
    public DateTime timestamp;

    public DecisionLog(string sceneName, string decisionPoint, string selectedOption, float responseTime)
    {
        this.sceneName = sceneName;
        this.decisionPoint = decisionPoint;
        this.selectedOption = selectedOption;
        this.responseTime = responseTime;
        this.timestamp = DateTime.Now;
    }
}
