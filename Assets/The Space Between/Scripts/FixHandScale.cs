using UnityEngine;

public class FixHandScale : MonoBehaviour
{
    //public Transform leftHandRoot;
    //public Transform rightHandRoot;

    //void LateUpdate()
    //{
    //    if (leftHandRoot)
    //        leftHandRoot.localScale = Vector3.one;

    //    if (rightHandRoot)
    //        rightHandRoot.localScale = Vector3.one;
    //}

    public Transform leftHandRoot;
    public Transform rightHandRoot;
    public Transform[] leftHandBones;
    public Transform[] rightHandBones;

    Vector3 leftScale;
    Vector3 rightScale;

    void Start()
    {
        leftScale = leftHandRoot.localScale;
        rightScale = rightHandRoot.localScale;
    }

    void LateUpdate()
    {
        leftHandRoot.localScale = leftScale;
        rightHandRoot.localScale = rightScale;

        foreach (var b in leftHandBones)
            b.localScale = Vector3.one;

        foreach (var b in rightHandBones)
            b.localScale = Vector3.one;
    }
}
