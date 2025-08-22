using UnityEngine;

public class ScanPointScript : MonoBehaviour
{
    [Header("Prefab to spawn on Scanner collision")]
    public GameObject Ping;
    public GameObject Threat;
    public GameObject Canvas;

    [Header("Spawn settings")]
    public Vector3 spawnOffset = Vector3.zero;

    // Called when this trigger collider overlaps another collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Scanner")
        {
            SpawnPrefab();
        }
    }

    private void SpawnPrefab()
    {
        if (Ping != null)
        {
            //Instantiate(Ping, transform.position + spawnOffset, Quaternion.identity, Canvas.transform);
            Instantiate(Threat, transform.position + spawnOffset, Quaternion.identity, Canvas.transform);
        }
        else
        {
            Debug.LogWarning("No prefab assigned to SpawnOnScannerCollision on " + gameObject.name);
        }
    }
}
