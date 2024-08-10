
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TurkGridData", menuName = "ScriptableObjects/TrukGridData", order = 1)]

public class PuzzleShapeSO : ScriptableObject
{
    [Header("Grid Dimensions")]
    public int width = 10;
    public int height = 10;
    public int min_pieces = 8;
    public int max_pieces = 10;

    [Header("Hole Positions")]
    public List<Vector2Int> holePositions = new List<Vector2Int>();

    // Method to check if a position is a hole
    public bool IsHole(int x, int y)
    {
        return holePositions.Contains(new Vector2Int(x, y));
    }
}