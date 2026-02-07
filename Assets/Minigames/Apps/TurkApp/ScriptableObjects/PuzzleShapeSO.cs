using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TurkGridData", menuName = "ScriptableObjects/TrukGridData", order = 1)]

public class PuzzleShapeSO : ScriptableObject
{
    public string Name;
    public string Artist;

    public Texture2D puzzleTexture;

    public int min_pieces = 8;
    public int max_pieces = 10;

    private bool[] holeMap;
    private int width;

    private void OnValidate()
    {
        if (puzzleTexture == null)
            return;

        width = puzzleTexture.width;
        int height = puzzleTexture.height;

        Color[] pixels = puzzleTexture.GetPixels();
        holeMap = new bool[pixels.Length];

        for (int i = 0; i < pixels.Length; i++)
            holeMap[i] = pixels[i].grayscale < 0.5f;
    }

    public bool IsHole(int x, int y)
    {
        return holeMap[y * width + x];
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