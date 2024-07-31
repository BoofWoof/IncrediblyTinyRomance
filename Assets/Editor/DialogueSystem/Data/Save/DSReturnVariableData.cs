
using System;
using UnityEngine;

[Serializable]
public class DSReturnVariableData
{
    [field: SerializeField] public string ReturnValueInfoGUID { get; set; }
    [field: SerializeField] public string TypeUuid { get; set; }
    [field: SerializeField] public float ReturnValue { get; set; }

    public DSReturnVariableData clone()
    {
        return new DSReturnVariableData()
        {
            ReturnValueInfoGUID = ReturnValueInfoGUID,
            TypeUuid = TypeUuid,
            ReturnValue = ReturnValue
        };
    }
}