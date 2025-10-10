using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PipeData
{
    public int PipeTypeID;
    public int Rotations;

    public bool isCap;
    public bool isSource;
    public bool isSink;

    public bool canRotate;
}

[CreateAssetMenu(fileName = "PurificationLevelSO", menuName = "PurificationGame/LevelSO")]
public class PurificationLevelSO : ScriptableObject
{
    [Header("Meta Data")]
    public string LevelName;
    public string LevelDescription;
    public Vector3 VentGridPos;
    public float SquareSize;

    [Header("Events")]
    public string ConnectedCutsceneName;
    public GameObject OnScreenData;
    public Vector3 OnScreenDataPos;

    [Header("Data")]
    public int Rows;
    public int Cols;
    public List<PipeData> Data;
}
