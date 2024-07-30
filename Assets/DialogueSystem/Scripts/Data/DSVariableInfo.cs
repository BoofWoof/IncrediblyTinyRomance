using System;
using UnityEngine;

public enum OperandTypeEnum
{
    Add,
    Set
}

namespace DS.Data
{
    [Serializable]
    public class DSVariableInfo
    {
        [field: SerializeField] public DialogueOptionsVariable VariableInfoSO { get; set; }
        [field: SerializeField] public OperandTypeEnum OperandType { get; set; }
        [field: SerializeField] public float OperandValue { get; set; }
        [field: SerializeField] public string OptionUid { get; set; }

    }

}