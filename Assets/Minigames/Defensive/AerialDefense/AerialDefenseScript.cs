using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class AerialDefenseScript : MonoBehaviour
{
    //Put stats here: Firing rate and ect.
    public int TargetsToKill = 5;
    public int MaxHealth = 3;
    public int RemainingHealth;
    public static int TotalTimesDamaged;

    public TMP_Text TargetsToKillText;
    public TMP_Text RemainingHealthText;

    public delegate void OnADLockStateChange(bool lockState);
    public static OnADLockStateChange onADLockStateChange;

    public static AerialDefenseScript Instance;
    public ThreatSpawnerScript threatSpawnerScript;

    public static bool GameRunning = false;

    public ParticleSystem[] DamageParticleSystems;

    public static RectTransform StaticGameCanvas;
    public RectTransform GameCanvas;

    public float TransitionPeriod = 2f;

    public void Awake()
    {
        Instance = this;
        RemainingHealth = MaxHealth;

        StaticGameCanvas = GameCanvas;

        TargetsToKillText.text = TargetsToKill.ToString();
        RemainingHealthText.text = RemainingHealth.ToString();
    }

    public static void SetTargetsToKill(int newTargetCount)
    {
        Instance.TargetsToKill = newTargetCount;
        Instance.TargetsToKillText.text = Instance.TargetsToKill.ToString();
    }

    public static void Unlock()
    {
        Instance.StartWave();
    }

    public void StartWave()
    {
        if (ChannelChanger.DangerActive) return;
        ChannelChanger.DangerActive = true;

        if(threatSpawnerScript.threatWaveInfo.optionalVolume != null)
        {
            StartCoroutine(StartVolume(threatSpawnerScript.threatWaveInfo.optionalVolume));
        }

        CrossfadeScript.TransitionSong(5);

        onADLockStateChange?.Invoke(false);
        threatSpawnerScript.StartWave();
        GameRunning = true;
    }

    public void StopWave()
    {
        GameRunning = false;

        Instance.threatSpawnerScript.StopAllCoroutines();

        RemainingHealth = MaxHealth;
        Instance.RemainingHealthText.text = Instance.RemainingHealth.ToString();

        CrossfadeScript.TransitionSong(1);

        if (threatSpawnerScript.threatWaveInfo.optionalVolume != null)
        {
            StartCoroutine(StopVolume(threatSpawnerScript.threatWaveInfo.optionalVolume));
        }

        ChannelChanger.DangerActive = false;
        ChannelChanger.ActiveChannelChanger.LockSwitch();
    }

    public static void ThreatDestroyed()
    {
        if (!GameRunning) return;

        Instance.TargetsToKill--;
        Instance.TargetsToKillText.text = Instance.TargetsToKill.ToString();

        if(Instance.TargetsToKill == 0)
        {
            TurretScript.autoFire = true;
            Instance.StopWave();
        }
    }

    public static void TakeDamage()
    {
        if (!GameRunning) return;

        Debug.Log("Aerial Defense: Hit");
        Instance.RemainingHealth--;
        Instance.RemainingHealthText.text = Instance.RemainingHealth.ToString();

        TotalTimesDamaged++;
        int particleImpactIdx = TotalTimesDamaged % Instance.DamageParticleSystems.Length;
        Instance.DamageParticleSystems[particleImpactIdx].Play();
        Instance.DamageParticleSystems[particleImpactIdx].GetComponent<AudioSource>().Play();
        Instance.DamageParticleSystems[particleImpactIdx].GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.0f);
        Instance.Invoke(nameof(DamageImpact), 0.7f);

        if (Instance.RemainingHealth <= 0)
        {
            FallingThreatScript.DestroyAllThreats();
            Instance.StopWave();
        }
    }

    public void DamageImpact()
    {
        MoveCamera.moveCamera.ImpactShakeScreen(0.8f, 0.6f);
    }

    public IEnumerator StartVolume(Volume volume)
    {
        Debug.Log("EFFECTS GO HERE");
        volume.weight = 0f;

        float time = 0;

        while(time < TransitionPeriod)
        {
            time += Time.deltaTime;
            float progress = time/TransitionPeriod;

            volume.weight = Mathf.Lerp(0, 1, progress);

            yield return null;
        }
        volume.weight = 1;
    }
    public IEnumerator StopVolume(Volume volume)
    {
        Debug.Log("EFFECTS GO HERE");
        volume.weight = 1f;

        float time = 0;

        while (time < TransitionPeriod)
        {
            time += Time.deltaTime;
            float progress = time / TransitionPeriod;

            volume.weight = Mathf.Lerp(1, 0, progress);

            yield return null;
        }
        volume.weight = 0;
    }
}
