using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "HealOnComplete", menuName = "Upgrades/HealOnComplete")]
public class HealOnCompleteSO : ValueModifierAbstract
{
    [Header("Reward Changes")]
    public float HealAmount = 3f;

    public override string ModifierDescription()
    {
        return "";
    }

    public override void OnBuy()
    {
        TurkPuzzleScript.OnPuzzleComplete += HealCity;
    }

    private void HealCity(int PuzzlesComplete, TurkPuzzleScript puzzleScript)
    {
        DefenseStats.DamageCity(-HealAmount);
    }

    public override void ValueModifier(ref float referenceValue)
    {
        return;
    }
}
