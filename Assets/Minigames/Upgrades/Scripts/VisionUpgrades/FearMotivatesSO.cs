using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "FearMotivates", menuName = "Upgrades/FearMotivates")]
public class FearMotivatesSO : ValueModifierAbstract
{
    [Header("Reward Changes")]
    public float PointPerPoint;

    public Color DisplayColor = Color.green;
    public override string ModifierDescription()
    {
        return "";
    }

    public override void ValueModifier(ref float referenceValue)
    {
        return;
    }

    public override void OnBuy()
    {
        TurkPuzzleScript.secondaryMuliplierListModifier += ListModifier;
    }

    public float GetAngerMultiplier()
    {
        return 1 + PrayerScript.instance.RamAngyLevel / PointPerPoint;
    }

    public void ListModifier(ref List<SecondaryMultiplier> referenceValue)
    {
        float multiplier = GetAngerMultiplier();

        if (multiplier > 1.01f)
        {
            //string hex = DisplayColor.ToHexString().Substring(0, 6);
            referenceValue.Add(
                new SecondaryMultiplier
                {
                    multiplier = multiplier,
                    description = "<color=#" + DisplayColor.ToHexString() + "><b>RAM ANGER: x</b>" + multiplier.AllSignificantDigits(3) + "</color>"
                }
                );
        }
    }
}
