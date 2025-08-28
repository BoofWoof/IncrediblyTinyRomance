using UnityEngine;

public class AerialDefenseTriggers : MonoBehaviour
{
    public ThreatWaveInfo threatWaveInfo;

    public void Activate()
    {
        AerialDefenseScript.Unlock();
        AerialDefenseScript.Instance.threatSpawnerScript.threatWaveInfo = threatWaveInfo;
        AerialDefenseScript.Instance.StartWave();
    }
}
