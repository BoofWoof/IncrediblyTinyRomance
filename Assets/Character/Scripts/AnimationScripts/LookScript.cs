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
    [Range(0f, 1f)]
    public float HeadLookWeight = 1f;

    public Transform EyeL;
    public Transform EyeR;
    public Transform Head;

    public bool Flip = false;

    [Header("Target")]
    public Transform Target;

    private RotationLimits LEyeRotLimits = new RotationLimits(-70f, 10f, -10f, 5f);
    private RotationLimits REyeRotLimits = new RotationLimits(-10f, 70f, -10f, 5f);
    private RotationLimits HeadRotLimits = new RotationLimits(-30f, 50f, -15f, 35f);


    [Header("Speed")]
    public float lookSpeed = 15f;
    private Quaternion initialHeadLocalRotation;
    private Quaternion initialLEyeLocalRotation;
    private Quaternion initialREyeLocalRotation;


    private void Start()
    {
        initialHeadLocalRotation = Head.localRotation;
        initialLEyeLocalRotation = EyeL.localRotation;
        initialREyeLocalRotation = EyeR.localRotation;
    }
    void LateUpdate()
    {
        if (Target == null) return;
        LookAt(Head, -5f, HeadRotLimits, initialHeadLocalRotation);
        LookAt(EyeL, 0f, LEyeRotLimits, initialLEyeLocalRotation);
        LookAt(EyeR, 0f, REyeRotLimits, initialREyeLocalRotation);
    }

    public void SetLookWeight(float newWeight)
    {
        HeadLookWeight = newWeight;
    }

    void LookAt(Transform sourceTransform, float yOffset, RotationLimits rotLimits, Quaternion initialRotation)
    {
        // Direction from bone to target in world space
        Vector3 directionToTarget = sourceTransform.position - Target.position;
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

        // Create rotation from the clamped angles
        Quaternion targetLocalRotation = Quaternion.Euler(pitch, yaw, 0f);

        // Apply rotation relative to original pose
        Quaternion finalRotation = initialRotation * targetLocalRotation;

        sourceTransform.localRotation = Quaternion.Slerp(sourceTransform.localRotation, finalRotation, Time.deltaTime * lookSpeed);
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
