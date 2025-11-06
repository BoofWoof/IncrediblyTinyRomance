using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurkPuzzleScript : MonoBehaviour
{
    public static TurkPuzzleScript instance;

    public AudioSource Win;
    public AudioSource Pickup;
    public AudioSource Drop;

    public GameObject EmptyTile;
    public TMP_Text PuzzleName;
    public Material ConstMat;

    [Header("Grid Settings")]
    public List<PuzzleShapeSO> VeryEasyPuzzles = new List<PuzzleShapeSO>();
    public List<PuzzleShapeSO> EasyPuzzles = new List<PuzzleShapeSO>();
    public List<PuzzleShapeSO> MediumPuzzles = new List<PuzzleShapeSO>();
    public List<PuzzleShapeSO> HardPuzzles = new List<PuzzleShapeSO>();
    public List<PuzzleShapeSO> VeryHardPuzzles = new List<PuzzleShapeSO>();
    public List<PuzzleShapeSO> TitanPuzzles = new List<PuzzleShapeSO>();
    public List<PuzzleShapeSO> GodPuzzles = new List<PuzzleShapeSO>();

    public static List<List<PuzzleShapeSO>> PuzzlesList = new List<List<PuzzleShapeSO>>();

    public List<Color> ColorsList = new List<Color>();

    public static int CurrentDifficutly = 0;
    public static int DifficultiesUnlocked = 1;
    public Button DifficultyIncreaseButton;
    public Button DifficultyDecreaseButton;

    private static PuzzleShapeSO selectedGridData;
    public static float squareSize = 50f;      // Size of each square
    public TileSetSO constallationTiles;         // Sprite to use for the grid squares

    public static List<GameObject> gridSquares = new List<GameObject>();
    public static List<GameObject> puzzlePieceSquares = new List<GameObject>();
    public static List<GameObject> puzzlePiece = new List<GameObject>();

    public static GameObject[,] puzzlePieceGrid;
    public static GameObject[,] holeGrid;
    private static GameObject PuzzleCenter;

    public static TurkPuzzleScript puzzleScript;

    public delegate void PieceModifier(ref float BaseValue);
    public static PieceModifier pieceCountModifier;

    public ModifierMenuText modifierMenuText;
    private static ModifierMenuText.RewardModifier rewardBaseModifier;
    public static ModifierMenuText.RewardModifier RewardBaseModifier 
    { 
        get => rewardBaseModifier;
        set {
            rewardBaseModifier = value;
            instance.ModifierUpdate();
        }
    }
    private static ModifierMenuText.RewardModifier rewardMultiplier;
    public static ModifierMenuText.RewardModifier RewardMultiplier
    {
        get => rewardMultiplier;
        set
        {
            rewardMultiplier = value;
            instance.ModifierUpdate();
        }
    }

    [Header("Piece Storage")]
    public RectTransform PieceHolder;
    public delegate void PuzzleCompleteCallback(int PuzzlesComplete, TurkPuzzleScript puzzleScript);
    public static PuzzleCompleteCallback OnPuzzleComplete;

    public void UnlockNewDifficulty()
    {
        DifficultiesUnlocked++;
        UpdateDifficultyButtons();
    }

    public void ModifierUpdate()
    {
        modifierMenuText.ModifiersToList = new List<ModifierMenuText.RewardModifier>();
        modifierMenuText.BaseValue = TurkData.CreditsPerPuzzle;
        modifierMenuText.Units = "<sprite index=1>";
        modifierMenuText.BaseText = "Base Earnings";
        modifierMenuText.FinalText = "Earnings Per Puzzle";
        if (rewardBaseModifier != null) modifierMenuText.ModifiersToList.Add(rewardBaseModifier);
        if (rewardMultiplier != null) modifierMenuText.ModifiersToList.Add(rewardMultiplier);
    }

    public void IncreaseDifficulty()
    {
        CurrentDifficutly++;
        if (CurrentDifficutly > 1) CurrentDifficutly = 1;
        puzzleScript.GeneratePuzzle();

        UpdateDifficultyButtons();
    }
    public void DecreaseDifficulty()
    {
        CurrentDifficutly--;
        if (CurrentDifficutly < 0) CurrentDifficutly = 0;
        puzzleScript.GeneratePuzzle();

        UpdateDifficultyButtons();
    }
    void Start()
    {
        instance = this;

        ModifierUpdate();

        PuzzleName.gameObject.SetActive(false);

        PuzzlesList.Add(VeryEasyPuzzles);
        PuzzlesList.Add(EasyPuzzles);
        PuzzlesList.Add(MediumPuzzles);
        PuzzlesList.Add(HardPuzzles);
        PuzzlesList.Add(VeryHardPuzzles);
        PuzzlesList.Add(TitanPuzzles);
        PuzzlesList.Add(GodPuzzles);

        PuzzleCenter = gameObject;
        puzzleScript = this;

        GeneratePuzzle();

        UpdateDifficultyButtons();
    }

    public void UpdateDifficultyButtons()
    {
        DifficultyDecreaseButton.interactable = (CurrentDifficutly != 0);
        DifficultyIncreaseButton.interactable = !(CurrentDifficutly >= DifficultiesUnlocked - 1);
    }

    public static PuzzleShapeSO SamplePuzzles()
    {
        List<PuzzleShapeSO> PuzzleSamples = PuzzlesList[CurrentDifficutly];
        int sampledPuzzleIdx = Random.Range(0, PuzzleSamples.Count);
        return PuzzleSamples[sampledPuzzleIdx];
    }

    private void GeneratePuzzle()
    {
        selectedGridData = SamplePuzzles();
        GenerateGrid();
        GeneratePuzzlePieces();
        GroupPuzzlePieces();
        UpdateTileSprites();
        PlacePieces();
        ScrambleCords();
    }

    private void UpdateTileSprites()
    {
        foreach (GameObject piece in puzzlePieceSquares)
        {
            TurkCubeScript tcs = piece.GetComponent<TurkCubeScript>();
            piece.GetComponent<Image>().sprite = constallationTiles.GetSprite(
                !tcs.ConnectedDirections[Directions.Up],
                !tcs.ConnectedDirections[Directions.Down],
                !tcs.ConnectedDirections[Directions.Left],
                !tcs.ConnectedDirections[Directions.Right]
                );
        }
    }

    public static bool CheckWin()
    {
        foreach(GameObject gridSquare in gridSquares)
        {
            if(!gridSquare.GetComponent<TurkHoleScript>().isFilled()) return false;
        }

        instance.StartCoroutine(instance.WinCutscene());
        return true;
    }

    public void DisablePieces()
    {

    }

    public IEnumerator WinCutscene()
    {
        DisablePieces();
        Win.Play();

        Debug.Log("Turk Puzzle Complete!");
        ApplyReward(CurrentDifficutly);

        float timePass = 0f;
        float transitionPeriod = 1.5f;

        while (timePass < transitionPeriod)
        {
            timePass += Time.deltaTime;
            Shader.SetGlobalFloat("_TurkCompletion", timePass/transitionPeriod);

            yield return null;
        }
        Shader.SetGlobalFloat("_TurkCompletion", 1);

        yield return new WaitForSeconds(0.2f);
        PuzzleName.text = selectedGridData.Name;
        PuzzleName.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        PuzzleName.gameObject.SetActive(false);
        Shader.SetGlobalFloat("_TurkCompletion", 0);
        OnPuzzleComplete?.Invoke(TurkData.PuzzlesSolved, this);
        puzzleScript.GeneratePuzzle();
    }

    public void ApplyReward(int completitionDifficulty)
    {
        int tempDifficulty = CurrentDifficutly;
        CurrentDifficutly = completitionDifficulty;

        TurkData.PuzzlesSolved += 1;
        float reward = TurkData.CreditsPerPuzzle;
        RewardBaseModifier?.Invoke(ref reward);
        RewardMultiplier?.Invoke(ref reward);
        CurrencyData.Credits += reward;

        CurrentDifficutly = tempDifficulty;
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

        Vector2 center_idx = new Vector2(selectedGridData.GetWidth() / 2f, selectedGridData.GetHeight() / 2f);
        Vector2Int gridIdx = Vector2Int.FloorToInt(offset / squareSize + center_idx);

        return gridIdx;
    }

    public static Vector2 GridIdxToPos(Vector2Int GridIdx)
    {
        Vector2 center_idx = new Vector2(selectedGridData.GetWidth() / 2f - 0.5f, selectedGridData.GetHeight() / 2f - 0.5f);
        return new Vector2(
            (GridIdx.x - center_idx.x) * squareSize,
            (GridIdx.y - center_idx.y) * squareSize
            );

    }

    private void GroupPuzzlePieces()
    {
        float pieceCount = Random.Range(selectedGridData.min_pieces, selectedGridData.max_pieces + 1);

        pieceCountModifier?.Invoke(ref pieceCount);

        if (pieceCount < 2) pieceCount = 2;
        puzzlePiece = SelectRandomSquares((int)pieceCount);

        int GroupIdx = 0;
        foreach (GameObject pieceRoot in puzzlePiece)
        {
            pieceRoot.GetComponent<Image>().color = ColorsList[GroupIdx];
            pieceRoot.GetComponent<Image>().material.SetColor("_Tint", ColorsList[GroupIdx]);

            pieceRoot.GetComponent<TurkCubeScript>().Linked = true;
            pieceRoot.GetComponent<TurkCubeScript>().PieceRoot = true;
            pieceRoot.GetComponent<TurkCubeScript>().GroupID = GroupIdx;
            GroupIdx++;
        }

        int linkedPieces = puzzlePiece.Count;

        int breakOutCheck = 0;
        bool searchDiagonally = false;
        while (linkedPieces < puzzlePieceSquares.Count)
        {
            bool newLinkFound = false;
            foreach (GameObject pieceRoot in puzzlePiece)
            {
                TurkCubeScript pieceRootScript = pieceRoot.GetComponent<TurkCubeScript>();
                GameObject newLink = pieceRootScript.AttemptRandomExpand(searchDiagonally);

                if (newLink == null) continue;
                newLinkFound = true;
                linkedPieces++;
                newLink.GetComponent<Image>().color = pieceRoot.GetComponent<Image>().color;
                newLink.GetComponent<Image>().material.SetColor("_Tint", pieceRoot.GetComponent<Image>().color);
                newLink.transform.parent = pieceRoot.transform;
            }
            if (!newLinkFound) searchDiagonally = true;

            breakOutCheck++;
            if (breakOutCheck > 500)
            {
                Debug.Log("Early Exit");
                break;
            }
        }
        //Second connection pass.
        foreach (GameObject piece in puzzlePieceSquares)
        {
            TurkCubeScript pieceScript = piece.GetComponent<TurkCubeScript>();
            pieceScript.ConnectionCheck();

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

        puzzlePieceGrid = new GameObject[selectedGridData.GetWidth(), selectedGridData.GetHeight()];

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
            imageComponent.sprite = constallationTiles.GetSprite(true, true, true, true);
            imageComponent.color = Color.white;
            imageComponent.material = ConstMat;
            imageComponent.material.SetTexture("_MainTex", imageComponent.sprite.ExtractSpriteTexture());

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

        holeGrid = new GameObject[selectedGridData.GetWidth(), selectedGridData.GetHeight()];

        Vector2 center_idx = new Vector2(selectedGridData.GetWidth()/2f - 0.5f, selectedGridData.GetHeight()/2f - 0.5f); 
        for (int y = 0; y < selectedGridData.GetHeight(); y++)
        {
            for (int x = 0; x < selectedGridData.GetWidth(); x++)
            {
                // Skip hole positions
                if (selectedGridData.IsHole(x, y))
                {
                    continue;
                }

                // Instantiate a new square
                GameObject newSquare = Instantiate(EmptyTile);
                newSquare.transform.parent = transform;

                RectTransform rectTransform = newSquare.GetComponent<RectTransform>();
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
                Image imageComponent = newSquare.GetComponent<Image>();
                imageComponent.color = new Color(255, 255, 255, 1f);

                // Add a collider.
                newSquare.GetComponent<BoxCollider2D>();

                TurkHoleScript turkCubeScript = newSquare.GetComponent<TurkHoleScript>();
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
