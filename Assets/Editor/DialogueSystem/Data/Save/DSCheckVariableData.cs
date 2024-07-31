using System;
using System.Reflection.Emit;
using UnityEngine;

[Serializable]
public class DSCheckVariableData
{
    [field: SerializeField] public string VariableInfoGUID { get; set; }
    [field: SerializeField] public float ThresholdValue { get; set; }

    public DSCheckVariableData clone()
    {
        return new DSCheckVariableData()
        {
            VariableInfoGUID = VariableInfoGUID,
            ThresholdValue = ThresholdValue
        };
    }
}

