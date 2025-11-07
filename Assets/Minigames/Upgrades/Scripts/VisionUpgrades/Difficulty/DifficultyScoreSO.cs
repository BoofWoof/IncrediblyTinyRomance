
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyScore", menuName = "Upgrades/Difficulty/DifficultyScore")]
public class DifficultyScoreSO : ValueModifierAbstract
{
    public float BasePower = 2f;
    public override string ModifierDescription()
    {
        return UpgradeName + ": x" + CalculateMultiplier().ToString("n0") + " <color=#808080>(" + BasePower.ToString("n0") + "^DifficultyLevel)</color>";
    }

    public override void OnBuy()
    {
        TurkPuzzleScript.RewardMultiplier += ValueModifier;
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
