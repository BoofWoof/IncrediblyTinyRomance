using UnityEngine;

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
    public Transform EyeL;
    public Transform EyeR;
    public Transform Head;

    public Transform Target;

    private RotationLimits LEyeRotLimits = new RotationLimits(-70f, 10f, -10f, 5f);
    private RotationLimits REyeRotLimits = new RotationLimits(-10f, 70f, -10f, 5f);
    private RotationLimits HeadRotLimits = new RotationLimits(-30f, 30f, -15f, 35f);

    void Update()
    {
        if (Target == null) return;
        LookAt(Head, -5f, HeadRotLimits);
        LookAt(EyeL, -80f, LEyeRotLimits);
        LookAt(EyeR, -80f, REyeRotLimits);
    }

    void LookAt(Transform lookObject, float xOffset, RotationLimits rotLimits)
    {
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
}
