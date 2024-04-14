using System;
using System.Collections.Generic;
using UnityEngine;

namespace DS.Data.Save
{
    [Serializable]
    public class DSGraphSaveDataSO: ScriptableObject
    {
        [field: SerializeField] public string fileName {  get; set; }
        [field: SerializeField] public List<DSGroupSaveData> Groups { get; set; }
        [field: SerializeField] public List<DSNodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<string> OldGroupNames { get; set; }
        [field: SerializeField] public List<string> OldUngroupedNames { get; set; }
        [field: SerializeField] public SerializableDictionary<string, List<string>> OldGroupNodeNames { get; set; }
    
        public void Initialize(string fileName)
        {
            fileName = fileName;

            Groups = new List<DSGroupSaveData>();
            Nodes = new List<DSNodeSaveData>();
        }
    }
}
