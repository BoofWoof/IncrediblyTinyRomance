using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class TurkCubeScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector2Int cord;

    public bool ExpandedUp = false;
    public bool ExpandedDown = false;
    public bool ExpandedLeft = false;
    public bool ExpandedRight = false;

    public bool FullyExpanded = false;

    public bool Linked = false;
    private bool FirstRelease = true;

    public bool PieceRoot = false;

    private bool isDragging;
    private Vector2 offset;

    private List<TurkCubeScript> ExpandedToScripts = new List<TurkCubeScript>();

    enum Directions
    {
        Left,
        Right,
        Up,
        Down
    }
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
    public GameObject AttemptRandomExpand()
    {
        foreach (TurkCubeScript ExpandedToScript in ExpandedToScripts)
        {
            GameObject expandToObject = ExpandedToScript.AttemptRandomExpand();
            if (expandToObject != null) return expandToObject;
        }
        if (FullyExpanded) return null;

        List<Directions> directionsLeft = new List<Directions>();
        if (!ExpandedLeft)
        {
            directionsLeft.Add(Directions.Left);
        }
        if (!ExpandedRight)
        {
            directionsLeft.Add(Directions.Right);
        }
        if (!ExpandedUp)
        {
            directionsLeft.Add(Directions.Up);
        }
        if (!ExpandedDown)
        {
            directionsLeft.Add(Directions.Down);
        }
        if (directionsLeft.Count == 0) return null;

        while (directionsLeft.Count > 0)
        {
            Directions RandomDir = directionsLeft[Random.Range(0, directionsLeft.Count)];
            GameObject selectedObject = null;
            switch (RandomDir)
            {
                case Directions.Up:
                    {
                        selectedObject = ExpandUp();
                        directionsLeft.Remove(Directions.Up);
                        break;
                    }
                case Directions.Left:
                    {
                        selectedObject = ExpandLeft();
                        directionsLeft.Remove(Directions.Left);
                        break;
                    }
                case Directions.Right:
                    {
                        selectedObject = ExpandRight();
                        directionsLeft.Remove(Directions.Right);
                        break;
                    }
                case Directions.Down:
                    {
                        selectedObject = ExpandDown();
                        directionsLeft.Remove(Directions.Down);
                        break;
                    }
            }
            CheckFullyExpanded();
            if (selectedObject != null) return selectedObject;
        }
        return null;
    }

    private void CheckFullyExpanded()
    {
        FullyExpanded = (ExpandedDown && ExpandedUp && ExpandedLeft && ExpandedRight);
    }
    private GameObject ExpandUp()
    {
        ExpandedUp = true;
        if (!TurkPuzzleScript.IsCoordinateInsideGrid(cord.x, cord.y + 1)) return null;

        GameObject expandedTo = TurkPuzzleScript.puzzlePieceGrid[cord.x, cord.y + 1];
        TurkCubeScript expandToScript = expandedTo.GetComponent<TurkCubeScript>();
        if (expandToScript.Linked) return null;
        if (!ExpandedToScripts.Contains(expandToScript)) ExpandedToScripts.Add(expandToScript);
        expandToScript.Linked = true;

        return expandedTo;
    }

    private GameObject ExpandDown()
    {
        ExpandedDown = true;
        if (!TurkPuzzleScript.IsCoordinateInsideGrid(cord.x, cord.y - 1)) return null;

        GameObject expandedTo = TurkPuzzleScript.puzzlePieceGrid[cord.x, cord.y - 1];
        TurkCubeScript expandToScript = expandedTo.GetComponent<TurkCubeScript>();
        if (expandToScript.Linked) return null;
        if (!ExpandedToScripts.Contains(expandToScript)) ExpandedToScripts.Add(expandToScript);
        expandToScript.Linked = true;

        return expandedTo;
    }

    private GameObject ExpandLeft()
    {
        ExpandedLeft = true;
        if (!TurkPuzzleScript.IsCoordinateInsideGrid(cord.x - 1, cord.y)) return null;

        GameObject expandedTo = TurkPuzzleScript.puzzlePieceGrid[cord.x - 1, cord.y];
        TurkCubeScript expandToScript = expandedTo.GetComponent<TurkCubeScript>();
        if (expandToScript.Linked) return null;
        if (!ExpandedToScripts.Contains(expandToScript)) ExpandedToScripts.Add(expandToScript);
        expandToScript.Linked = true;

        return expandedTo;
    }

    private GameObject ExpandRight()
    {
        ExpandedRight = true;
        if (!TurkPuzzleScript.IsCoordinateInsideGrid(cord.x + 1, cord.y)) return null;

        GameObject expandedTo = TurkPuzzleScript.puzzlePieceGrid[cord.x + 1, cord.y];
        TurkCubeScript expandToScript = expandedTo.GetComponent<TurkCubeScript>();
        if (expandToScript.Linked) return null;
        if (!ExpandedToScripts.Contains(expandToScript)) ExpandedToScripts.Add(expandToScript);
        expandToScript.Linked = true;

        return expandedTo;
    }
    #endregion
}
