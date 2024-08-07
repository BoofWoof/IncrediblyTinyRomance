using System;
using UnityEngine;

[Serializable]
public class DSVariableData
{
    [field: SerializeField] public string VariableInfoGUID { get; set; }
    [field: SerializeField] public OperandTypeEnum OperandType { get; set; }
    [field: SerializeField] public float OperandValue { get; set; }
    [field: SerializeField] public string OptionUid { get; set; }

    public DSVariableData clone()
    {
        return new DSVariableData()
        {
            VariableInfoGUID = VariableInfoGUID,
            OperandType = OperandType,
            OperandValue = OperandValue,
            OptionUid = OptionUid
        };
    }

}
