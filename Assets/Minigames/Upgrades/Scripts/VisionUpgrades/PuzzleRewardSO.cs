using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleRewardUpgrade", menuName = "Upgrades/PuzzleReward")]
public class PuzzleRewardSO : ValueModifierAbstract
{
    [Header("Reward Changes")]
    public float MultiplyReward;

    public override string ModifierDescription()
    {
        return UpgradeName + ": x" + MultiplyReward.NumberToString(true);
    }

    public override void ValueModifier(ref float referenceValue)
    {
        referenceValue *= MultiplyReward;
    }

    public override void OnBuy()
    {
        TurkPuzzleScript.RewardMultiplier += ValueModifier;
    }
}
