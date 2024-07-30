using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewReturnValue", menuName = "DialogueVariables/ReturnValue")]
public class DialogueReturnValue : ScriptableObject
{
    //RETURN OPTIONS
    [SerializeField] public List<string> VariableStates = new List<string>();
    [SerializeField] public List<string> StateUuids = new List<string>();
}