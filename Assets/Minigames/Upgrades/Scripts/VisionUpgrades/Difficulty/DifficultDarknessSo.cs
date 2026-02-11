
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultDarknessSo", menuName = "Upgrades/Difficulty/DifficultDarknessSo")]
public class DifficultDarknessSo : ValueModifierAbstract
{
    public override string ModifierDescription()
    {
        return "";
    }

    public override void OnBuy()
    {
        TurkMaterialUpdaterScript.VisionStrengthModifier += ValueModifier;
    }

    public override void ValueModifier(ref float referenceValue)
    {
        referenceValue *= TurkPuzzleScript.instance.LevelSets[TurkPuzzleScript.CurrentDifficutly].DarknessModifier;
    }
}
