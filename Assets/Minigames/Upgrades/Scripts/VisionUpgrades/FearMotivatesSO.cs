using System.Drawing;
using UnityEngine;

[CreateAssetMenu(fileName = "FearMotivates", menuName = "Upgrades/FearMotivates")]
public class FearMotivatesSO : ValueModifierAbstract
{
    [Header("Reward Changes")]
    public float PointPerPoint;

    public override string ModifierDescription()
    {
        return UpgradeName + ": x" + GetAngerMultiplier().NumberToString(true) + "<color=#808080>(1 + " + PrayerScript.instance.RamAngyLevel.ToString("n0") + "/" + PointPerPoint.ToString() + ")</color>";
    }

    public override void ValueModifier(ref float referenceValue)
    {
        referenceValue *= GetAngerMultiplier();
    }

    public override void OnBuy()
    {
        TurkPuzzleScript.RewardMultiplier += ValueModifier;
    }

    public float GetAngerMultiplier()
    {
        return 1 + PrayerScript.instance.RamAngyLevel / PointPerPoint;
    }
}
