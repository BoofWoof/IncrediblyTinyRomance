using System.Collections;
using UnityEngine;

public class FallUpwardSpawnerScript : MonoBehaviour
{
    public GameObject SpawnObject;

    public float minUpwardLaunchForce = 0.2f;
    public float maxUpwardLaunchForce = 0.5f;
    public float minTorqueMagnitude = 5f;
    public float maxTorqueMagnitude = 15f;
    public float objectScale = 1f;

    public float newGravity = 0.1f;

    public void Start()
    {
        Physics.gravity = Vector3.up * newGravity;
        StartCoroutine(SpawnCoroutine());
    }

    public IEnumerator SpawnCoroutine()
    {
        while (true)
        {
        Vector3 spawnPosition = transform.position + Vector3.right * Random.Range(-5f, 5f);

        GameObject newObject = Instantiate(SpawnObject, spawnPosition, Quaternion.identity, transform);
        newObject.transform.localScale = Vector3.one * objectScale;
        Rigidbody rb = newObject.GetComponent<Rigidbody>();

        float randomUpwardLaunchForce = Random.Range(minUpwardLaunchForce, maxUpwardLaunchForce);
        rb.AddForce(Vector3.up * randomUpwardLaunchForce, ForceMode.Impulse);
        Vector3 randomTorqueDirection = Random.insideUnitSphere.normalized;
        float randomTorqueMagnitude = Random.Range(minTorqueMagnitude, maxTorqueMagnitude);

        rb.AddTorque(randomTorqueDirection * randomTorqueMagnitude, ForceMode.Impulse);

        StartCoroutine(DeleteObjectInAFewSeconds(30f, newObject));

        yield return new WaitForSeconds(2f);
        }
    }

    public IEnumerator DeleteObjectInAFewSeconds(float seconds, GameObject deletionTarget)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(deletionTarget);
    }
}
