using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurkCubeScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector2Int cord;

    public Dictionary<Directions, bool> CardinalExpands = new Dictionary<Directions, bool>
    {
        {Directions.Up, false},
        {Directions.Left, false},
        {Directions.Right, false},
        {Directions.Down, false}
    };

    public Dictionary<Directions, bool> DiagonalExpands = new Dictionary<Directions, bool>
    {
        {Directions.LowerLeft, false},
        {Directions.UpperLeft, false},
        {Directions.LowerRight, false},
        {Directions.UpperRight, false}
    };

    public Dictionary<Directions, bool> ConnectedDirections = new Dictionary<Directions, bool>
    {
        {Directions.Up, false},
        {Directions.Left, false},
        {Directions.Right, false},
        {Directions.Down, false}
    };

    public bool FullyCardinallyExpanded = false;
    public bool FullyDiagonallyExpanded = false;

    public bool Linked = false;
    private bool FirstRelease = true;

    public bool PieceRoot = false;

    private bool isDragging;
    private Vector2 offset;

    public int GroupID = -1;

    public List<TurkCubeScript> ExpandedToScripts = new List<TurkCubeScript>();

    #region Follow Mouse
    void Update()
    {
        if (isDragging)
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                TrukAppScript.PhoneScreenCanvas.transform as RectTransform,
                Input.mousePosition,
                TrukAppScript.PhoneScreenCanvas.worldCamera,
                out mousePos);

            GameObject rootPiece = null;
            if (PieceRoot)
            {
                rootPiece = gameObject;
            }
            else {
                rootPiece = transform.parent.gameObject;
            }
            rootPiece.GetComponent<RectTransform>().anchoredPosition = mousePos + offset;
        }
    }

    public Vector2 CalcualteCenterOffset()
    {
        // Initialize min and max with the first point in the list
        Vector2 min = Vector2.zero;
        Vector2 max = Vector2.zero;

        // Iterate through the list to find min and max x and y values
        foreach (RectTransform child in transform)
        {
            Vector2 point = child.anchoredPosition;
            if (point.x < min.x) min.x = point.x;
            if (point.y < min.y) min.y = point.y;

            if (point.x > max.x) max.x = point.x;
            if (point.y > max.y) max.y = point.y;
        }

        return new Vector2((min.x + max.x)/2f, (min.y + max.y) / 2f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        TurkPuzzleScript.instance.Pickup.Play();

        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            TrukAppScript.PhoneScreenCanvas.transform as RectTransform,
            Input.mousePosition,
            TrukAppScript.PhoneScreenCanvas.worldCamera,
            out mousePos);

        GameObject rootPiece;
        if (PieceRoot)
        {
            rootPiece = gameObject;
        }
        else
        {
            rootPiece = transform.parent.gameObject;
        }

        rootPiece.transform.parent = TurkPuzzleScript.puzzleScript.transform;

        offset = rootPiece.GetComponent<RectTransform>().anchoredPosition - mousePos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        TurkPuzzleScript.instance.Drop.Play();
        TurkPuzzleScript.instance.Pickup.Stop();

        GameObject rootPiece;
        if (PieceRoot)
        {
            rootPiece = gameObject;
        }
        else
        {
            rootPiece = transform.parent.gameObject;
        }
        TurkCubeScript rootPieceScript = rootPiece.GetComponent<TurkCubeScript>();

        Vector2 startPos = rootPiece.GetComponent<RectTransform>().anchoredPosition;
        Vector2Int gridIdx = TurkPuzzleScript.PosToGridIdx(startPos);
        Vector2 pos = TurkPuzzleScript.GridIdxToPos(gridIdx);
        rootPiece.GetComponent<RectTransform>().anchoredPosition = pos;

        bool successfulUpdate = rootPieceScript.UpdateCord();

        if (TurkPuzzleScript.CheckWin()) return;

        if (rootPieceScript.FirstRelease)
        {
            if (successfulUpdate)
            {
                rootPieceScript.FirstRelease = false;
                TurkPuzzleScript.puzzleScript.MovePieceToHolder();
                return;
            } else
            {
                rootPiece.transform.parent = TurkPuzzleScript.puzzleScript.PieceHolder.transform;
                rootPiece.GetComponent<RectTransform>().anchoredPosition = -rootPieceScript.CalcualteCenterOffset();
                return;
            }
        }

        rootPiece.GetComponent<RectTransform>().anchoredPosition = TurkPuzzleScript.GridIdxToPos(rootPieceScript.cord);
    }

    public bool UpdateCord()
    {
        //Returns false if failed.
        List<GameObject> puzzlePieces = new List<GameObject>();
        puzzlePieces.Add(gameObject);
        foreach (RectTransform child in transform)
        {
            puzzlePieces.Add(child.gameObject);
        }

        foreach (GameObject puzzlePiece in puzzlePieces)
        {
            RectTransform puzzleTransform = puzzlePiece.GetComponent<RectTransform>();

            Vector2Int newCord;
            if (puzzlePiece.GetComponent<TurkCubeScript>().PieceRoot)
            {
                newCord = TurkPuzzleScript.PosToGridIdx(puzzleTransform.anchoredPosition);
            }
            else
            {
                newCord = TurkPuzzleScript.PosToGridIdx(GetComponent<RectTransform>().anchoredPosition + puzzleTransform.anchoredPosition);
            }

            if (TurkPuzzleScript.IsCordTaken(newCord, puzzlePieces)) return false;
            if (newCord.x > 13) return false; //Stops from placing on glass. Sorry for magic number. <3
        }

        List<TurkCubeScript> fillers = new List<TurkCubeScript>();

        foreach (GameObject puzzlePiece in puzzlePieces)
        {
            RectTransform puzzleTransform = puzzlePiece.GetComponent<RectTransform>();
            Vector2Int oldCord = puzzlePiece.GetComponent<TurkCubeScript>().cord;

            Vector2Int newCord;
            if (puzzlePiece.GetComponent<TurkCubeScript>().PieceRoot)
            {
                newCord = TurkPuzzleScript.PosToGridIdx(puzzleTransform.anchoredPosition);
            }
            else
            {
                newCord = TurkPuzzleScript.PosToGridIdx(GetComponent<RectTransform>().anchoredPosition + puzzleTransform.anchoredPosition);
            }
            puzzleTransform.GetComponent<TurkCubeScript>().cord = newCord;
            if (TurkPuzzleScript.IsCoordinateInsideGrid(oldCord.x, oldCord.y)) TurkPuzzleScript.holeGrid[oldCord.x, oldCord.y].GetComponent<TurkHoleScript>().EmptyHole();
            fillers.Add(puzzlePiece.GetComponent<TurkCubeScript>());
        }
        foreach (TurkCubeScript filler in fillers)
        {
            Vector2Int cord = filler.cord;
            if (TurkPuzzleScript.IsCoordinateInsideGrid(cord.x, cord.y)) TurkPuzzleScript.holeGrid[cord.x, cord.y].GetComponent<TurkHoleScript>().FillHole(filler);
        }
        return true;
    }
    #endregion

    #region Find Other Pieces

    public GameObject AttemptRandomExpand(bool expandDiagonally = false)
    {
        foreach (TurkCubeScript ExpandedToScript in ExpandedToScripts)
        {
            GameObject expandToObject = ExpandedToScript.AttemptRandomExpand(expandDiagonally);
            if (expandToObject != null) return expandToObject;
        }
        if (FullyCardinallyExpanded && !expandDiagonally) return null;
        if (FullyDiagonallyExpanded && expandDiagonally) return null;

        List<Directions> directionsLeft = new List<Directions>();
        if (expandDiagonally)
        {
            foreach(KeyValuePair<Directions, bool> expansionKeyValue in DiagonalExpands)
            {
                if(!expansionKeyValue.Value) directionsLeft.Add(expansionKeyValue.Key);
            }
        } else
        {
            foreach (KeyValuePair<Directions, bool> expansionKeyValue in CardinalExpands)
            {
                if (!expansionKeyValue.Value) directionsLeft.Add(expansionKeyValue.Key);
            }
        }
        if (directionsLeft.Count == 0)
        {
            if (expandDiagonally)
            {
                FullyDiagonallyExpanded = true;
            } else
            {
                FullyCardinallyExpanded = true;
            }
            return null;
        }

        while (directionsLeft.Count > 0)
        {
            Directions RandomDir = directionsLeft[Random.Range(0, directionsLeft.Count)];
            GameObject selectedObject = Expand(RandomDir);
            directionsLeft.Remove(RandomDir);

            if (selectedObject != null) return selectedObject;
        }
        return null;
    }

    private GameObject Expand(Directions ExpandDirection)
    {
        if (CardinalExpands.ContainsKey(ExpandDirection)) CardinalExpands[ExpandDirection] = true;
        if (DiagonalExpands.ContainsKey(ExpandDirection)) DiagonalExpands[ExpandDirection] = true;


        Vector2Int expandCordShift = ExpandDirection.ToCordShift();
        if (!TurkPuzzleScript.IsCoordinateInsideGrid(cord.x + expandCordShift.x, cord.y + expandCordShift.y)) return null;
        GameObject expandedTo = TurkPuzzleScript.puzzlePieceGrid[cord.x + expandCordShift.x, cord.y + expandCordShift.y];
        TurkCubeScript expandToScript = expandedTo.GetComponent<TurkCubeScript>();

        if (expandToScript.Linked &&
            expandToScript.GroupID != GroupID
            ) return null;

        if (CardinalExpands.ContainsKey(ExpandDirection))
        {
            ConnectedDirections[ExpandDirection] = true;
            expandToScript.ConnectedDirections[ExpandDirection.FlipDirection()] = true;
        }

        if (expandToScript.Linked) return null;

        if (!ExpandedToScripts.Contains(expandToScript)) ExpandedToScripts.Add(expandToScript);
        expandToScript.Linked = true;
        expandToScript.GroupID = GroupID;

        return expandedTo;
    }

    public void ConnectionCheck()
    {
        foreach (Directions connectionKey in DirectionHelper.CardinalDirections)
        {
            Vector2Int posShift = connectionKey.ToCordShift();
            if (!TurkPuzzleScript.IsCoordinateInsideGrid(cord.x + posShift.x, cord.y + posShift.y))
            {
                ConnectedDirections[connectionKey] = false;
            }
            else
            {
                ConnectedDirections[connectionKey] = TurkPuzzleScript.puzzlePieceGrid[cord.x + posShift.x, cord.y + posShift.y].GetComponent<TurkCubeScript>().GroupID == GroupID;
            }
        }
    }
    #endregion
}
