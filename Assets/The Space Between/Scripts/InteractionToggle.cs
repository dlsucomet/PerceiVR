using UnityEngine;

public class InteractionToggle : MonoBehaviour
{
    public GameObject leftInteractions;
    public GameObject rightInteractions;

    public void EnableInteractions()
    {
        if (leftInteractions) leftInteractions.SetActive(true);
        if (rightInteractions) rightInteractions.SetActive(true);
    }

    public void DisableInteractions()
    {
        if (leftInteractions) leftInteractions.SetActive(false);
        if (rightInteractions) rightInteractions.SetActive(false);
    }
}
