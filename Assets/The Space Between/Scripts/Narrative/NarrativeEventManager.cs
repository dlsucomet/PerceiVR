using UnityEngine;

public class NarrativeEventManager : MonoBehaviour
{
    public GameObject decisionPointCanvas;

    public void TriggerDecisionPoint()
    {
        Debug.Log("Timeline finished, triggering decision point");
        if (decisionPointCanvas != null)
        {
            decisionPointCanvas.SetActive(true);
        }

    }
}