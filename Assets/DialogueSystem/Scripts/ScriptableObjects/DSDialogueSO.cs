using System.Collections.Generic;
using UnityEngine;

namespace DS.ScriptableObjects
{
    using DS.Data;
    using Enumerations;
    public class DSDialogueSO : ScriptableObject
    {
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField] [field: TextArea()] public string Text { get; set; }
        [field: SerializeField] public bool SkipText { get; set; }
        [field: SerializeField] public List<DSDialogueChoiceData> Choices { get; set; }
        [field: SerializeField] public DSDialogueType DialogueType { get; set; }
        [field: SerializeField] public bool IsStartingDialogue { get; set; }
        [field: SerializeField] public DSSpeakerInfo SpeakerInfo { get; set; }
        [field: SerializeField] public DSVariableInfo VariableInfo { get; set; }
        [field: SerializeField] public DSCheckInfo CheckInfo { get; set; }
        [field: SerializeField] public DSReturnValueInfo ReturnValueInfo { get; set; }

        public void Initialize(string dialogueName, string text, bool skipText, List<DSDialogueChoiceData> choices, DSDialogueType dialogueType, bool isStartingDialogue, DSSpeakerInfo speakerInfo, DSVariableInfo variableInfo, DSCheckInfo checkInfo, DSReturnValueInfo returnValueInfo)
        {
            DialogueName = dialogueName;
            Text = text;
            SkipText = skipText;
            Choices = choices;
            DialogueType = dialogueType;
            IsStartingDialogue = isStartingDialogue;
            SpeakerInfo = speakerInfo;
            VariableInfo = variableInfo;
            CheckInfo = checkInfo;
            ReturnValueInfo = returnValueInfo;
        }
    }
}
