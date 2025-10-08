using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PipeData
{
    public int PipeTypeID; 
    public int Rotations;
}

public class VentGridScript : MonoBehaviour
{
    public GameObject PipeObjectPrefab;

    public int Cols;
    public int Rows;

    public float GridPixelWidth;
    public float GridPixelHeight;
    public float VentPixelWidth;
    public float VentPixelHeight;

    public PipeData[] PipeGridData;
    public GameObject[] PipeStacks;
    public List<PipeStackScript> Goals;
    public List<PipeStackScript> Sources;

    public int MaxPipeTypeIdx = 6;

    void ResetGrid()
    {
        ClearGrid();

        PipeGridData = new PipeData[Cols * Rows];
        PipeStacks = new GameObject[Cols * Rows];

        GridPixelWidth = Cols * VentPixelWidth;
        GridPixelHeight = Rows * VentPixelHeight;

        Goals = new List<PipeStackScript>();
        Sources = new List<PipeStackScript>();

        VentPixelWidth = PipeObjectPrefab.GetComponent<RectTransform>().sizeDelta.x;
        VentPixelHeight = PipeObjectPrefab.GetComponent<RectTransform>().sizeDelta.y;
    }

    public void SpawnGrid()
    {
        ResetGrid();

        for (int i = 0; i < PipeGridData.Length; i++)
        {
            GameObject NewVent = Instantiate(PipeObjectPrefab);
            RectTransform NewVentRect = NewVent.GetComponent<RectTransform>();
            NewVentRect.parent = transform;
            NewVentRect.transform.localPosition = Vector3.zero;
            NewVentRect.localScale = Vector3.one;
            NewVentRect.localRotation = Quaternion.identity;

            (float xPos, float yPos) = IdxToPos(i);
            NewVentRect.anchoredPosition = new Vector2(xPos, yPos);

            PipeStackScript pipeStackScript = NewVent.GetComponent<PipeStackScript>();
            pipeStackScript.GridSource = this;
            pipeStackScript.VentPosID = i;

            pipeStackScript.RotationTracker = Random.Range(0, 4);
            pipeStackScript.SetPipeType(Random.Range(0, MaxPipeTypeIdx));

            PipeStacks[i] = NewVent;
        }
    }

    public void ClearGrid()
    {

        for (int i = 0; i < PipeStacks.Length; i++)
        {
            if (PipeStacks[i] != null) DestroyImmediate(PipeStacks[i]);
        }
    }

    public int ConvertVector2ToPosID(int currentPosID, Direction shiftDirection)
    {
        Vector2 PosIDShift = Vector2.zero;
        switch (shiftDirection)
        {
            case Direction.UP:
                PosIDShift = Vector2.up;
                break;
            case Direction.DOWN:
                PosIDShift = Vector2.down;
                break;
            case Direction.LEFT:
                PosIDShift = Vector2.left;
                break;
            case Direction.RIGHT:
                PosIDShift = Vector2.right;
                break;
        }

        (int x, int y) = IdxToCord(currentPosID);
        Vector2 currentPos = new Vector2(x, y);
        Vector2 newPos = PosIDShift + currentPos;

        int newIdx = CordToIdx((int)newPos.x, (int)newPos.y);

        return newIdx;
    }

    public (bool, bool, PipeStackScript) ExpansionCheck(Direction expansionDirection, int sourceVentID)
    {
        bool validExpansion = false;
        bool secondaryExpansion = true;
        PipeStackScript nextVent = null;

        int nextVentId = ConvertVector2ToPosID(sourceVentID, expansionDirection);
        if(nextVentId < 0) return (validExpansion, secondaryExpansion, nextVent);

        nextVent = PipeStacks[nextVentId].GetComponent<PipeStackScript>();

        switch (expansionDirection)
        {
            case Direction.UP:
                secondaryExpansion = nextVent.DownSecondary;
                if(nextVent.DownConnection == PipeConnectionType.Closed) return (validExpansion, secondaryExpansion, nextVent);
                break;
            case Direction.DOWN:
                secondaryExpansion = nextVent.UpSecondary;
                if (nextVent.UpConnection == PipeConnectionType.Closed) return (validExpansion, secondaryExpansion, nextVent);
                break;
            case Direction.LEFT:
                secondaryExpansion = nextVent.RightSecondary;
                if (nextVent.RightConnection == PipeConnectionType.Closed) return (validExpansion, secondaryExpansion, nextVent);
                break;
            case Direction.RIGHT:
                secondaryExpansion = nextVent.LeftSecondary;
                if (nextVent.LeftConnection == PipeConnectionType.Closed) return (validExpansion, secondaryExpansion, nextVent);
                break;
        }

        return (true, secondaryExpansion, nextVent);
    }

    private int CordToIdx(int col, int row)
    {
        if (col < 0 ||
            col >= Cols ||
            row < 0 ||
            row >= Rows) return -1;
        return Cols * row + col;
    }
    private (int, int) IdxToCord(int idx)
    {
        return (idx%Cols, Mathf.FloorToInt(idx/Cols));
    }
    private (float, float) IdxToPos(int idx)
    {
        (int col, int row) = IdxToCord(idx);
        return CordToPos(col, row);
    }
    private (float, float) CordToPos(int col, int row)
    {
        float leftMostPos = GridPixelWidth / 2f;
        float bottomMostPos = GridPixelHeight / 2f;

        return (-leftMostPos + VentPixelWidth * col + VentPixelWidth/2f, -bottomMostPos + VentPixelHeight * row + VentPixelHeight / 2f);
    }
}
