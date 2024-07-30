using System;
using UnityEngine;

namespace DS.Data
{
    [Serializable]
    public class DSCheckInfo
    {
        [field: SerializeField] public DialogueOptionsVariable VariableInfoSO { get; set; }
        [field: SerializeField] public float ThresholdValue { get; set; }

    }

}