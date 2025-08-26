using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AerialDefenseScript : MonoBehaviour
{
    //Put stats here: Firing rate and ect.
    public int TargetsToKill = 5;
    public int MaxHealth = 3;
    public int RemainingHealth;
    public static int TotalTimesDamaged;

    public TMP_Text TargetsToKillText;
    public TMP_Text RemainingHealthText;

    public static bool Locked = true;
    public GameObject LockScreen;

    public delegate void OnADLockStateChange(bool lockState);
    public static OnADLockStateChange onADLockStateChange;

    public static AerialDefenseScript Instance;
    public ThreatSpawnerScript threatSpawnerScript;

    public static bool GameRunning = false;

    public ParticleSystem[] DamageParticleSystems;

    public static RectTransform StaticGameCanvas;
    public RectTransform GameCanvas;

    public void Start()
    {
        Instance = this;
        RemainingHealth = MaxHealth;

        StaticGameCanvas = GameCanvas;

        Instance.TargetsToKillText.text = Instance.TargetsToKill.ToString();
        Instance.RemainingHealthText.text = Instance.RemainingHealth.ToString();
    }

    public static void SetTargetsToKill(int newTargetCount)
    {
        Instance.TargetsToKill = newTargetCount;
        Instance.TargetsToKillText.text = Instance.TargetsToKill.ToString();
    }

    public static void Unlock()
    {
        Locked = false;
        Instance.LockScreen.SetActive(false);
        Instance.StartWave();
    }

    public void StartWave()
    {
        onADLockStateChange?.Invoke(false);
        threatSpawnerScript.StartWave();
        GameRunning = true;
    }

    public void StopWave()
    {
        GameRunning = false;
        Instance.threatSpawnerScript.StopAllCoroutines();
    }

    public static void ThreatDestroyed()
    {
        if (!GameRunning) return;

        Instance.TargetsToKill--;
        Instance.TargetsToKillText.text = Instance.TargetsToKill.ToString();

        if(Instance.TargetsToKill == 0)
        {
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
            Instance.StopWave();
        }
    }

    public void DamageImpact()
    {
        MoveCamera.moveCamera.ShakeScreen(0.8f, 0.6f);
    }
}
