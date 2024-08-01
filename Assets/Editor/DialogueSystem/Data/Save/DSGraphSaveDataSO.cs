using System;
using System.Collections.Generic;
using UnityEngine;

namespace DS.Data.Save
{
    [Serializable]
    public class DSGraphSaveDataSO: ScriptableObject
    {
        [field: SerializeField] public string FileName {  get; set; }
        [field: SerializeField] public int NodesMade { get; set; }
        [field: SerializeField] public List<DSGroupSaveData> Groups { get; set; }
        [field: SerializeField] public List<DSNodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<string> OldGroupNames { get; set; }
        [field: SerializeField] public List<string> OldUngroupedNames { get; set; }
        [field: SerializeField] public SerializableDictionary<string, List<string>> OldGroupNodeNames { get; set; }
    
        public void Initialize(string fileName, int nodesMade)
        {
            FileName = fileName;
            NodesMade = nodesMade;

            Groups = new List<DSGroupSaveData>();
            Nodes = new List<DSNodeSaveData>();
        }
    }
}
