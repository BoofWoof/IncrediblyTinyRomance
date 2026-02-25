using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public struct SecondaryMultiplier
{
    public float multiplier;
    public string description;
}

public class TurkPuzzleScript : MonoBehaviour
{
    public static TurkPuzzleScript instance;

    public UnityEvent OnPuzzleGenerate;
    public UnityEvent<int> OnDifficultyUp;
    public UnityEvent<int> OnDifficultyDown;

    private bool FirstOpen = true;

    [Header("Stats")]
    [HideInInspector] public float StartingTime;
    public static Dictionary<int, float> TimeRecords = new Dictionary<int, float>();
    public static Dictionary<int, int> PuzzlesCompleted = new Dictionary<int, int>();

    public TMP_Text PuzzleSolvedText;
    public TMP_Text BestTimeText;
    public TMP_Text PuzzleEarningsText;
    public TMP_Text UniquePuzzlesSolvedText;
    public TMP_Text ScoreMultiplierText;
    public GameObject ClickToContinueText;
    public GameObject InteractionBlocker;

    [Header("Objects")]
    public TMP_Text PuzzleName;
    public TMP_Text NewRecordText;
    public ParticleSystem NewRecordParticles;
    public ParticleSystem NewRecordParticles2;

    public int RepeatsBannedFor = 3;

    public AudioSource Win;
    public AudioSource MultiplierSource;
    public AudioSource NewRecordSource;
    public AudioSource Pickup;
    public AudioSource Drop;
    public AudioSource DropBad;

    public GameObject EmptyTile;
    public Material ConstMat;

    public TMP_Text ArtistCredit;

    [Header("Grid Settings")]
    public List<VisionsDifficultySO> LevelSets;

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

    public delegate void SecondaryMuliplierListModifier(ref List<SecondaryMultiplier> BaseList);
    public static SecondaryMuliplierListModifier secondaryMuliplierListModifier;

    public ModifierMenuText modifierMenuText;
    private static ModifierMenuText.RewardModifier rewardBaseModifier;

    public Image CloudPanel;
    public static ModifierMenuText.RewardModifier RewardBaseModifier 
    { 
        get => rewardBaseModifier;
        set {
            rewardBaseModifier = value;
        }
    }
    private static ModifierMenuText.RewardModifier rewardMultiplier;
    public static ModifierMenuText.RewardModifier RewardMultiplier
    {
        get => rewardMultiplier;
        set
        {
            rewardMultiplier = value;
        }
    }

    [Header("Piece Storage")]
    public RectTransform PieceHolder;
    public delegate void PuzzleCompleteCallback(int PuzzlesComplete, TurkPuzzleScript puzzleScript);
    public static PuzzleCompleteCallback OnPuzzleComplete;


    public void OnAppOpen()
    {
        if (!FirstOpen) return;
        FirstOpen = false;
        StartingTime = Time.time;
    }
    public void UpdateStatText()
    {
        if (!PuzzlesCompleted.ContainsKey(CurrentDifficutly))
        {
            PuzzleSolvedText.text = "<b>Puzzles Solved:</b> 0";
        } else
        {
            PuzzleSolvedText.text = "<b>Puzzles Solved:</b> " + PuzzlesCompleted[CurrentDifficutly].ToString();
        }
        if (!TimeRecords.ContainsKey(CurrentDifficutly))
        {
            BestTimeText.text = "<b>Fastest Time:</b> ?:??";
        }
        else
        {
            BestTimeText.text = "<b>Fastest Time:</b> " + System.TimeSpan.FromSeconds(TimeRecords[CurrentDifficutly]).ToString("m\\:ss");
        }
    }

    public void UnlockNewDifficulty()
    {
        DifficultiesUnlocked++;
        UpdateDifficultyButtons();
    }

