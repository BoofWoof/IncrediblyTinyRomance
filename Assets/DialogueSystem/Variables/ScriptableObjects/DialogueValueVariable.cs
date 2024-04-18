using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewValueVariable", menuName = "DialogueVariables/Value")]
public class DialogueValueVariable : ScriptableObject
{
    [SerializeField] public string VariableName = "New Variable";
    [SerializeField] public int StartingValue = 0;
}
