using System;
using UnityEngine;

[Serializable]
public class DSSpeakerData
{
    [field: SerializeField] public string CharacterInfoGUID { get; set; }
    [field: SerializeField] public string SpriteUid { get; set; }
    [field: SerializeField] public string NoiseUid { get; set; }
}
