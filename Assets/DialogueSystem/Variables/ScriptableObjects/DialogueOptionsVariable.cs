using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewOptionsVariable", menuName = "DialogueVariables/Options")]
public class DialogueOptionsVariable : ScriptableObject
{
    [SerializeField] public string VariableName = "New Variable";
    [SerializeField] public string StartingUuid;

    [SerializeField] public List<string> VariableStates = new List<string>();
    [SerializeField] public List<string> StateUuids = new List<string>();
}
