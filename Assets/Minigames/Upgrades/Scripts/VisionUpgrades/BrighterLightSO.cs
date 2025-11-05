using UnityEngine;

[CreateAssetMenu(fileName = "BrighterLight", menuName = "Upgrades/Tools/BrighterLight")]
public class BrighterLightSO : ValueModifierAbstract
{
    public float LightIncrease;

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
        referenceValue += CalculateLightIncrease();
    }

    public float CalculateLightIncrease()
    {
        return LightIncrease;
    }
}