using System.Collections;
using UnityEngine;

public class LookGateCue : MonoBehaviour
{
    [Header("XR Camera")]
    [Tooltip("Assign CenterEyeAnchor / Main Camera")]
    public Transform xrCamera;

    [Header("Look Target")]
    [Tooltip("The object the player must look at")]
    public Transform lookTarget;

    [Header("Particles Cue")]
    public ParticleSystem cueParticles;
    public bool cueFollowsTarget = true;

    [Header("Look Detection")]
    [Range(2f, 15f)]
    [Tooltip("Allowed angle before it counts as 'looking'")]
    public float allowedAngleDeg = 7f;

    [Tooltip("How long they must look before resuming")]
    public float dwellSeconds = 0.35f;

    [Tooltip("Prevents gaze through walls")]
    public bool requireLineOfSight = true;

    public LayerMask occluderMask = ~0;

    [Header("Resume Timing")]
    [Tooltip("Small cinematic delay before timeline resumes")]
    public float resumeDelay = 0.2f;

    private float lookTimer;
    private bool gateActive;

    void Awake()
    {
        if (xrCamera == null && Camera.main != null)
            xrCamera = Camera.main.transform;

        if (cueParticles != null)
            cueParticles.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gateActive || xrCamera == null || lookTarget == null)
            return;

        if (cueParticles != null && cueFollowsTarget)
            cueParticles.transform.position = lookTarget.position;

        if (IsLookingAtTarget())
            lookTimer += Time.unscaledDeltaTime;
        else
            lookTimer = 0f;

        if (lookTimer >= dwellSeconds)
        {
            CompleteGate();
        }
    }

    public void BeginGate()
    {
        gateActive = true;
        lookTimer = 0f;

        if (cueParticles != null)
        {
            cueParticles.gameObject.SetActive(true);
            cueParticles.transform.position = lookTarget.position;
            cueParticles.Play(true);
        }
    }

    public void CancelGate()
    {
        gateActive = false;
        lookTimer = 0f;

        if (cueParticles != null)
        {
            cueParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            cueParticles.gameObject.SetActive(false);
        }
    }

    private void CompleteGate()
    {
        CancelGate();
        StartCoroutine(ResumeAfterDelay());
    }

    private IEnumerator ResumeAfterDelay()
    {
        yield return new WaitForSecondsRealtime(resumeDelay);

        if (!NarrativeManager.Instance)
        {
            Debug.LogError("NarrativeManager.Instance is NULL!");
            yield break;
        }

        NarrativeManager.Instance.ResumeNarrative();
    }

    private bool IsLookingAtTarget()
    {
        Vector3 toTarget = (lookTarget.position - xrCamera.position).normalized;

        float angle = Vector3.Angle(xrCamera.forward, toTarget);

        if (angle > allowedAngleDeg)
            return false;

        if (!requireLineOfSight)
            return true;

        float dist = Vector3.Distance(xrCamera.position, lookTarget.position);

        if (Physics.Raycast(
            xrCamera.position,
            toTarget,
            out RaycastHit hit,
            dist,
            occluderMask,
            QueryTriggerInteraction.Ignore))
        {
            if (hit.transform != lookTarget &&
                !hit.transform.IsChildOf(lookTarget))
                return false;
        }

        return true;
    }
}