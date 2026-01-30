using TMPro;
using UnityEngine;

public class DefenseStats : MonoBehaviour
{
    public static DefenseStats instance;

    public static float CityEfficiencyHealth = 100;

    public float HealRate = 0.1f;
    public static float MinimumHealth = 50f;
    public float MaxHealth = 100;

    public GameObject HealthBar;
    public TMP_Text HealthBarText;

    public AudioSource DamageAudio;

    public void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        if(!ChannelChanger.DangerActive) CityEfficiencyHealth += Time.deltaTime * HealRate;
        if (CityEfficiencyHealth > MaxHealth ) CityEfficiencyHealth = MaxHealth;

        if(CityEfficiencyHealth < MaxHealth)
        {
            HealthBar.SetActive(true);
            HealthBarText.text = CityEfficiencyHealth.ToString("F0") + "%";
        } else
        {
            HealthBar.SetActive(false);
        }
    }
    public static void DamageCity(float Damage)
    {
        instance.DamageAudio.Play();
        CityEfficiencyHealth -= Damage;
        if(CityEfficiencyHealth < MinimumHealth ) CityEfficiencyHealth = MinimumHealth;
    }
    public static float GetEfficiencyMultiplier()
    {
        return CityEfficiencyHealth / 100f;
    }
}
