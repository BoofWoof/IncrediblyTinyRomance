using UnityEngine;

[CreateAssetMenu(fileName = "VisionDifficultyUpgrade", menuName = "Upgrades/VisionDifficultyUpgrade")]
public class UnlockNewDifficultySO : UpgradesAbstract
{
    public override void OnBuy()
    {
        TurkPuzzleScript.instance.UnlockNewDifficulty();
    }
}
