using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleRewardUpgrade", menuName = "Upgrades/PuzzleReward")]
public class PuzzleRewardSO : UpgradesAbstract
{
    [Header("Reward Changes")]
    public float IncreaseReward;

    public override void OnBuy()
    {
        TurkData.CreditsPerPuzzle += IncreaseReward;
    }
}
