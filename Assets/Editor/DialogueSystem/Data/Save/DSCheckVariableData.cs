using System;
using UnityEngine;

[Serializable]
public class DSCheckVariableData
{
    [field: SerializeField] public string VariableInfoGUID { get; set; }
    [field: SerializeField] public float ThresholdValue { get; set; }

}

