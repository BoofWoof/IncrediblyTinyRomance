using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BlinkScript : MonoBehaviour
{
    public SkinnedMeshRenderer TargetMesh;
    public float MinimumBlinkTime = 2f;
    public float MaximumBlinkTime = 5f;

    public float BlinkLength = 0.1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        int leftEyeIndex = TargetMesh.sharedMesh.GetBlendShapeIndex("Blink.L");
        int rightEyeIndex = TargetMesh.sharedMesh.GetBlendShapeIndex("Blink.R");
        while (true)
        {
            float timePassedSec = 0;
            while (timePassedSec < BlinkLength)
            {
                timePassedSec += Time.deltaTime;
                float progress = timePassedSec / BlinkLength;

                TargetMesh.SetBlendShapeWeight(leftEyeIndex, Mathf.Lerp(0, 100, progress));
                TargetMesh.SetBlendShapeWeight(rightEyeIndex, Mathf.Lerp(0, 100, progress));
                yield return null;
            }
            TargetMesh.SetBlendShapeWeight(leftEyeIndex, 100f);
            TargetMesh.SetBlendShapeWeight(rightEyeIndex, 100f);
            timePassedSec = 0;
            while (timePassedSec < BlinkLength)
            {
                timePassedSec += Time.deltaTime;
                float progress = timePassedSec / BlinkLength;

                TargetMesh.SetBlendShapeWeight(leftEyeIndex, Mathf.Lerp(100, 0, progress));
                TargetMesh.SetBlendShapeWeight(rightEyeIndex, Mathf.Lerp(100, 0, progress));
                yield return null;
            }
            TargetMesh.SetBlendShapeWeight(leftEyeIndex, 0f);
            TargetMesh.SetBlendShapeWeight(rightEyeIndex, 0f);
            yield return new WaitForSeconds(Random.Range(MinimumBlinkTime, MaximumBlinkTime));
        }
    }
}