    public void IncreaseDifficulty()
    {
        CurrentDifficutly++;
        if (CurrentDifficutly >= DifficultiesUnlocked - 1) CurrentDifficutly = DifficultiesUnlocked - 1;
        puzzleScript.GeneratePuzzle();

        UpdateDifficultyButtons();
        OnDifficultyUp?.Invoke(CurrentDifficutly);
    }
    public void DecreaseDifficulty()
    {
        CurrentDifficutly--;
        if (CurrentDifficutly < 0) CurrentDifficutly = 0;
        puzzleScript.GeneratePuzzle();

        UpdateDifficultyButtons();
        OnDifficultyDown?.Invoke(CurrentDifficutly);
    }
    void Awake()
    {
        instance = this;

        Shader.SetGlobalFloat("_TurkCompletion", 0);

        NewRecordText.gameObject.SetActive(false);
        ScoreMultiplierText.gameObject.SetActive(false);
        ClickToContinueText.SetActive(false);
        InteractionBlocker.SetActive(false);


        ArtistCredit.text = "";

        PuzzleName.gameObject.SetActive(false);

        PuzzleCenter = gameObject;
        puzzleScript = this;

        GeneratePuzzle();

        UpdateDifficultyButtons();

    }

    public void UpdateDifficultyButtons()
    {
        DifficultyDecreaseButton.interactable = (CurrentDifficutly != 0);
        DifficultyIncreaseButton.interactable = !(CurrentDifficutly >= DifficultiesUnlocked - 1);

        StartCoroutine(OpenSkyHole(LevelSets[CurrentDifficutly].OpennessModifier));
    }

    public static PuzzleShapeSO SamplePuzzles()
    {
        List<PuzzleShapeSO> PuzzleSamples = instance.LevelSets[CurrentDifficutly].Puzzles;

        int puzzleIdx = 0;
        if (PuzzlesCompleted.ContainsKey(CurrentDifficutly))
        {
            puzzleIdx = PuzzlesCompleted[CurrentDifficutly]%PuzzleSamples.Count;   
        }

        return PuzzleSamples[puzzleIdx];
    }

    private void UpdatePuzzleIdx()
    {
        UniquePuzzlesSolvedText.color = Color.white;
        int puzzleIdx = 0;
        if (PuzzlesCompleted.ContainsKey(CurrentDifficutly))
        {
            puzzleIdx = PuzzlesCompleted[CurrentDifficutly];
        }
        int maxLength = LevelSets[CurrentDifficutly].Puzzles.Count;
        if (puzzleIdx >= maxLength)
        {
            UniquePuzzlesSolvedText.color = Color.green;
            puzzleIdx = maxLength;
        }
        UniquePuzzlesSolvedText.text = puzzleIdx.ToString() + "/" + maxLength.ToString();
    }

    private void GeneratePuzzle()
    {
        PuzzleEarningsText.gameObject.SetActive(false);
        UpdatePuzzleIdx();

        selectedGridData = SamplePuzzles();
        GenerateGrid();
        GeneratePuzzlePieces();
        GroupPuzzlePieces();
        UpdateTileSprites();
        PlacePieces();
        ScrambleCords();
        ShowArtist();
        UpdateStatText();
        StartingTime = Time.time;

        OnPuzzleGenerate?.Invoke();
    }

