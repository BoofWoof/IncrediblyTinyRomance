using PixelCrushers.DialogueSystem.Demo;
using TMPro;
using UnityEngine;

public class AerialDefenseScript : MonoBehaviour
{
    //Put stats here: Firing rate and ect.


    public int TargetsToKill = 5;
    public int MaxHealth = 3;
    public int RemainingHealth;

    public TMP_Text TargetsToKillText;
    public TMP_Text RemainingHealthText;

    public static AerialDefenseScript Instance;

    public void Start()
    {
        Instance = this;
        RemainingHealth = MaxHealth;

        Instance.TargetsToKillText.text = Instance.TargetsToKill.ToString();
        Instance.RemainingHealthText.text = Instance.RemainingHealth.ToString();
    }

    public static void ThreatDestroyed()
    {
        Instance.TargetsToKill--;
        Instance.TargetsToKillText.text = Instance.TargetsToKill.ToString();
    }

    public static void TakeDamage()
    {
        Debug.Log("Aerial Defense: Hit");
        Instance.RemainingHealth--;
        Instance.RemainingHealthText.text = Instance.RemainingHealth.ToString();
    }
}
