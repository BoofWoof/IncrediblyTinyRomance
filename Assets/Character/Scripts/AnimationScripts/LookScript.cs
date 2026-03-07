using NUnit.Framework;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

struct RotationLimits
{
    public float NegHor;
    public float PosHor;
    public float NegVer;
    public float PosVer;

    public RotationLimits(float NegHor, float PosHor, float NegVer, float PosVer)
    {
        this.NegHor = NegHor;
        this.PosHor = PosHor;
        this.NegVer = NegVer;
        this.PosVer = PosVer;
    }
}


public class LookScript : MonoBehaviour
{
    public float HeadLookWeight = 1f;

    public Transform EyeL;
    public Transform TargetEyeL;
    public Transform EyeR;
    public Transform TargetEyeR;
    public Transform Head;
    public Transform TargetHead;

    public bool Flip = false;

    [Header("Target")]
    public Transform Target;
    public Transform DistractionTarget = null;

    private RotationLimits LEyeRotLimits = new RotationLimits(-70f, 10f, -10f, 5f);
    private RotationLimits REyeRotLimits = new RotationLimits(-10f, 70f, -10f, 5f);
    private RotationLimits HeadRotLimits = new RotationLimits(-30f, 50f, -15f, 35f);


    [Header("Speed")]
    public float lookSpeed = 15f;
    private Quaternion initialHeadLocalRotation;
    private Quaternion initialLEyeLocalRotation;
    private Quaternion initialREyeLocalRotation;

    public float NearLookAwayDistance = 0.5f;
    public float LookAwayValue = 45f;

    public List<Transform> DistractionPoints;
    private bool Distracted = false;
    public bool ForceDistracted = false;
    public static Transform ExternalDistractionPoint;
    public static Transform PrevExternalDistractionPoint;


    private void Start()
    {
        TargetEyeL.transform.parent = EyeL;
        TargetEyeR.transform.parent = EyeR;
        TargetHead.transform.parent = Head;

        initialHeadLocalRotation = Head.localRotation;
        initialLEyeLocalRotation = EyeL.localRotation;
        initialREyeLocalRotation = EyeR.localRotation;

        StartCoroutine(OccassionalDistraction());
    }

    public IEnumerator OccassionalDistraction()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(30, 60));
            int distractionIndex = Random.Range(0, DistractionPoints.Count);
            Distracted = true;
            DistractionTarget = DistractionPoints[distractionIndex];
            if (ExternalDistractionPoint != null) DistractionTarget = ExternalDistractionPoint;
            yield return new WaitForSeconds(Random.Range(3, 5));
            if (!ForceDistracted) DistractionTarget = null;
            yield return new WaitForSeconds(2f);
            if (!ForceDistracted) Distracted = false;
        }
    }
    public IEnumerator ForceDistraction()
    {
        ForceDistracted = true;
        yield return new WaitForSeconds(0.3f);
        Distracted = true;
        DistractionTarget = ExternalDistractionPoint;
        yield return new WaitForSeconds(Random.Range(8, 12));
        DistractionTarget = null;
        yield return new WaitForSeconds(2f);
        Distracted = false;
        ForceDistracted = false;
    }

    void LateUpdate()
    {
        if (Target == null) return;
        if (PrevExternalDistractionPoint != ExternalDistractionPoint)
        {
            PrevExternalDistractionPoint = ExternalDistractionPoint;
            if(ExternalDistractionPoint != null) StartCoroutine(ForceDistraction());
        }

        LookAt(Head, -5f, HeadRotLimits, initialHeadLocalRotation, true);
        LookAt(EyeL, 0f, LEyeRotLimits, initialLEyeLocalRotation);
        LookAt(EyeR, 0f, REyeRotLimits, initialREyeLocalRotation);
    }

    public void SetLookWeight(float newWeight)
    {
        HeadLookWeight = newWeight;
    }

    void LookAt(Transform sourceTransform, float yOffset, RotationLimits rotLimits, Quaternion initialRotation, bool LookAwayCheck = false)
    {
        Transform selectedTarget = Target;
        float selectedLookSpeed = lookSpeed;
        if (Distracted && PrayerScript.instance.JudgementActive && !PrayerScript.instance.JudgementFocus) {
            if(DistractionTarget != null) selectedTarget = DistractionTarget;
            selectedLookSpeed = 2f;
        } 

        // Direction from bone to target in world space
        Vector3 directionToTarget = sourceTransform.position - selectedTarget.position;
        if (Flip)
        {
            directionToTarget = new Vector3(-directionToTarget.x, -directionToTarget.y, -directionToTarget.z);
        }

        // Convert target direction into the bone's local space
        Vector3 localDirection = sourceTransform.parent.InverseTransformDirection(directionToTarget.normalized);

        // Convert direction to angles
        float yaw = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
        float pitch = -Mathf.Asin(localDirection.y) * Mathf.Rad2Deg;

        // Clamp angles
        yaw = HeadLookWeight * Mathf.Clamp(yaw, rotLimits.NegHor, rotLimits.PosHor);
        pitch = HeadLookWeight * Mathf.Clamp(pitch, rotLimits.NegVer, rotLimits.PosVer);

        float lookAwayYaw = 0f;
        if (LookAwayCheck)
        {
            float distance = Vector3.Distance(sourceTransform.position, selectedTarget.position);
            if (distance < NearLookAwayDistance)
            {
                float progress = 1 - (distance / NearLookAwayDistance);
                lookAwayYaw = progress * LookAwayValue;
            }
        }

        // Create rotation from the clamped angles
        Quaternion targetLocalRotation = Quaternion.Euler(pitch, yaw + lookAwayYaw, 0f);

        // Apply rotation relative to original pose
        Quaternion finalRotation = initialRotation * targetLocalRotation;


        sourceTransform.localRotation = Quaternion.Slerp(sourceTransform.localRotation, finalRotation, Time.deltaTime * selectedLookSpeed);
    }

    /*
    Vector3 NormalizeAngles(Vector3 angles)
    {
        angles.x = NormalizeAngle(angles.x);
        angles.y = NormalizeAngle(angles.y);
        angles.z = NormalizeAngle(angles.z);
        return angles;
    }

    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
    /*

    /*
    void LookAt(Transform lookObject, float xOffset, RotationLimits rotLimits)
    {
        transform.LookAt(lookObject);

        Vector3 directionToTarget = Target.position - lookObject.position;

        // Create a rotation so that local up points toward the target
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

        lookObject.rotation = lookRotation;

        Vector3 localEulerAngles = lookObject.localRotation.eulerAngles;

        Vector3 limitedEulerAngles = LimitRotations(localEulerAngles, rotLimits);

        lookObject.localRotation = Quaternion.Euler(xOffset - limitedEulerAngles.x, limitedEulerAngles.y, 0f);
    }

    private Vector3 LimitRotations(Vector3 rawRotation, RotationLimits rotLimits)
    {
        rawRotation.x = Mathf.Clamp(rawRotation.x, rotLimits.NegVer, rotLimits.PosVer);
        rawRotation.y = Mathf.Clamp(rawRotation.y - 180f, rotLimits.NegHor, rotLimits.PosHor);

        return rawRotation;
    }
    */
}