    public void ShowArtist()
    {
        if (selectedGridData == null)
        {
            ArtistCredit.text = "";
            return;
        }
        if (selectedGridData.Artist.Length > 0)
        {
            ArtistCredit.text = "Artist: " + selectedGridData.Artist;
        } else
        {
            ArtistCredit.text = "";
        }
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

    public IEnumerator WinCutscene()
    {
        PieceHolder.gameObject.SetActive(false);
        InteractionBlocker.SetActive(true);

        bool newBestTime = false;
        if (!PuzzlesCompleted.ContainsKey(CurrentDifficutly))
        {
            PuzzlesCompleted[CurrentDifficutly] = 1;
        }
        else
        {
            PuzzlesCompleted[CurrentDifficutly] += 1;
        }
        float TotalTime = Time.time - StartingTime;
        if (!TimeRecords.ContainsKey(CurrentDifficutly))
        {
            newBestTime = true;
            TimeRecords[CurrentDifficutly] = TotalTime;
            NewRecordText.text = "NEW BEST TIME " + System.TimeSpan.FromSeconds(TimeRecords[CurrentDifficutly]).ToString("m\\:ss");
        }
        else
        {
            if ( TotalTime < TimeRecords[CurrentDifficutly])
            {
                newBestTime = true;
                TimeRecords[CurrentDifficutly] = TotalTime;
                NewRecordText.text = "NEW BEST TIME " + System.TimeSpan.FromSeconds(TimeRecords[CurrentDifficutly]).ToString("m\\:ss");
            }
        }
        UpdateStatText();


        TurkCubeScript.PickupEnabled = false;
        Win.Play();

        Debug.Log("Turk Puzzle Complete!");
        //Show Earnings
        PuzzleName.text = selectedGridData.Name;
        PuzzleName.gameObject.SetActive(true);
        PuzzleEarningsText.gameObject.SetActive(true);
        PuzzleEarningsText.text = "";

        float reward = CalculateReward(CurrentDifficutly);

        PuzzleEarningsText.text = "+ <sprite index=1> ";
        string finalEarningText = reward.AllSignificantDigits(3);

        //Puzzle Material Update
        float timePass = 0f;
        float transitionPeriod = 1.5f;
        while (timePass < transitionPeriod)
        {
            timePass += Time.deltaTime;
            float progress = timePass / transitionPeriod;
            Shader.SetGlobalFloat("_TurkCompletion", progress);

            int showCharacters = (int)Mathf.Lerp(0, finalEarningText.Length, progress);
            PuzzleEarningsText.text = "+ <sprite index=1> " + finalEarningText.Substring(0, showCharacters);

            yield return null;
        }
        Shader.SetGlobalFloat("_TurkCompletion", 1);
        PuzzleEarningsText.text = "+ <sprite index=1> " + finalEarningText;

        //Show Multipliers
        List<SecondaryMultiplier> secondaryMultipliers = new List<SecondaryMultiplier>();
        secondaryMuliplierListModifier?.Invoke(ref secondaryMultipliers);
        ScoreMultiplierText.text = "";
        foreach (SecondaryMultiplier secondaryMultiplier in secondaryMultipliers)
        {
            MultiplierSource.Play();
            ScoreMultiplierText.gameObject.SetActive(true);
            ScoreMultiplierText.text += secondaryMultiplier.description + "\r\n";
            reward *= secondaryMultiplier.multiplier;
            PuzzleEarningsText.text = "+ <sprite index=1> " + reward.AllSignificantDigits(2);
            yield return new WaitForSeconds(0.4f);
        }

        CurrencyData.Credits += reward;
        VisionMascotScript.SayText(selectedGridData.MascotStatement);

        if (newBestTime)
        {
            NewRecordText.gameObject.SetActive(true);
            NewRecordSource.Play();
            NewRecordParticles.Play();
            NewRecordParticles2.Play();
            yield return new WaitForSeconds(1.5f);
        }

        ClickToContinueText.SetActive(true);
        while (true)
        {
            yield return null;
            if(Input.GetMouseButtonDown(0)) break;
        }
        ClickToContinueText.SetActive(false);

        VisionMascotScript.ClearText();

        ScoreMultiplierText.gameObject.SetActive(false);

        PuzzleName.gameObject.SetActive(false);
        NewRecordText.gameObject.SetActive(false);
        Shader.SetGlobalFloat("_TurkCompletion", 0);
        OnPuzzleComplete?.Invoke(TurkData.PuzzlesSolved, this);
        puzzleScript.GeneratePuzzle();
        TurkCubeScript.PickupEnabled = true;

        InteractionBlocker.SetActive(false);
    }

    public float CalculateReward(int completitionDifficulty)
    {
        int tempDifficulty = CurrentDifficutly;
        CurrentDifficutly = completitionDifficulty;

        TurkData.PuzzlesSolved += 1;
        float reward = TurkData.CreditsPerPuzzle;
        RewardBaseModifier?.Invoke(ref reward);
        RewardMultiplier?.Invoke(ref reward);

        CurrentDifficutly = tempDifficulty;

        return reward;
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
        if (puzzlePiece.Count == 0)
        {
            TurkCubeScript.PieceHolderRestraint = false;
            PieceHolder.gameObject.SetActive(false);
            return;
        }
        GameObject pieceRoot = puzzlePiece[0];
        pieceRoot.GetComponent<TurkCubeScript>().SendToPieceHolder();
        /*
            Vector2 pieceHolderPos = PieceHolder.anchoredPosition;
            Vector2 centerOffset = pieceRoot.GetComponent<TurkCubeScript>().CalcualteCenterOffset();
            RectTransform rootTransform = pieceRoot.GetComponent<RectTransform>();
            rootTransform.anchoredPosition = pieceHolderPos - centerOffset;
            rootTransform.parent = PieceHolder;
        */

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
        VisionsDifficultySO currentDifficulty = LevelSets[CurrentDifficutly];
        float pieceCount = Random.Range(currentDifficulty.MinPieces, currentDifficulty.MaxPieces + 1);

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

        while (linkedPieces < puzzlePieceSquares.Count)
        {
            foreach (GameObject pieceRoot in puzzlePiece)
            {
                TurkCubeScript pieceRootScript = pieceRoot.GetComponent<TurkCubeScript>();
                GameObject newLink = pieceRootScript.AttemptRandomExpand();

                if (newLink == null) continue;
                linkedPieces++;
                newLink.GetComponent<Image>().color = pieceRoot.GetComponent<Image>().color;
                newLink.GetComponent<Image>().material.SetColor("_Tint", pieceRoot.GetComponent<Image>().color);
                newLink.transform.parent = pieceRoot.transform;
            }

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

        List<int> allPieceIndexes = Enumerable.Range(0,puzzlePieceSquares.Count).ToList();
        int retryCount = 15;
        List<int> selectedPieceIdx = new List<int>();
        List<GameObject> selectedPieces = new List<GameObject>();

        int sampleIdx = allPieceIndexes[UnityEngine.Random.Range(0, allPieceIndexes.Count)];

        selectedPieceIdx.Add(sampleIdx);
        selectedPieces.Add(puzzlePieceSquares[sampleIdx]);
        allPieceIndexes.Remove(sampleIdx);

        for (int i = 0; i < numberOfSquares-1; i++)
        {
            float furthestPiece = -1;
            int bestIdx = -1;

            for (int r = 0; r < retryCount; r++)
            {
                sampleIdx = allPieceIndexes[UnityEngine.Random.Range(0, allPieceIndexes.Count)];
                Vector2Int selectCord = puzzlePieceSquares[sampleIdx].GetComponent<TurkCubeScript>().cord;

                int closestDistance = 100000000;
                foreach (int preselectedIdx in selectedPieceIdx)
                {
                    Vector2Int preSelectCord = puzzlePieceSquares[preselectedIdx].GetComponent<TurkCubeScript>().cord;
                    int distance = Mathf.Abs(selectCord.x - preSelectCord.x) + Mathf.Abs(selectCord.x - preSelectCord.x);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                    }
                }

                if (closestDistance > furthestPiece)
                {
                    furthestPiece = closestDistance;
                    bestIdx = sampleIdx;
                }
            }
            selectedPieceIdx.Add(bestIdx);
            selectedPieces.Add(puzzlePieceSquares[bestIdx]);
            allPieceIndexes.Remove(bestIdx);
        }

        return selectedPieces;
    }

    private void GeneratePuzzlePieces()
    {
        TurkCubeScript.PieceHolderRestraint = true;
        PieceHolder.gameObject.SetActive(true);
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
    public IEnumerator OpenSkyHole(float finalValue)
    {
        float timePassed = 0f;
        float transitionPeriod = 2f;

        float startingValue = CloudPanel.materialForRendering.GetFloat("_HoleSize");

        while (timePassed < transitionPeriod)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / transitionPeriod;

            CloudPanel.materialForRendering.SetFloat("_HoleSize", Mathf.Lerp(startingValue, finalValue, progress));

            yield return null;
        }
        CloudPanel.materialForRendering.SetFloat("_HoleSize", finalValue);
    }

    public static bool isDifficultyCompleted(int difficultyLevel)
    {
        if (!PuzzlesCompleted.ContainsKey(difficultyLevel)) return false;
        return PuzzlesCompleted[difficultyLevel] >= instance.LevelSets[difficultyLevel].Puzzles.Count;
    }
}
