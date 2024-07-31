using System;
using UnityEngine;

[Serializable]
public class DSReturnValueInfo
{
    [field: SerializeField] public DialogueReturnValue ReturnValueObject { get; set; }
    [field: SerializeField] public string TypeUuid { get; set; }
    [field: SerializeField] public float ReturnValue { get; set; }

}