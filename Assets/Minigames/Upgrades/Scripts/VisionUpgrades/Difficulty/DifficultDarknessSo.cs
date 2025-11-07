
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultDarknessSo", menuName = "Upgrades/Difficulty/DifficultDarknessSo")]
public class DifficultDarknessSo : ValueModifierAbstract
{
    public List<float> LightDifficultyScaler = new List<float>()
    {
        2f,
        1f,
        0.8f,
        0.6f,
        0.5f,
        0.4f,
        0.3f
    };

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
        referenceValue *= LightDifficultyScaler[TurkPuzzleScript.CurrentDifficutly];
    }
}
