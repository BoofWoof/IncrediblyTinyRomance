using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TurkPuzzleScript : MonoBehaviour
{
    [Header("Grid Settings")]
    public PuzzleShapeSO GridData;
    private static PuzzleShapeSO selectedGridData;
    public static float squareSize = 50f;      // Size of each square
    public Sprite squareSprite;         // Sprite to use for the grid squares

    public static List<GameObject> gridSquares = new List<GameObject>();
    public static List<GameObject> puzzlePieceSquares = new List<GameObject>();
    public static List<GameObject> puzzlePiece = new List<GameObject>();

    public static GameObject[,] puzzlePieceGrid;
    public static GameObject[,] holeGrid;
    private static GameObject PuzzleCenter;

    public static TurkPuzzleScript puzzleScript;

    [Header("Piece Storage")]
    public RectTransform PieceHolder;


    void Start()
    {
        selectedGridData = GridData;
        PuzzleCenter = gameObject;
        puzzleScript = this;

        if (selectedGridData != null)
        {
            GeneratePuzzle();
        }
        else
        {
            Debug.LogError("Missing references. Ensure GridData is set.");
        }
    }

    private void GeneratePuzzle()
    {
        GenerateGrid();
        GeneratePuzzlePieces();
        GroupPuzzlePieces();
        PlacePieces();
        ScrambleCords();
    }

    public static bool CheckWin()
    {
        foreach(GameObject gridSquare in gridSquares)
        {
            if(!gridSquare.GetComponent<TurkHoleScript>().isFilled()) return false;
        }
        Debug.Log("You win!");
        puzzleScript.GeneratePuzzle();
        return true;
    }

    private void ScrambleCords()
    {
        foreach (GameObject piece in puzzlePieceSquares)
        {
            piece.GetComponent<TurkCubeScript>().cord = new Vector2Int(100, 100);
        }
    }

    private void PlacePieces()
    {
        MovePieceToHolder();

        foreach (GameObject pieceRoot in puzzlePiece)
        {
            pieceRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(5000, 0);
        }
    }

    public void MovePieceToHolder()
    {
        if (puzzlePiece.Count == 0) return;
        GameObject pieceRoot = puzzlePiece[0];
        Vector2 pieceHolderPos = PieceHolder.anchoredPosition;
        Vector2 centerOffset = pieceRoot.GetComponent<TurkCubeScript>().CalcualteCenterOffset();
        RectTransform rootTransform = pieceRoot.GetComponent<RectTransform>();
        rootTransform.anchoredPosition = pieceHolderPos - centerOffset;
        rootTransform.parent = PieceHolder;

        puzzlePiece.Remove(pieceRoot);
    }

    public static Vector2Int PosToGridIdx(Vector2 Pos)
    {
        Vector2 offset = Pos - PuzzleCenter.GetComponent<RectTransform>().anchoredPosition;
        offset += puzzleScript.GetComponent<RectTransform>().anchoredPosition;

        Vector2 center_idx = new Vector2(selectedGridData.width / 2f, selectedGridData.height / 2f);
        Vector2Int gridIdx = Vector2Int.FloorToInt(offset / squareSize + center_idx);

        return gridIdx;
    }

    public static Vector2 GridIdxToPos(Vector2Int GridIdx)
    {
        Vector2 center_idx = new Vector2(selectedGridData.width / 2f - 0.5f, selectedGridData.height / 2f - 0.5f);
        return new Vector2(
            (GridIdx.x - center_idx.x) * squareSize,
            (GridIdx.y - center_idx.y) * squareSize
            );

    }

    private void GroupPuzzlePieces()
    {
        puzzlePiece = SelectRandomSquares(UnityEngine.Random.Range(selectedGridData.min_pieces, selectedGridData.max_pieces+1));

        foreach (GameObject pieceRoot in puzzlePiece)
        {
            pieceRoot.GetComponent<Image>().color = RandomExtensions.RandomColor();
            pieceRoot.GetComponent<TurkCubeScript>().Linked = true;
            pieceRoot.GetComponent<TurkCubeScript>().PieceRoot = true;
        }

        int linkedPieces = puzzlePiece.Count;

        int breakOutCheck = 0;
        while(linkedPieces < puzzlePieceSquares.Count)
        {
            foreach (GameObject pieceRoot in puzzlePiece)
            {
                TurkCubeScript pieceRootScript = pieceRoot.GetComponent<TurkCubeScript>();
                GameObject newLink = pieceRootScript.AttemptRandomExpand();

                if (newLink == null) continue;
                linkedPieces++;
                newLink.GetComponent<Image>().color = pieceRoot.GetComponent<Image>().color;
                newLink.transform.parent = pieceRoot.transform;
            }

            breakOutCheck++;
            if (breakOutCheck > 500)
            {
                Debug.Log("Early Exit");
                break;
            }
        }
    }
    private List<GameObject> SelectRandomSquares(int numberOfSquares)
    {
        if (numberOfSquares > gridSquares.Count)
        {
            Debug.LogWarning("Requested more squares than available. Returning all squares.");
            return new List<GameObject>(gridSquares);
        }

        return puzzlePieceSquares.OrderBy(x => UnityEngine.Random.value).Take(numberOfSquares).ToList();
    }

    private void GeneratePuzzlePieces()
    {
        // Clear existing squares if any
        foreach (GameObject square in puzzlePieceSquares)
        {
            Destroy(square);
        }
        puzzlePieceSquares.Clear();

        puzzlePieceGrid = new GameObject[selectedGridData.width, selectedGridData.height];

        foreach (GameObject hole in gridSquares)
        {
            // Instantiate a new square
            GameObject newSquare = new GameObject("PuzzlePiece");
            newSquare.transform.parent = transform;

            RectTransform rectTransform = newSquare.AddComponent<RectTransform>();
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(squareSize, squareSize);

            // Set the position of the square
            newSquare.transform.localRotation = Quaternion.identity;
            newSquare.transform.localScale = Vector2.one;
            newSquare.transform.localPosition = hole.transform.localPosition;

            // Add an Image component and set the sprite
            Image imageComponent = newSquare.AddComponent<Image>();
            imageComponent.sprite = squareSprite;
            imageComponent.color = Color.red;

            TurkCubeScript turkCubeScript = newSquare.AddComponent<TurkCubeScript>();
            turkCubeScript.cord = hole.GetComponent<TurkHoleScript>().cord;

            puzzlePieceSquares.Add(newSquare);
            puzzlePieceGrid[turkCubeScript.cord.x, turkCubeScript.cord.y] = newSquare;
        }
    }

    void GenerateGrid()
    {
        // Clear existing squares if any
        foreach (GameObject square in gridSquares)
        {
            Destroy(square);
        }
        gridSquares.Clear();

        holeGrid = new GameObject[selectedGridData.width, selectedGridData.height];

        Vector2 center_idx = new Vector2(selectedGridData.width/2f - 0.5f, selectedGridData.height/2f - 0.5f); 
        for (int y = 0; y < selectedGridData.height; y++)
        {
            for (int x = 0; x < selectedGridData.width; x++)
            {
                // Skip hole positions
                if (selectedGridData.IsHole(x, y))
                {
                    continue;
                }

                // Instantiate a new square
                GameObject newSquare = new GameObject("PuzzleHole");
                newSquare.transform.parent = transform;

                RectTransform rectTransform = newSquare.AddComponent<RectTransform>();
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.sizeDelta = new Vector2(squareSize, squareSize);

                // Set the position of the square
                newSquare.transform.localRotation = Quaternion.identity;
                newSquare.transform.localScale = Vector2.one;
                rectTransform.localPosition = new Vector2(
                    (x - center_idx.x) * squareSize,
                    (y - center_idx.y) * squareSize
                    );

                // Add an Image component and set the sprite
                Image imageComponent = newSquare.AddComponent<Image>();
                imageComponent.sprite = squareSprite;
                imageComponent.color = Color.white;

                // Add a collider.
                newSquare.AddComponent<BoxCollider2D>();

                TurkHoleScript turkCubeScript = newSquare.AddComponent<TurkHoleScript>();
                turkCubeScript.cord = new Vector2Int(x, y);

                gridSquares.Add(newSquare);
                holeGrid[x, y] = newSquare;
            }
        }
    }
    public static bool IsCoordinateInsideGrid(int x, int y)
    {
        // Check if the coordinates are within the bounds of the grid
        if (!(x >= 0 && x < puzzlePieceGrid.GetLength(0) && y >= 0 && y < puzzlePieceGrid.GetLength(1))) return false;
        if (selectedGridData.IsHole(x, y)) return false;
        return true;
    }

    public static bool IsCordTaken(Vector2Int testCord, List<GameObject> mask)
    {
        foreach (GameObject piece in puzzlePieceSquares)
        {
            if (mask.Contains(piece)) continue;

            if(piece.GetComponent<TurkCubeScript>().cord == testCord) return true;
        }
        return false;
    }
}
