using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageProductionReduction", menuName = "Upgrades/Challenges/DamageProductionReduction")]
public class DamageProductionReduction : ValueModifierAbstract
{
    public Color DisplayColor = Color.red;

    public override string ModifierDescription()
    {
        return "";
    }

    public override void OnBuy()
    {
        TurkPuzzleScript.secondaryMuliplierListModifier += ListModifier;
    }

    public void ListModifier(ref List<SecondaryMultiplier> referenceValue)
    {
        float multiplier = DefenseStats.GetEfficiencyMultiplier();

        if(multiplier < 1f)
        {
            //string hex = DisplayColor.ToHexString().Substring(0, 6);
            referenceValue.Add(
                new SecondaryMultiplier
                {
                    multiplier = multiplier,
                    description = "<color=#" + DisplayColor.ToHexString() + "><b>DAMAGE: x</b>" + multiplier.AllSignificantDigits(3) + "</color>"
                }
                );
        }
    }
    public override void ValueModifier(ref float referenceValue)
    {
    }
}
