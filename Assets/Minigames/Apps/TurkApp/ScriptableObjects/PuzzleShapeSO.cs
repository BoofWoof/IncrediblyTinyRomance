
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TurkGridData", menuName = "ScriptableObjects/TrukGridData", order = 1)]

public class PuzzleShapeSO : ScriptableObject
{
    public string Name;

    public Texture2D puzzleTexture;

    public int min_pieces = 8;
    public int max_pieces = 10;

    // Method to check if a position is a hole
    public bool IsHole(int x, int y)
    {
        Color pixels = puzzleTexture.GetPixel(x, y);
        return pixels.grayscale < 0.5f;
    }

    public int GetWidth()
    {
        return puzzleTexture.width;
    }

    public int GetHeight()
    {
        return puzzleTexture.height;
    }
}