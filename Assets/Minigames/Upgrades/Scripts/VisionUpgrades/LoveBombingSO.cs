using UnityEngine;

[CreateAssetMenu(fileName = "LoveBombing", menuName = "Upgrades/LoveBombing")]
public class LoveBombingSO : UpgradesAbstract
{
    [Header("PuzzleDifficultyReward")]
    public int CompletionDifficulty;

    public override void OnBuy()
    {
        PrayerScript.PrayerSubmitted += OnPrayerSubmission;
    }

    public void OnPrayerSubmission(bool goodPrayer)
    {
        if (goodPrayer)
        {
            float reward = TurkPuzzleScript.instance.CalculateReward(CompletionDifficulty);
            CurrencyData.Credits += reward;
        }
    }
}