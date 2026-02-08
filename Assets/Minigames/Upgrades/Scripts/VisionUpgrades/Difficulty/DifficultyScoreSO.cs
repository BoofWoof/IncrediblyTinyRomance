
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyScore", menuName = "Upgrades/Difficulty/DifficultyScore")]
public class DifficultyScoreSO : ValueModifierAbstract
{
    public float BasePower = 2f;

    public Color DisplayColor = Color.green;

    public override string ModifierDescription()
    {
        return UpgradeName + ": x" + CalculateMultiplier().ToString("n0") + " <color=#808080>(" + BasePower.ToString("n0") + "^DifficultyLevel)</color>";
    }

    public override void OnBuy()
    {
        TurkPuzzleScript.secondaryMuliplierListModifier += ListModifier;
    }

    public override void ValueModifier(ref float referenceValue)
    {
        return;
    }

    public float CalculateMultiplier()
    {
        return Mathf.Pow(BasePower, TurkPuzzleScript.CurrentDifficutly);
    }

    public void ListModifier(ref List<SecondaryMultiplier> referenceValue)
    {
        float multiplier = CalculateMultiplier();

        if (multiplier > 1.01f)
        {
            //string hex = DisplayColor.ToHexString().Substring(0, 6);
            referenceValue.Add(
                new SecondaryMultiplier
                {
                    multiplier = multiplier,
                    description = "<color=#" + DisplayColor.ToHexString() + "><b>Difficulty: x</b>" + multiplier.AllSignificantDigits(3) + "</color>"
                }
                );
        }
    }
}
