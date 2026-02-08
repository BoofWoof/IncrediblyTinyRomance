using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleRewardUpgrade", menuName = "Upgrades/PuzzleReward")]
public class PuzzleRewardSO : ValueModifierAbstract
{
    [Header("Reward Changes")]
    public float MultiplyReward;
    public Color DisplayColor = Color.green;

    public static float TotalMultiplier = 1f;
    public static bool Subscribed = false;

    public override string ModifierDescription()
    {
        return UpgradeName + ": x" + MultiplyReward.NumberToString(true);
    }

    public override void ValueModifier(ref float referenceValue)
    {
        return;
    }

    public override void OnBuy()
    {
        TotalMultiplier *= MultiplyReward;
        if (Subscribed) return;
        TurkPuzzleScript.secondaryMuliplierListModifier += ListModifier;
        Subscribed = true;
    }

    public void ListModifier(ref List<SecondaryMultiplier> referenceValue)
    {
        float multiplier = TotalMultiplier;

        if (multiplier > 1.01f)
        {
            //string hex = DisplayColor.ToHexString().Substring(0, 6);
            referenceValue.Add(
                new SecondaryMultiplier
                {
                    multiplier = multiplier,
                    description = "<color=#" + DisplayColor.ToHexString() + "><b>GRANDEUR: x</b>" + multiplier.AllSignificantDigits(3) + "</color>"
                }
                );
        }
    }
}
