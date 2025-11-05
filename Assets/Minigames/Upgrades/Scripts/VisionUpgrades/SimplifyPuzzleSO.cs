using UnityEngine;

[CreateAssetMenu(fileName = "Simplify", menuName = "Upgrades/Tools/Simplify")]
public class SimplifyPuzzleSO : ValueModifierAbstract
{
    [Header("Reward Changes")]
    public int MaxPieceReduction = 1;
    public int MinPieceReduction = 0;
    public float Chance = 0.5f;

    public override string ModifierDescription()
    {
        return "";
    }

    public override void ValueModifier(ref float referenceValue)
    {
        referenceValue -= GetPieceChange();
    }

    public override void OnBuy()
    {
        TurkPuzzleScript.pieceCountModifier += ValueModifier;
    }

    public float GetPieceChange()
    {
        if (Random.value < Chance)
        {
            return MaxPieceReduction;
        }
        return MinPieceReduction;
    }
}
