
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VisionDifficultyUpgrade", menuName = "Upgrades/Difficulty/VisionDifficultyUpgrade")]
public class UnlockNewDifficultySO : UpgradesAbstract
{
    public override void OnBuy()
    {
        TurkPuzzleScript.instance.UnlockNewDifficulty();
        ActiveBroadcast.BroadcastActivation("DifficultyGlow");
    }
}
