using System.Collections.Generic;
using UnityEngine;


public enum PipeConnectionType
{
    Closed,
    All,
    UpConnected,
    DownConnected,
    LeftConnected,
    RightConnected
}

[System.Serializable]
public struct PipeStruct
{
    public Sprite PipeSprite;

    public PipeConnectionType UpConnection;
    public PipeConnectionType DownConnection;
    public PipeConnectionType LeftConnection;
    public PipeConnectionType RightConnection;
}

[CreateAssetMenu(fileName = "PipeSOHolder", menuName = "Pipes/PipeSOHolder")]
public class PipeSOHolder : ScriptableObject
{
    public List<PipeStruct> PipeStructs;
}
