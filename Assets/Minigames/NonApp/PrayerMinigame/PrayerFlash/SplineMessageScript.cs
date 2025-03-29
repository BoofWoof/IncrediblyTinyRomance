using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

public class SplineMessageScript : MonoBehaviour
{
    public SplineContainer splineContainer;  // Reference to the SplineContainer
    public GameObject messageObject;                // Prefab to instantiate
    public float period = 2f;               // Movement speed
    public float delay = 0f;

    public AnimationCurve speedCurve;

    public void ActivateMessage()
    {
        if (splineContainer != null && messageObject != null)
        {
            StartCoroutine(MoveAlongSpline());
        }
    }

    IEnumerator MoveAlongSpline()
    {
        yield return new WaitForSeconds(delay);

        GameObject spawnedObject = Instantiate(messageObject, Vector3.zero, Quaternion.identity);
        MoveObject(0f, spawnedObject);  // Initialize position
        float timePassedSec = 0f;

        while (timePassedSec < period)
        {
            timePassedSec += Time.deltaTime;
            float progress = speedCurve.Evaluate(timePassedSec/period);
            MoveObject(progress, spawnedObject);
            yield return null; // Wait for next frame
        }
        Light light = spawnedObject.GetComponent<Light>();
        if (light != null) { light.enabled = false; }
        spawnedObject.GetComponent<ParticleSystem>().Stop();
        yield return new WaitUntil(() => !spawnedObject.GetComponent<ParticleSystem>().IsAlive());
        Destroy(spawnedObject);
    }

    void MoveObject(float t, GameObject spawnedObject)
    {
        if (splineContainer != null && splineContainer.Spline != null)
        {
            // Get the position on the spline at 't' (0 to 1)
            Vector3 position = splineContainer.EvaluatePosition(t);
            Quaternion rotation = Quaternion.LookRotation(splineContainer.EvaluateTangent(t));

            spawnedObject.transform.position = position;
            spawnedObject.transform.rotation = rotation;
        }
    }
}
