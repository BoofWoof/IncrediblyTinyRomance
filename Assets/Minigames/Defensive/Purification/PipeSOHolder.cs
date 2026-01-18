using System.Collections.Generic;
using UnityEngine;


public enum PipeConnectionType
{
    Closed,
    All,
    UpConnected,
    DownConnected,
    LeftConnected,
    RightConnected,
    Capped
}

[System.Serializable]
public struct PipeStruct
{
    [Header("Sprites")]
    public Sprite PipeSprite;
    public Sprite SecondaryPipeSprite;
    public Sprite SecondaryCapSprite;

    [Header("Sprite Alts")]
    public Sprite SourceVersion;
    public Sprite SinkVersion;
    public Sprite CapVersion;

    [Header("Toggles")]
    public bool EnableBackdrop;
    public bool ShowFan;

    [Header("ConnectionData")]
    public PipeConnectionType UpConnection;
    public bool UpSecondaryVentConnection;
    public PipeConnectionType DownConnection;
    public bool DownSecondaryVentConnection;
    public PipeConnectionType LeftConnection;
    public bool LeftSecondaryVentConnection;
    public PipeConnectionType RightConnection;
    public bool RightSecondaryVentConnection;

    [Header("Force Logic")]
    public bool ForceCapped;
}

[CreateAssetMenu(fileName = "PipeSOHolder", menuName = "Pipes/PipeSOHolder")]
public class PipeSOHolder : ScriptableObject
{
    public List<PipeStruct> PipeStructs;
}
