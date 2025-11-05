using UnityEngine;

[CreateAssetMenu(fileName = "VisionDifficultyUpgrade", menuName = "Upgrades/VisionDifficultyUpgrade")]
public class UnlockNewDifficultySO : ValueModifierAbstract
{
    public bool AddMultiplier = false;
    public float BasePower = 2f;

    public override string ModifierDescription()
    {
        return UpgradeName + ": x" + CalculateMultiplier().ToString("n0") + " <color=#808080>(" + BasePower.ToString("n0") + "^DifficultyLevel)</color>";
    }

    public override void OnBuy()
    {
        TurkPuzzleScript.instance.UnlockNewDifficulty();

        if (AddMultiplier)
        {
            TurkPuzzleScript.RewardMultiplier += ValueModifier;
        }
    }

    public override void ValueModifier(ref float referenceValue)
    {
        referenceValue *= CalculateMultiplier();
    }

    public float CalculateMultiplier()
    {
        return Mathf.Pow(BasePower, TurkPuzzleScript.CurrentDifficutly);
    }
}
