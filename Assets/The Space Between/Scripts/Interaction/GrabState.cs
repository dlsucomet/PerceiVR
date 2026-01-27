using UnityEngine;
using Oculus.Interaction;

public class GrabState : MonoBehaviour
{
    public Grabbable grabbable;

    public bool IsGrabbed => grabbable != null && grabbable.SelectingPointsCount > 0;

    void Awake()
    {
        if (grabbable == null)
            grabbable = GetComponent<Grabbable>();
    }
}