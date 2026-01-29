using UnityEngine;

[CreateAssetMenu(fileName = "Speedster", menuName = "Upgrades/Challenges/Speedster")]
public class SpeedsterSO : ValueModifierAbstract
{
    public float SlowestTime;
    public float FastestTime;
    public float MaxMultiplier;
    public float MinMultiplier;

    public override string ModifierDescription()
    {
        return "";
    }

    public override void OnBuy()
    {
        TurkPuzzleScript.RewardMultiplier += ValueModifier;
    }

    public override void ValueModifier(ref float referenceValue)
    {
        float multiplier = CalculateSpeedMultiplier();
        TurkPuzzleScript.instance.DisplayedMultiplier *= multiplier;
    }

    public float CalculateSpeedMultiplier()
    {
        float timePassed = Time.time - TurkPuzzleScript.instance.StartingTime;
        float rawMultValue = (SlowestTime - timePassed) / (SlowestTime - FastestTime);
        float clampedMultValue = Mathf.Clamp01(rawMultValue);
        float finalMultiplier = Mathf.Lerp(MinMultiplier, MaxMultiplier, clampedMultValue);

        Debug.Log(finalMultiplier);
        return finalMultiplier;
    }
}
