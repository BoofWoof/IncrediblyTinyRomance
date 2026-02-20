using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DefenseStats : MonoBehaviour
{
    public UnityEvent<float> OnHealthChange;

    public static DefenseStats instance;

    public static float CityEfficiencyHealth = 100;

    public float HealRate = 0.1f;
    public static float MinimumHealth = 50f;
    public float MaxHealth = 100;

    public AudioSource DamageAudio;

    private bool FirstDamage = true;
    public void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        if(!GameStateMonitor.DangerActive) CityEfficiencyHealth += Time.deltaTime * HealRate;
        if (CityEfficiencyHealth > MaxHealth ) CityEfficiencyHealth = MaxHealth;
        
        OnHealthChange?.Invoke(GetEfficiencyMultiplier());
    }
    public static void DamageCity(float Damage)
    {
        if (Damage <= 0 && CityEfficiencyHealth == instance.MaxHealth) return;
        if (instance.FirstDamage)
        {
            instance.FirstDamage = false;
            ActiveBroadcast.BroadcastActivation("DamageArchive");
        }

        if(Damage > 0) instance.DamageAudio.Play();
        CityEfficiencyHealth -= Damage;
        if(CityEfficiencyHealth < MinimumHealth ) CityEfficiencyHealth = MinimumHealth;
    }
    public static float GetEfficiencyMultiplier()
    {
        return CityEfficiencyHealth / instance.MaxHealth;
    }
}
