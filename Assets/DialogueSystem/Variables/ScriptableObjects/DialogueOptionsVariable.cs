using System.Collections.Generic;
using UnityEngine;

public enum VariableTypeEnum
{
    Value,
    Option
}

[CreateAssetMenu(fileName = "NewOptionsVariable", menuName = "DialogueVariables/Options")]
public class DialogueOptionsVariable : ScriptableObject
{
    [SerializeField] public string VariableName = "New Variable";
    [SerializeField] public VariableTypeEnum VariableType = VariableTypeEnum.Value;

    //FLOATS
    [SerializeField] public float StartingValue = 0;

    //OPTIONS
    [SerializeField] public string StartingUuid;
    [SerializeField] public List<string> VariableStates = new List<string>();
    [SerializeField] public List<string> StateUuids = new List<string>(); 
    [SerializeField] public string uniqueID;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = System.Guid.NewGuid().ToString();
        }
    }
}
