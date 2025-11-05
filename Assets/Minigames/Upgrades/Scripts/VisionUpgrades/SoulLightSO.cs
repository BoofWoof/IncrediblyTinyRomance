using UnityEngine;

[CreateAssetMenu(fileName = "SoulLight", menuName = "Upgrades/Synergies/SoulLight")]
public class SoulLightSO : ValueModifierAbstract
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
        return Mathf.Lerp(LightIncrease, 0, PrayerScript.instance.GetAngerLevel());
    }
}
