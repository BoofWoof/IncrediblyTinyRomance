using System.Collections;
using UnityEngine;

[System.Serializable]
public struct ThreatWaveInfo
{
    public ThreatInfo[] threatInfos;
    public float pauseMean;
    public float pauseStd;
    public int neededDestruction;
}
[System.Serializable]
public struct ThreatInfo
{
    public GameObject ThreatPrefab;
    public float Weight;
}

public class ThreatSpawnerScript : MonoBehaviour
{
    public RectTransform SpawnRect; // The RectTransform to use
    public RectTransform TargetRect; // The RectTransform to use

    public float SpawnLength = 1f;    // Length of the horizontal line
    public Color SpawnColor = Color.blue;
    public Color TargetColor = Color.red;

    public RectTransform CanvasParent;

    public ThreatWaveInfo threatWaveInfo;

    public void Start()
    {
        SpawnRect = GetComponent<RectTransform>();
    }

    public void StartWave()
    {
        StartCoroutine(RunWave(threatWaveInfo));
    }

    public IEnumerator RunWave(ThreatWaveInfo waveInfo)
    {
        AerialDefenseScript.SetTargetsToKill(waveInfo.neededDestruction);
        while (true)
        {
            SpawnThreat(waveInfo.threatInfos[0].ThreatPrefab);
            float waitTime = waveInfo.pauseMean + Random.value * waveInfo.pauseStd;
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void SpawnThreat(GameObject ThreatPrefab)
    {
        GameObject newThreat = Instantiate(ThreatPrefab, CanvasParent);
        RectTransform threatRect = newThreat.GetComponent<RectTransform>();
        threatRect.localScale = Vector3.one;

        float randomSpawnPos = (Random.value - 0.5f) * SpawnLength / threatRect.lossyScale.x;
        threatRect.anchoredPosition = CanvasParent.right * randomSpawnPos;

        threatRect.localRotation = Quaternion.identity;
        threatRect.SetSiblingIndex(0);
    }

    private void OnDrawGizmos()
    {
        SpawnRect = GetComponent<RectTransform>();

        Gizmos.color = SpawnColor;

        // Get world position and rotation of the RectTransform
        Vector3 center = SpawnRect.position;
        Quaternion rotation = SpawnRect.rotation;

        // Calculate half-length vector along the local X-axis
        Vector3 half = rotation * Vector3.right * (SpawnLength / 2f);

        // Draw the line
        Gizmos.DrawLine(center - half, center + half);

        if (TargetRect == null)
            return;

        Gizmos.color = TargetColor;

        // Get world position and rotation of the RectTransform
        center = TargetRect.position;
        rotation = TargetRect.rotation;

        // Calculate half-length vector along the local X-axis
        half = rotation * Vector3.right * (SpawnLength / 2f);

        // Draw the line
        Gizmos.DrawLine(center - half, center + half);
    }
}
