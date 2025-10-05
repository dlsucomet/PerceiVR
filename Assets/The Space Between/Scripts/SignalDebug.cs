using UnityEngine;

public class SignalDebug : MonoBehaviour
{
    public void OnSignalReceived()
    {
        Debug.Log("imeline signal received at runtime!");
    }
}
