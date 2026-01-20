using UnityEngine;

public class FixHandScale : MonoBehaviour
{
    public Transform leftHandRoot;
    public Transform rightHandRoot;

    void LateUpdate()
    {
        if (leftHandRoot)
            leftHandRoot.localScale = Vector3.one;

        if (rightHandRoot)
            rightHandRoot.localScale = Vector3.one;
    }
}
