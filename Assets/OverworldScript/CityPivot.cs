using System.Collections;
using UnityEngine;

public class CityPivot : MonoBehaviour
{
    public AnimationCurve RotationCurve;

    public bool Rotating = false;
    public float RotationPeriod = 1f;
    public float DegPerRotation = 45f;
    public int Rotations = 0;

    public delegate void OnSpinDelegate(int Spins);
    static public OnSpinDelegate OnSpin;


    public void RotateClockwise()
    {
        if (Rotating) return;
        StartCoroutine(ChangeRotation(RotationsToAngle(Rotations), RotationsToAngle(Rotations + 1)));
        Rotations++;

        OnSpin?.Invoke(Rotations);
    }

    public void RotateCounter()
    {
        if (Rotating) return;
        StartCoroutine(ChangeRotation(RotationsToAngle(Rotations), RotationsToAngle(Rotations - 1)));
        Rotations--;

        OnSpin?.Invoke(Rotations);
    }

    public IEnumerator ChangeRotation(float currentRotation, float newRotation)
    {

        Rotating = true;
        float timePassed = 0f;
        float rotationDelta = newRotation - currentRotation;

        while (timePassed < RotationPeriod)
        {
            timePassed += Time.deltaTime;
            float adjustedRotation = currentRotation + RotationCurve.Evaluate(timePassed/RotationPeriod) * rotationDelta;
            transform.rotation = Quaternion.Euler(0f, adjustedRotation, 0f);
            Debug.Log(adjustedRotation);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0f, newRotation, 0f);

        Rotating = false;
    }

    private float RotationsToAngle(int rotations)
    {
        return DegPerRotation * rotations;
    }
}
