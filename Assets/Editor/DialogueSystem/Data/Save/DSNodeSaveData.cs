using System;
using System.Collections.Generic;
using UnityEngine;


namespace DS.Data.Save
{
    using DS.Enumerations;

    [Serializable]
    public class DSNodeSaveData
    {
        [field: SerializeField] public string ID {  get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Text {  get; set; }
        [field: SerializeField] public bool SkipText { get; set; }
        [field: SerializeField] public List<DSChoiceSaveData> Choices { get; set; }
        [field: SerializeField] public string GroupID {  get; set; }
        [field: SerializeField] public DSDialogueType DialogueType { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
        [field: SerializeField] public DSSpeakerData SpeakerInfo { get; set; }
        [field: SerializeField] public DSVariableData VariableInfo { get; set; }
        [field: SerializeField] public DSCheckVariableData CheckVariableInfo { get; set; }
        [field: SerializeField] public DSReturnVariableData ReturnValueInfo { get; set; }
    }
}
