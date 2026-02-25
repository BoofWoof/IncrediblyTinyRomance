using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VisionsDifficultySO", menuName = "Visions/VisionsDifficultySO")]
public class VisionsDifficultySO : ScriptableObject
{
    public string PuzzleSetName = "NameMePlease";
    public List<PuzzleShapeSO> Puzzles;
    public float DarknessModifier = 1f;
    public float OpennessModifier = 1f;
    public float FalconSpeed = 15f;
    public float MiloRecord = 10f;
    public int MinPieces = 4;
    public int MaxPieces = 5;
}
