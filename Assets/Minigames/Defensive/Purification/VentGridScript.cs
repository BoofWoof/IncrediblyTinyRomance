using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
public class VentGridScript : MonoBehaviour
{
    public string LevelName;
    public string LevelDescription;

    public GameObject PipeObjectPrefab;

    public int Cols;
    public int Rows;

    public float GridPixelWidth;
    public float GridPixelHeight;
    public float VentPixelWidth;
    public float VentPixelHeight;

    public GameObject[] PipeStacks;
    public List<PipeStackScript> Goals;
    public List<PipeStackScript> Sources;

    public int MaxPipeTypeIdx = 6;

    void ResetGrid()
    {
        ClearGrid();

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

        for (int i = 0; i < Cols * Rows; i++)
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

    public void SpawnGridFromSaveData(PurificationLevelSO saveData)
    {
        ClearGrid();

        LevelName = saveData.LevelName;
        LevelDescription = saveData.LevelDescription;

        //saveData.ConnectedCutsceneName = "";
        ///saveData.OnScreenData = null;

        Rows = saveData.Rows;
        Cols = saveData.Cols;

        ResetGrid();

        int i = 0;
        foreach (PipeData pipeData in saveData.Data)
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

            pipeStackScript.RotationTracker = pipeData.Rotations;

            pipeStackScript.isCapped = pipeData.isCap;
            pipeStackScript.isSource = pipeData.isSource;
            pipeStackScript.isGoal = pipeData.isSink;

            pipeStackScript.canRotate = pipeData.canRotate;

            pipeStackScript.SetPipeType(pipeData.PipeTypeID);

            PipeStacks[i] = NewVent;

            i++;
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
                if (nextVent.DownConnection == PipeConnectionType.Closed)
                {
                    nextVent.DownLeakParticles.SetActive(true);
                    return (validExpansion, secondaryExpansion, nextVent);
                } else
                {
                    nextVent.DownLeakParticles.SetActive(false);
                }
                break;
            case Direction.DOWN:
                secondaryExpansion = nextVent.UpSecondary;
                if (nextVent.UpConnection == PipeConnectionType.Closed)
                {
                    nextVent.UpLeakParticles.SetActive(true);
                    return (validExpansion, secondaryExpansion, nextVent);
                }
                {
                    nextVent.UpLeakParticles.SetActive(false);
                }
                break;
            case Direction.LEFT:
                secondaryExpansion = nextVent.RightSecondary;
                if (nextVent.RightConnection == PipeConnectionType.Closed)
                {
                    nextVent.RightLeakParticles.SetActive(true);
                    return (validExpansion, secondaryExpansion, nextVent);
                }
                {
                    nextVent.RightLeakParticles.SetActive(false);
                }
                break;
            case Direction.RIGHT:
                secondaryExpansion = nextVent.LeftSecondary;
                if (nextVent.LeftConnection == PipeConnectionType.Closed)
                {
                    nextVent.LeftLeakParticles.SetActive(true);
                    return (validExpansion, secondaryExpansion, nextVent);
                }
                {
                    nextVent.LeftLeakParticles.SetActive(false);
                }
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
